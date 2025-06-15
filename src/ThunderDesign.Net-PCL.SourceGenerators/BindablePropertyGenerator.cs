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

            var compilationProvider = context.CompilationProvider;

            context.RegisterSourceOutput(fieldsWithAttribute.Combine(compilationProvider), (spc, tuple) =>
            {
                var (info, compilation) = (tuple.Left, tuple.Right);
                GenerateBindableProperty(spc, info, compilation);
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

        private static void GenerateBindableProperty(SourceProductionContext context, BindableFieldInfo info, Compilation compilation)
        {
            var classSymbol = info.ContainingClass;
            var fieldSymbol = info.FieldSymbol;
            var fieldName = fieldSymbol.Name;
            var propertyName = PropertyGeneratorHelpers.ToPropertyName(fieldName);
            var typeName = fieldSymbol.Type.ToDisplayString();

            // Rule 1: Class must be partial
            if (!PropertyGeneratorHelpers.IsPartial(classSymbol))
            {
                PropertyGeneratorHelpers.ReportDiagnostic(context, info.FieldDeclaration.GetLocation(), $"Class '{classSymbol.Name}' must be partial to use [BindableProperty].");
                return;
            }

            // Rule 2: Field must start with "_" followed by a letter, or a lowercase letter
            if (!PropertyGeneratorHelpers.IsValidFieldName(fieldName))
            {
                PropertyGeneratorHelpers.ReportDiagnostic(context, info.FieldDeclaration.GetLocation(), $"Field '{fieldName}' must start with '_' followed by a letter, or a lowercase letter to use [BindableProperty].");
                return;
            }

            // Rule 3: Property must not already exist
            if (PropertyGeneratorHelpers.PropertyExists(classSymbol, propertyName))
            {
                PropertyGeneratorHelpers.ReportDiagnostic(context, info.FieldDeclaration.GetLocation(), $"Property '{propertyName}' already exists in '{classSymbol.Name}'.");
                return;
            }

            // Attribute arguments
            var threadSafe = info.AttributeData.ConstructorArguments.Length > 0 && (bool)info.AttributeData.ConstructorArguments[0].Value!;
            var notify = info.AttributeData.ConstructorArguments.Length > 1 && (bool)info.AttributeData.ConstructorArguments[1].Value!;
            var readOnly = info.AttributeData.ConstructorArguments.Length > 2 && (bool)info.AttributeData.ConstructorArguments[2].Value!;

            // Check for INotifyPropertyChanged, IBindableObject, ThreadObject
            var implementsINotify = ImplementsInterface(classSymbol, "System.ComponentModel.INotifyPropertyChanged");
            var implementsIBindable = ImplementsInterface(classSymbol, "ThunderDesign.Net.Threading.Interfaces.IBindableObject");
            var inheritsThreadObject = PropertyGeneratorHelpers.InheritsFrom(classSymbol, "ThunderDesign.Net.Threading.Objects.ThreadObject");

            var stringTypeSymbol = compilation.GetSpecialType(SpecialType.System_String);
            var voidTypeSymbol = compilation.GetSpecialType(SpecialType.System_Void);
            var propertyChangedEventType = compilation.GetTypeByMetadataName("System.ComponentModel.PropertyChangedEventHandler");

            var source = new StringBuilder();
            var ns = classSymbol.ContainingNamespace.IsGlobalNamespace ? null : classSymbol.ContainingNamespace.ToDisplayString();

            if (!string.IsNullOrEmpty(ns))
            {
                source.AppendLine($"namespace {ns} {{");
            }

            source.AppendLine("using ThunderDesign.Net.Threading.Extentions;");
            source.AppendLine("using ThunderDesign.Net.Threading.Interfaces;");
            source.AppendLine("using ThunderDesign.Net.Threading.Objects;");

            source.AppendLine($"partial class {classSymbol.Name}");

            // Add interface if needed
            var interfaces = new List<string>();
            if (!implementsIBindable)
                interfaces.Add("IBindableObject");
            if (interfaces.Count > 0)
                source.Append(" : " + string.Join(", ", interfaces));
            source.AppendLine();
            source.AppendLine("{");

            // Add event if needed
            if (!implementsINotify && !PropertyGeneratorHelpers.EventExists(classSymbol, "PropertyChanged", propertyChangedEventType))
            {
                if (PropertyGeneratorHelpers.EventExists(classSymbol, "PropertyChanged"))
                {
                    PropertyGeneratorHelpers.ReportDiagnostic(
                        context,
                        info.FieldDeclaration.GetLocation(),
                        $"Event PropertyChanged already exists in '{classSymbol.Name}' with a different type. Expected: System.ComponentModel.PropertyChangedEventHandler."
                    );
                }
                else
                {
                    source.AppendLine("    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;");
                }
            }

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

            // Add property
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
                source.AppendLine($@"
    public {typeName} {propertyName}
    {{
        get {{ return this.GetProperty(ref {fieldName}, {lockerArg}); }}
        set {{ this.SetProperty(ref {fieldName}, value, {lockerArg}, {notifyArg}); }}
    }}");
            }

            source.AppendLine("}");

            if (!string.IsNullOrEmpty(ns))
                source.AppendLine("}");

            context.AddSource($"{classSymbol.Name}_{propertyName}_BindableProperty.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
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