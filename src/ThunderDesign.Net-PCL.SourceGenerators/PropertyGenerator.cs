using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace ThunderDesign.Net_PCL.SourceGenerators
{
    [Generator]
    public class PropertyGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var fieldsWithAttribute = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => node is FieldDeclarationSyntax fds && fds.AttributeLists.Count > 0,
                    transform: static (ctx, _) => PropertyGeneratorHelpers.GetFieldWithAttribute(ctx, "PropertyAttribute")
                )
                .Where(static info => !info.Equals(default(PropertyFieldInfo)));

            var compilationProvider = context.CompilationProvider;
            context.RegisterSourceOutput(fieldsWithAttribute.Combine(compilationProvider), (spc, tuple) =>
            {
                var (info, compilation) = (tuple.Left, tuple.Right);
                GenerateProperty(spc, info, compilation);
            });
        }

        private static void GenerateProperty(SourceProductionContext context, PropertyFieldInfo info, Compilation compilation)
        {
            var classSymbol = info.ContainingClass;
            var fieldSymbol = info.FieldSymbol;
            var fieldName = fieldSymbol.Name;
            var propertyName = PropertyGeneratorHelpers.ToPropertyName(fieldName);
            var typeName = fieldSymbol.Type.ToDisplayString();

            // Rule 1: Class must be partial
            if (!PropertyGeneratorHelpers.IsPartial(classSymbol))
            {
                PropertyGeneratorHelpers.ReportDiagnostic(context, info.FieldDeclaration.GetLocation(), $"Class '{classSymbol.Name}' must be partial to use [Property].");
                return;
            }

            // Rule 2: Field must start with "_" followed by a letter, or a lowercase letter
            if (!PropertyGeneratorHelpers.IsValidFieldName(fieldName))
            {
                PropertyGeneratorHelpers.ReportDiagnostic(context, info.FieldDeclaration.GetLocation(), $"Field '{fieldName}' must start with '_' followed by a letter, or a lowercase letter to use [Property].");
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
            var readOnly = info.AttributeData.ConstructorArguments.Length > 1 && (bool)info.AttributeData.ConstructorArguments[1].Value!;

            // Check for ThreadObject
            var inheritsThreadObject = PropertyGeneratorHelpers.InheritsFrom(classSymbol, "ThunderDesign.Net.Threading.Objects.ThreadObject");

            var source = new StringBuilder();
            var ns = classSymbol.ContainingNamespace.IsGlobalNamespace ? null : classSymbol.ContainingNamespace.ToDisplayString();

            if (!string.IsNullOrEmpty(ns))
            {
                source.AppendLine($"namespace {ns} {{");
            }

            source.AppendLine("using ThunderDesign.Net.Threading.Extentions;");
            source.AppendLine("using ThunderDesign.Net.Threading.Objects;");

            source.AppendLine($"partial class {classSymbol.Name}");
            source.AppendLine("{");

            // Add _Locker if needed
            if (!inheritsThreadObject && !PropertyGeneratorHelpers.FieldExists(classSymbol, "_Locker"))
                source.AppendLine("    protected readonly object _Locker = new object();");

            // Add property
            var lockerArg = threadSafe ? "_Locker" : "null";
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
        set {{ this.SetProperty(ref {fieldName}, value, {lockerArg}); }}
    }}");
            }

            source.AppendLine("}");

            if (!string.IsNullOrEmpty(ns))
                source.AppendLine("}");

            context.AddSource($"{classSymbol.Name}_{propertyName}_Property.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
        }
    }

    public struct PropertyFieldInfo
    {
        public IFieldSymbol FieldSymbol { get; set; }
        public INamedTypeSymbol ContainingClass { get; set; }
        public AttributeData AttributeData { get; set; }
        public FieldDeclarationSyntax FieldDeclaration { get; set; }
    }
}