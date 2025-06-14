using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ThunderDesign.Net_PCL.SourceGenerators
{
    internal static class PropertyGeneratorHelpers
    {
        public static string ToPropertyName(string fieldName)
        {
            if (fieldName.StartsWith("_"))
                fieldName = fieldName.Substring(1);
            if (fieldName.Length == 0)
                return fieldName;
            return char.ToUpper(fieldName[0]) + fieldName.Substring(1);
        }

        public static bool IsPartial(INamedTypeSymbol classSymbol)
        {
            return classSymbol.DeclaringSyntaxReferences
                .Select(r => r.GetSyntax())
                .OfType<ClassDeclarationSyntax>()
                .Any(c => c.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)));
        }

        public static bool InheritsFrom(INamedTypeSymbol type, string baseTypeMetadataName)
        {
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.ToDisplayString() == baseTypeMetadataName)
                    return true;
                baseType = baseType.BaseType;
            }
            return false;
        }

        public static void ReportDiagnostic(SourceProductionContext context, Location location, string message)
        {
            var descriptor = new DiagnosticDescriptor(
                id: "TDGEN002",
                title: "Property Error",
                messageFormat: message,
                category: "ThunderDesign.Net-PCL.SourceGenerators",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true);
            context.ReportDiagnostic(Diagnostic.Create(descriptor, location));
        }

        public static PropertyFieldInfo GetFieldWithAttribute(GeneratorSyntaxContext context, string attributeName)
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
                        if (attr.AttributeClass?.Name == attributeName)
                        {
                            var containingClass = fieldSymbol.ContainingType;
                            return new PropertyFieldInfo
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
            return default(PropertyFieldInfo);
        }

        // Rule 2: Field must start with "_" followed by a letter, or a lowercase letter
        public static bool IsValidFieldName(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                return false;

            if (fieldName.StartsWith("_"))
            {
                // Must have at least two characters and the second must be a letter (any case)
                return fieldName.Length > 1 && char.IsLetter(fieldName[1]);
            }
            else
            {
                // First character must be a lowercase letter
                return char.IsLower(fieldName[0]);
            }
        }

        // Rule 3: Property must not already exist
        public static bool PropertyExists(INamedTypeSymbol classSymbol, string propertyName)
        {
            return classSymbol.GetMembers().OfType<IPropertySymbol>().Any(p => p.Name == propertyName);
        }
    }
}