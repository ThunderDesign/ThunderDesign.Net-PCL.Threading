using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThunderDesign.Net_PCL.SourceGenerators
{
    [Generator]
    public class BindablePropertyGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var fieldsWithAttribute = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => node is FieldDeclarationSyntax fds && fds.AttributeLists.Count > 0,
                    transform: static (ctx, _) => GetBindableField(ctx)
                )
                .Where(static info => !info.Equals(default(BindableFieldInfo)));

            // Group fields by containing class
            var grouped = fieldsWithAttribute.Collect()
                .Select((list, _) => list
                    .GroupBy(info => info.ContainingClass, SymbolEqualityComparer.Default)
                    .Select(g => (ClassSymbol: g.Key, Fields: g.ToList()))
                    .ToList()
                );

            var compilationProvider = context.CompilationProvider;

            context.RegisterSourceOutput(grouped.Combine(compilationProvider), (spc, tuple) =>
            {
                var (classGroups, compilation) = (tuple.Left, tuple.Right);
                foreach (var group in classGroups)
                {
                    var classSymbol = group.ClassSymbol as INamedTypeSymbol;
                    var fields = group.Fields;
                    if (classSymbol != null)
                    {
                        GenerateBindablePropertyClass(spc, classSymbol, fields, compilation);
                    }
                }
            });
        }

        private static BindableFieldInfo GetBindableField(GeneratorSyntaxContext context)
        {
            var fieldDecl = (FieldDeclarationSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            foreach (var variable in fieldDecl.Declaration.Variables)
            {
                var symbol = semanticModel.GetDeclaredSymbol(variable);
                if (symbol is IFieldSymbol fieldSymbol)
                {
                    foreach (var attr in fieldSymbol.GetAttributes())
                    {
                        if (attr.AttributeClass?.Name == "BindablePropertyAttribute")
                        {
                            var containingClass = fieldSymbol.ContainingType;
                            return new BindableFieldInfo
                            {
                                FieldSymbol = fieldSymbol,
                                ContainingClass = containingClass,
                                AttributeData = attr,
                                FieldDeclaration = fieldDecl
                            };
                        }
                    }
                }
            }
            return default(BindableFieldInfo);
        }

        // New method to generate all properties and shared members for a class
        private static void GenerateBindablePropertyClass(
            SourceProductionContext context,
            INamedTypeSymbol classSymbol,
            List<BindableFieldInfo> fields,
            Compilation compilation)
        {
            var implementsINotify = ImplementsInterface(classSymbol, "System.ComponentModel.INotifyPropertyChanged");
            var implementsIBindable = ImplementsInterface(classSymbol, "ThunderDesign.Net.Threading.Interfaces.IBindableObject");
            var inheritsThreadObject = PropertyGeneratorHelpers.InheritsFrom(classSymbol, "ThunderDesign.Net.Threading.Objects.ThreadObject");

            var stringTypeSymbol = compilation.GetSpecialType(SpecialType.System_String);
            var voidTypeSymbol = compilation.GetSpecialType(SpecialType.System_Void);
            var propertyChangedEventType = compilation.GetTypeByMetadataName("System.ComponentModel.PropertyChangedEventHandler");

            var source = new StringBuilder();
            var ns = classSymbol.ContainingNamespace.IsGlobalNamespace ? null : classSymbol.ContainingNamespace.ToDisplayString();

            if (!string.IsNullOrEmpty(ns))
                source.AppendLine($"namespace {ns} {{");

            source.AppendLine("using ThunderDesign.Net.Threading.Extentions;");
            source.AppendLine("using ThunderDesign.Net.Threading.Interfaces;");
            source.AppendLine("using ThunderDesign.Net.Threading.Objects;");

            source.Append($"partial class {classSymbol.Name}");
            var interfaces = new List<string>();
            if (!implementsIBindable)
                interfaces.Add("IBindableObject");
            if (interfaces.Count > 0)
                source.Append(" : " + string.Join(", ", interfaces));
            source.AppendLine();
            source.AppendLine("{");

            // Add event if needed
            if (!implementsINotify && !PropertyGeneratorHelpers.EventExists(classSymbol, "PropertyChanged", propertyChangedEventType))
                source.AppendLine("    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;");

            // Add _Locker if needed
            if (!inheritsThreadObject && !PropertyGeneratorHelpers.FieldExists(classSymbol, "_Locker"))
                source.AppendLine("    protected readonly object _Locker = new object();");

            // Add OnPropertyChanged if needed
            if (!implementsIBindable && !PropertyGeneratorHelpers.MethodExists(
                classSymbol,
                "OnPropertyChanged",
                new ITypeSymbol[] { stringTypeSymbol },
                voidTypeSymbol))
            {
                source.AppendLine(@"
    public virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = """")
    {
        this.NotifyPropertyChanged(PropertyChanged, propertyName);
    }");
            }

            // Generate all properties
            foreach (var info in fields)
            {
                var fieldSymbol = info.FieldSymbol;
                var fieldName = fieldSymbol.Name;
                var propertyName = PropertyGeneratorHelpers.ToPropertyName(fieldName);
                var typeName = fieldSymbol.Type.ToDisplayString();

                var readOnly = info.AttributeData.ConstructorArguments.Length > 0 && (bool)info.AttributeData.ConstructorArguments[0].Value!;
                var threadSafe = info.AttributeData.ConstructorArguments.Length > 1 && (bool)info.AttributeData.ConstructorArguments[1].Value!;
                var notify = info.AttributeData.ConstructorArguments.Length > 2 && (bool)info.AttributeData.ConstructorArguments[2].Value!;
                string[] alsoNotify = null;
                if (info.AttributeData.ConstructorArguments.Length > 3)
                {
                    var arg = info.AttributeData.ConstructorArguments[3];
                    if (arg.Kind == TypedConstantKind.Array && arg.Values != null)
                    {
                        alsoNotify = arg.Values
                            .Select(tc => tc.Value as string)
                            .Where(s => !string.IsNullOrEmpty(s))
                            .ToArray();
                    }
                }
                if (alsoNotify == null)
                    alsoNotify = new string[0];

                var lockerArg = threadSafe ? "_Locker" : "null";
                var notifyArg = notify ? "true" : "false";
                if (readOnly)
                {
                    source.AppendLine($@"
    public {typeName} {propertyName}
    {{
        get {{ return this.GetProperty(ref {fieldName}, {lockerArg}); }}
    }}");
                }
                else
                {
                    string setAccessor;
                    if (alsoNotify.Length > 0)
                    {
                        var notifyCalls = new StringBuilder();
                        foreach (var prop in alsoNotify)
                        {
                            if (!string.IsNullOrEmpty(prop))
                                notifyCalls.AppendLine($"                this.OnPropertyChanged(\"{prop}\");");
                        }
                        setAccessor = $@"
        set
        {{
            if (this.SetProperty(ref {fieldName}, value, {lockerArg}, {notifyArg}))
            {{
{notifyCalls.ToString().TrimEnd()}
            }}
        }}";
                    }
                    else
                    {
                        setAccessor = $"set {{ this.SetProperty(ref {fieldName}, value, {lockerArg}, {notifyArg}); }}";
                    }

                    source.AppendLine($@"
    public {typeName} {propertyName}
    {{
        get {{ return this.GetProperty(ref {fieldName}, {lockerArg}); }}
        {setAccessor}
    }}");
                }
            }

            source.AppendLine("}");

            if (!string.IsNullOrEmpty(ns))
                source.AppendLine("}");

            context.AddSource($"{classSymbol.Name}_BindableProperties.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
        }

        private static bool ImplementsInterface(INamedTypeSymbol type, string interfaceName)
        {
            return type.AllInterfaces.Any(i => i.ToDisplayString() == interfaceName);
        }

        private struct BindableFieldInfo
        {
            public IFieldSymbol FieldSymbol { get; set; }
            public INamedTypeSymbol ContainingClass { get; set; }
            public AttributeData AttributeData { get; set; }
            public FieldDeclarationSyntax FieldDeclaration { get; set; }
        }
    }
}