using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using ThunderDesign.Net.SourceGenerators.Helpers;

namespace ThunderDesign.Net.SourceGenerators
{
    [Generator]
    public class UnifiedPropertyGenerator : IIncrementalGenerator
    {
        private static readonly Dictionary<int, string> AccessibilityNameMap = new Dictionary<int, string>
        {
            { 0, "Public" },
            { 1, "Private" },
            { 2, "Protected" },
            { 3, "Internal" },
            { 4, "ProtectedInternal" },
            { 5, "PrivateProtected" }
        };

        private static readonly Dictionary<string, int> AccessibilityRankMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Public", 6 },
            { "ProtectedInternal", 5 },
            { "Internal", 4 },
            { "Protected", 3 },
            { "PrivateProtected", 2 },
            { "Private", 1 }
        };

        private static readonly Dictionary<string, string> AccessibilityKeywordMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Public", "public " },
            { "Private", "private " },
            { "Protected", "protected " },
            { "Internal", "internal " },
            { "ProtectedInternal", "protected internal " },
            { "PrivateProtected", "private protected " }
        };

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            //if (!Debugger.IsAttached)
            //{
            //    Debugger.Launch();
            //}
            // Collect all fields with [BindableProperty] or [Property]
            var fieldsWithAttribute = context.SyntaxProvider
                .CreateSyntaxProvider<(INamedTypeSymbol Class, BindableFieldInfo Bindable, PropertyFieldInfo Property)>(
                    predicate: static (node, _) => node is FieldDeclarationSyntax fds && fds.AttributeLists.Count > 0,
                    transform: GetFieldInfos
                )
                .Where(static info => !info.Equals(default((INamedTypeSymbol, BindableFieldInfo, PropertyFieldInfo))));

            // Group by class - use the original approach which was working before
            var grouped = fieldsWithAttribute.Collect()
                .Select((list, _) => list
                    .Where(info => info.Class is INamedTypeSymbol)
                    .GroupBy(info => info.Class, SymbolEqualityComparer.Default)
                    .Select(g => (
                        ClassSymbol: g.Key as INamedTypeSymbol, // Explicit cast to INamedTypeSymbol
                        BindableFields: g.Select(x => x.Bindable).Where(b => !b.Equals(default(BindableFieldInfo))).ToList(),
                        PropertyFields: g.Select(x => x.Property).Where(p => !p.Equals(default(PropertyFieldInfo))).ToList()
                    ))
                    .ToList()
                );

            context.RegisterSourceOutput(
                grouped.Combine(context.CompilationProvider),
                GenerateSourceCode
            );
        }

        private static (INamedTypeSymbol Class, BindableFieldInfo Bindable, PropertyFieldInfo Property) GetFieldInfos(GeneratorSyntaxContext ctx, CancellationToken _)
        {
            var bindable = GetBindableField(ctx);
            if (!bindable.Equals(default(BindableFieldInfo)))
                return (Class: bindable.ContainingClass, Bindable: bindable, Property: default(PropertyFieldInfo));
            
            var prop = PropertyGeneratorHelpers.GetFieldWithAttribute(ctx, "PropertyAttribute");
            if (!prop.Equals(default(PropertyFieldInfo)))
                return (Class: prop.ContainingClass, Bindable: default(BindableFieldInfo), Property: prop);
            
            return default;
        }

        private static void GenerateSourceCode(SourceProductionContext spc, 
            (List<(INamedTypeSymbol ClassSymbol, List<BindableFieldInfo> BindableFields, List<PropertyFieldInfo> PropertyFields)> Left, 
            Compilation Right) tuple)
        {
            var (classGroups, compilation) = tuple;
            foreach (var group in classGroups)
            {
                if (group.ClassSymbol != null)
                {
                    GenerateUnifiedPropertyClass(spc, group.ClassSymbol, group.BindableFields, group.PropertyFields, compilation);
                }
            }
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
                            return new BindableFieldInfo
                            {
                                FieldSymbol = fieldSymbol,
                                ContainingClass = fieldSymbol.ContainingType,
                                AttributeData = attr,
                                FieldDeclaration = fieldDecl
                            };
                        }
                    }
                }
            }
            return default;
        }

        private static void GenerateUnifiedPropertyClass(
            SourceProductionContext context,
            INamedTypeSymbol classSymbol,
            List<BindableFieldInfo> bindableFields,
            List<PropertyFieldInfo> propertyFields,
            Compilation compilation)
        {
            if (!ValidateFields(context, classSymbol, bindableFields, propertyFields))
                return;

            var implementsINotify = ImplementsInterface(classSymbol, "System.ComponentModel.INotifyPropertyChanged");
            var implementsIBindable = ImplementsInterface(classSymbol, "ThunderDesign.Net.Threading.Interfaces.IBindableObject");
            var inheritsThreadObject = PropertyGeneratorHelpers.InheritsFrom(classSymbol, "ThunderDesign.Net.Threading.Objects.ThreadObject");

            var stringTypeSymbol = compilation.GetSpecialType(SpecialType.System_String);
            var voidTypeSymbol = compilation.GetSpecialType(SpecialType.System_Void);
            var propertyChangedEventType = compilation.GetTypeByMetadataName("System.ComponentModel.PropertyChangedEventHandler");

            var source = new StringBuilder();
            GenerateClassHeader(source, classSymbol, bindableFields, implementsIBindable);
            
            // Add infrastructure members if needed
            GenerateInfrastructureMembers(
                source, 
                bindableFields, 
                implementsINotify, 
                implementsIBindable, 
                inheritsThreadObject,
                classSymbol, 
                propertyChangedEventType, 
                stringTypeSymbol, 
                voidTypeSymbol);

            // Generate properties
            GenerateBindableProperties(source, bindableFields, classSymbol, compilation);
            GenerateRegularProperties(source, propertyFields, classSymbol, compilation);

            source.AppendLine("}");
            if (!string.IsNullOrEmpty(classSymbol.ContainingNamespace?.ToDisplayString()))
                source.AppendLine("}");

            // Generate unique filename
            var safeClassName = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                .Replace(".", "_")
                .Replace("global::", "");
            var hintName = $"{safeClassName}_AllProperties.g.cs";

            context.AddSource(hintName, SourceText.From(source.ToString(), Encoding.UTF8));
        }

        private static bool ValidateFields(
            SourceProductionContext context,
            INamedTypeSymbol classSymbol,
            List<BindableFieldInfo> bindableFields,
            List<PropertyFieldInfo> propertyFields)
        {
            // Check bindable fields
            foreach (var info in bindableFields)
            {
                if (!PropertyGeneratorHelpers.IsPartial(classSymbol))
                {
                    PropertyGeneratorHelpers.ReportDiagnostic(context, info.FieldDeclaration.GetLocation(), 
                        $"Class '{classSymbol.Name}' must be partial to use [BindableProperty].");
                    return false;
                }
                
                if (!PropertyGeneratorHelpers.IsValidFieldName(info.FieldSymbol.Name))
                {
                    PropertyGeneratorHelpers.ReportDiagnostic(context, info.FieldDeclaration.GetLocation(), 
                        $"Field '{info.FieldSymbol.Name}' must start with '_' or a lowercase letter to use [BindableProperty].");
                    return false;
                }
                
                var propertyName = PropertyGeneratorHelpers.ToPropertyName(info.FieldSymbol.Name);
                if (PropertyGeneratorHelpers.PropertyExists(classSymbol, propertyName))
                {
                    PropertyGeneratorHelpers.ReportDiagnostic(context, info.FieldDeclaration.GetLocation(), 
                        $"Property '{propertyName}' already exists in '{classSymbol.Name}'.");
                    return false;
                }
            }

            // Check property fields
            foreach (var info in propertyFields)
            {
                if (!PropertyGeneratorHelpers.IsPartial(classSymbol))
                {
                    PropertyGeneratorHelpers.ReportDiagnostic(context, info.FieldDeclaration.GetLocation(), 
                        $"Class '{classSymbol.Name}' must be partial to use [Property].");
                    return false;
                }
                
                if (!PropertyGeneratorHelpers.IsValidFieldName(info.FieldSymbol.Name))
                {
                    PropertyGeneratorHelpers.ReportDiagnostic(context, info.FieldDeclaration.GetLocation(), 
                        $"Field '{info.FieldSymbol.Name}' must start with '_' or a lowercase letter to use [Property].");
                    return false;
                }
                
                var propertyName = PropertyGeneratorHelpers.ToPropertyName(info.FieldSymbol.Name);
                if (PropertyGeneratorHelpers.PropertyExists(classSymbol, propertyName))
                {
                    PropertyGeneratorHelpers.ReportDiagnostic(context, info.FieldDeclaration.GetLocation(), 
                        $"Property '{propertyName}' already exists in '{classSymbol.Name}'.");
                    return false;
                }
            }
            
            return true;
        }

        private static void GenerateClassHeader(
            StringBuilder source, 
            INamedTypeSymbol classSymbol, 
            List<BindableFieldInfo> bindableFields, 
            bool implementsIBindable)
        {
            var ns = classSymbol.ContainingNamespace?.ToDisplayString();
            
            if (!string.IsNullOrEmpty(ns))
                source.AppendLine($"namespace {ns} {{");

            source.AppendLine("#nullable enable");
            source.AppendLine("using ThunderDesign.Net.Threading.Extentions;");
            source.AppendLine("using ThunderDesign.Net.Threading.Objects;");
            
            if (bindableFields.Count > 0)
                source.AppendLine("using ThunderDesign.Net.Threading.Interfaces;");

            source.Append($"partial class {classSymbol.Name}");
            
            if (bindableFields.Count > 0 && !implementsIBindable)
                source.Append(" : IBindableObject");
                
            source.AppendLine();
            source.AppendLine("{");
        }

        private static void GenerateInfrastructureMembers(
            StringBuilder source,
            List<BindableFieldInfo> bindableFields,
            bool implementsINotify,
            bool implementsIBindable,
            bool inheritsThreadObject,
            INamedTypeSymbol classSymbol,
            INamedTypeSymbol propertyChangedEventType,
            ITypeSymbol stringTypeSymbol,
            ITypeSymbol voidTypeSymbol)
        {
            // Add event if needed
            if (bindableFields.Count > 0 && !implementsINotify && 
                !PropertyGeneratorHelpers.EventExists(classSymbol, "PropertyChanged", propertyChangedEventType))
            {
                source.AppendLine("    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;");
            }

            // Add _Locker if needed
            if ((!inheritsThreadObject) && !PropertyGeneratorHelpers.FieldExists(classSymbol, "_Locker"))
            {
                source.AppendLine("    protected readonly object _Locker = new object();");
            }

            // Add OnPropertyChanged if needed
            if (bindableFields.Count > 0 && !implementsIBindable && !PropertyGeneratorHelpers.MethodExists(
                classSymbol,
                "OnPropertyChanged",
                new ITypeSymbol[] { stringTypeSymbol },
                voidTypeSymbol))
            {
                // Only use 'virtual' if the class is not sealed
                string virtualModifier = classSymbol.IsSealed ? "" : "virtual ";
                
                source.AppendLine($@"
    public {virtualModifier}void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = """")
    {{
        this.NotifyPropertyChanged(PropertyChanged, propertyName);
    }}");
            }
        }

        private static void GenerateBindableProperties(
            StringBuilder source, 
            List<BindableFieldInfo> bindableFields, 
            INamedTypeSymbol classSymbol,
            Compilation compilation)
        {
            foreach (var info in bindableFields)
            {
                var propertyName = PropertyGeneratorHelpers.ToPropertyName(info.FieldSymbol.Name);
                if (!PropertyGeneratorHelpers.IsPartial(classSymbol) ||
                    !PropertyGeneratorHelpers.IsValidFieldName(info.FieldSymbol.Name) ||
                    PropertyGeneratorHelpers.PropertyExists(classSymbol, propertyName))
                {
                    continue;
                }

                var fieldSymbol = info.FieldSymbol;
                var fieldName = fieldSymbol.Name;
                
                // Use NullableFlowState-aware display string to properly handle nullable types
                var typeName = GetNullableAwareTypeName(fieldSymbol.Type, compilation);

                var args = info.AttributeData.ConstructorArguments;
                
                // Check if field is readonly (takes precedence over attribute parameter)
                var readOnly = fieldSymbol.IsReadOnly;
                
                var threadSafe = args.Length > 0 && (bool)args[0].Value!;
                var notify = args.Length > 1 && (bool)args[1].Value!;
                
                string[] alsoNotify = GetAlsoNotifyProperties(args);

                var getterEnum = args.Length > 3 ? args[3].Value : null;
                var setterEnum = args.Length > 4 ? args[4].Value : null;

                // Convert the numeric enum value to its string representation
                string getterValue = getterEnum != null ? GetAccessibilityName((int)getterEnum) : "Public";
                string setterValue = setterEnum != null ? GetAccessibilityName((int)setterEnum) : "Public";

                // For readonly properties, use getter accessibility as property accessibility
                // For read-write properties, use the widest accessibility
                string propertyAccessRaw = readOnly ? getterValue : GetWidestAccessibility(getterValue, setterValue);
                string propertyAccessibilityStr = ToPropertyAccessibilityString(propertyAccessRaw);
                
                var lockerArg = threadSafe ? "_Locker" : "null";
                var notifyArg = notify ? "true" : "false";
                
                if (readOnly)
                {
                    GenerateReadOnlyBindableProperty(
                        source, 
                        propertyAccessibilityStr, 
                        typeName, 
                        propertyName, 
                        getterValue, 
                        propertyAccessRaw, 
                        fieldName, 
                        lockerArg);
                }
                else
                {
                    GenerateReadWriteBindableProperty(
                        source, 
                        propertyAccessibilityStr, 
                        typeName, 
                        propertyName, 
                        getterValue, 
                        propertyAccessRaw, 
                        fieldName, 
                        lockerArg, 
                        notifyArg, 
                        alsoNotify, 
                        setterValue);
                }
            }
        }

        private static string[] GetAlsoNotifyProperties(ImmutableArray<TypedConstant> args)
        {
            if (args.Length <= 2)
                return Array.Empty<string>();
                
            var arg = args[2];
            if (arg.Kind == TypedConstantKind.Array && arg.Values != null)
            {
                return arg.Values
                    .Select(tc => tc.Value as string)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();
            }
            
            return Array.Empty<string>();
        }

        private static void GenerateReadOnlyBindableProperty(
            StringBuilder source,
            string propertyAccessibilityStr,
            string typeName,
            string propertyName,
            string getterValue,
            string propertyAccessRaw,
            string fieldName,
            string lockerArg)
        {
            string getterModifier = getterValue.Equals(propertyAccessRaw, StringComparison.OrdinalIgnoreCase)
                ? ""
                : ToPropertyAccessibilityString(getterValue);

            source.AppendLine($@"
    {propertyAccessibilityStr}{typeName} {propertyName}
    {{
        {getterModifier}get {{ return this.GetProperty(ref {fieldName}, {lockerArg}); }}
    }}");
        }

        private static void GenerateReadWriteBindableProperty(
            StringBuilder source,
            string propertyAccessibilityStr,
            string typeName,
            string propertyName,
            string getterValue,
            string propertyAccessRaw,
            string fieldName,
            string lockerArg,
            string notifyArg,
            string[] alsoNotify,
            string setterValue)
        {
            string getterModifier = getterValue.Equals(propertyAccessRaw, StringComparison.OrdinalIgnoreCase)
                ? ""
                : ToPropertyAccessibilityString(getterValue);

            string setterModifier = setterValue.Equals(propertyAccessRaw, StringComparison.OrdinalIgnoreCase)
                ? ""
                : ToPropertyAccessibilityString(setterValue);

            if (alsoNotify.Length > 0)
            {
                var notifyCalls = new StringBuilder();
                foreach (var prop in alsoNotify)
                {
                    if (!string.IsNullOrEmpty(prop))
                        notifyCalls.AppendLine($"                this.OnPropertyChanged(\"{prop}\");");
                }

                source.AppendLine($@"
    {propertyAccessibilityStr}{typeName} {propertyName}
    {{
        {getterModifier}get {{ return this.GetProperty(ref {fieldName}, {lockerArg}); }}
        {setterModifier}set
        {{
            if (this.SetProperty(ref {fieldName}, value, {lockerArg}, {notifyArg}))
            {{
{notifyCalls.ToString().TrimEnd()}
            }}
        }}
    }}");
            }
            else
            {
                source.AppendLine($@"
    {propertyAccessibilityStr}{typeName} {propertyName}
    {{
        {getterModifier}get {{ return this.GetProperty(ref {fieldName}, {lockerArg}); }}
        {setterModifier}set {{ this.SetProperty(ref {fieldName}, value, {lockerArg}, {notifyArg}); }}
    }}");
            }
        }

        private static void GenerateRegularProperties(
            StringBuilder source, 
            List<PropertyFieldInfo> propertyFields, 
            INamedTypeSymbol classSymbol,
            Compilation compilation)
        {
            foreach (var info in propertyFields)
            {
                var propertyName = PropertyGeneratorHelpers.ToPropertyName(info.FieldSymbol.Name);
                if (!PropertyGeneratorHelpers.IsPartial(classSymbol) ||
                    !PropertyGeneratorHelpers.IsValidFieldName(info.FieldSymbol.Name) ||
                    PropertyGeneratorHelpers.PropertyExists(classSymbol, propertyName))
                {
                    continue;
                }

                var fieldSymbol = info.FieldSymbol;
                var fieldName = fieldSymbol.Name;
                
                // Use NullableFlowState-aware display string to properly handle nullable types
                var typeName = GetNullableAwareTypeName(fieldSymbol.Type, compilation);

                var args = info.AttributeData.ConstructorArguments;
                
                // Check if field is readonly (takes precedence over attribute parameter)
                var readOnly = fieldSymbol.IsReadOnly;
                
                var threadSafe = args.Length > 0 && (bool)args[0].Value!;
                var getterEnum = args.Length > 1 ? args[1].Value : null;
                var setterEnum = args.Length > 2 ? args[2].Value : null;

                // Convert the numeric enum value to its string representation
                string getterValue = getterEnum != null ? GetAccessibilityName((int)getterEnum) : "Public";
                string setterValue = setterEnum != null ? GetAccessibilityName((int)setterEnum) : "Public";

                // For readonly properties, use getter accessibility as property accessibility
                // For read-write properties, use the widest accessibility
                string propertyAccessRaw = readOnly ? getterValue : GetWidestAccessibility(getterValue, setterValue);
                string propertyAccessibilityStr = ToPropertyAccessibilityString(propertyAccessRaw);
                
                var lockerArg = threadSafe ? "_Locker" : "null";
                
                if (readOnly)
                {
                    GenerateReadOnlyProperty(
                        source, 
                        propertyAccessibilityStr, 
                        typeName, 
                        propertyName, 
                        getterValue, 
                        propertyAccessRaw, 
                        fieldName, 
                        lockerArg);
                }
                else
                {
                    GenerateReadWriteProperty(
                        source, 
                        propertyAccessibilityStr, 
                        typeName, 
                        propertyName, 
                        getterValue, 
                        propertyAccessRaw, 
                        fieldName, 
                        lockerArg, 
                        setterValue);
                }
            }
        }

        private static void GenerateReadOnlyProperty(
            StringBuilder source,
            string propertyAccessibilityStr,
            string typeName,
            string propertyName,
            string getterValue,
            string propertyAccessRaw,
            string fieldName,
            string lockerArg)
        {
            string getterModifier = getterValue.Equals(propertyAccessRaw, StringComparison.OrdinalIgnoreCase)
                ? ""
                : ToPropertyAccessibilityString(getterValue);

            source.AppendLine($@"
    {propertyAccessibilityStr}{typeName} {propertyName}
    {{
        {getterModifier}get {{ return this.GetProperty(ref {fieldName}, {lockerArg}); }}
    }}");
        }

        private static void GenerateReadWriteProperty(
            StringBuilder source,
            string propertyAccessibilityStr,
            string typeName,
            string propertyName,
            string getterValue,
            string propertyAccessRaw,
            string fieldName,
            string lockerArg,
            string setterValue)
        {
            string getterModifier = getterValue.Equals(propertyAccessRaw, StringComparison.OrdinalIgnoreCase)
                ? ""
                : ToPropertyAccessibilityString(getterValue);

            string setterModifier = setterValue.Equals(propertyAccessRaw, StringComparison.OrdinalIgnoreCase)
                ? ""
                : ToPropertyAccessibilityString(setterValue);

            source.AppendLine($@"
    {propertyAccessibilityStr}{typeName} {propertyName}
    {{
        {getterModifier}get {{ return this.GetProperty(ref {fieldName}, {lockerArg}); }}
        {setterModifier}set {{ this.SetProperty(ref {fieldName}, value, {lockerArg}); }}
    }}");
        }

        private static string GetNullableAwareTypeName(ITypeSymbol typeSymbol, Compilation compilation)
        {
            // Create a SymbolDisplayFormat that includes nullable annotations
            var format = new SymbolDisplayFormat(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
                                     SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
                                     SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
            );

            return typeSymbol.ToDisplayString(format);
        }

        private static bool ImplementsInterface(INamedTypeSymbol type, string interfaceName)
        {
            return type.AllInterfaces.Any(i => i.ToDisplayString() == interfaceName);
        }

        private static string GetAccessibilityName(int value)
        {
            return AccessibilityNameMap.TryGetValue(value, out string name) ? name : "Public";
        }

        private static string ToPropertyAccessibilityString(string access)
        {
            return AccessibilityKeywordMap.TryGetValue(access, out string keyword) ? keyword : "public ";
        }

        private static string GetWidestAccessibility(string getter, string setter)
        {
            int getterRank = AccessibilityRankMap.TryGetValue(getter, out int gRank) ? gRank : 0;
            int setterRank = AccessibilityRankMap.TryGetValue(setter, out int sRank) ? sRank : 0;
            return getterRank >= setterRank ? getter : setter;
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