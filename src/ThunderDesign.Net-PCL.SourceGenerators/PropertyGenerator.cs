using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
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

            // Group fields by containing class
            var grouped = fieldsWithAttribute.Collect()
                .Select((list, _) => list
                    .Where(info => info.ContainingClass is INamedTypeSymbol) // Filter to only INamedTypeSymbol
                    .GroupBy(info => (INamedTypeSymbol)info.ContainingClass, SymbolEqualityComparer.Default)
                    .Select(g => (ClassSymbol: g.Key, Fields: (IList<PropertyFieldInfo>)g.ToList()))
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
                        GeneratePropertyClass(spc, classSymbol, fields, compilation);
                    }
                }
            });
        }

        // New method to generate all properties for a class
        private static void GeneratePropertyClass(
            SourceProductionContext context,
            INamedTypeSymbol classSymbol,
            IList<PropertyFieldInfo> fields, // Use IList<T> for compatibility
            Compilation compilation)
        {
            var inheritsThreadObject = PropertyGeneratorHelpers.InheritsFrom(classSymbol, "ThunderDesign.Net.Threading.Objects.ThreadObject");

            var source = new StringBuilder();
            var ns = classSymbol.ContainingNamespace.IsGlobalNamespace ? null : classSymbol.ContainingNamespace.ToDisplayString();

            if (!string.IsNullOrEmpty(ns))
                source.AppendLine($"namespace {ns} {{");

            source.AppendLine("using ThunderDesign.Net.Threading.Extentions;");
            source.AppendLine("using ThunderDesign.Net.Threading.Objects;");

            source.AppendLine($"partial class {classSymbol.Name}");
            source.AppendLine("{");

            // Add _Locker if needed
            if (!inheritsThreadObject && !PropertyGeneratorHelpers.FieldExists(classSymbol, "_Locker"))
                source.AppendLine("    protected readonly object _Locker = new object();");

            // Generate all properties
            foreach (var info in fields)
            {
                var fieldSymbol = info.FieldSymbol;
                var fieldName = fieldSymbol.Name;
                var propertyName = PropertyGeneratorHelpers.ToPropertyName(fieldName);
                var typeName = fieldSymbol.Type.ToDisplayString();

                var readOnly = info.AttributeData.ConstructorArguments.Length > 0 && (bool)info.AttributeData.ConstructorArguments[0].Value!;
                var threadSafe = info.AttributeData.ConstructorArguments.Length > 1 && (bool)info.AttributeData.ConstructorArguments[1].Value!;

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
            }

            source.AppendLine("}");

            if (!string.IsNullOrEmpty(ns))
                source.AppendLine("}");

            context.AddSource($"{classSymbol.Name}_Properties.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
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