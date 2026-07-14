using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Helpers
{
    /// <summary>
    /// Reflection-based helper used to assert that a thread-safe wrapper class
    /// exposes a public member for every public instance member declared directly
    /// on the wrapped BCL base type (for the currently targeted framework).
    /// </summary>
    public static class ApiParityHelper
    {
        public static void AssertWrapperCoversBaseApi(
            Type wrapperType,
            Type baseType,
            IEnumerable<string>? knownExclusions = null)
        {
            string[] globalExclusions = ["GetType()", "ToString()", "Equals(Object)", "GetHashCode()"];
    
            if (wrapperType is null) throw new ArgumentNullException(nameof(wrapperType));
            if (baseType is null) throw new ArgumentNullException(nameof(baseType));

            var exclusions = new HashSet<string>(knownExclusions ?? Array.Empty<string>(), StringComparer.Ordinal);
            exclusions.UnionWith(globalExclusions);

            var baseMembers = GetAllPublicInstanceMembers(baseType);
            var wrapperMembers = GetDeclaredPublicInstanceMembers(wrapperType);

            var missing = new List<string>();

            foreach (var baseMember in baseMembers)
            {
                var signature = DescribeMember(baseMember);

                if (exclusions.Contains(baseMember.Name) || exclusions.Contains(signature))
                    continue;

                if (!HasMatchingMember(wrapperMembers, baseMember))
                {
                    missing.Add(signature);
                }
                //else
                //{
                //    // Optionally, you could add a check to ensure that the wrapper member has the same return type as the base member.
                //    // This is not strictly necessary for API parity, but it can help catch subtle issues.
                //    var wrapperMember = wrapperMembers.FirstOrDefault(wm => wm.Name == baseMember.Name);
                //    if (wrapperMember != null && wrapperMember is MethodInfo wrapperMethod && baseMember is MethodInfo baseMethod)
                //    {
                //        if (wrapperMethod.ReturnType != baseMethod.ReturnType)
                //        {
                //            missing.Add($"{signature} (return type mismatch: expected {baseMethod.ReturnType.Name}, found {wrapperMethod.ReturnType.Name})");
                //        }
                //    }
                //}
            }

            if (missing.Count > 0)
            {
                Assert.Fail(
                    $"'{wrapperType.Name}' is missing wrapper members for the following public API declared on '{baseType.Name}' " +
                    $"(target framework: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}):{Environment.NewLine}" +
                    string.Join(Environment.NewLine, missing.Select(m => " - " + m)));
            }
        }

        private static List<MemberInfo> GetDeclaredPublicInstanceMembers(Type baseType)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            var methods = baseType.GetMethods(flags)
                .Where(m => !m.IsSpecialName) // exclude property accessors, operators, event add/remove
                .Cast<MemberInfo>();

            var properties = baseType.GetProperties(flags)
                .Cast<MemberInfo>();

            return methods.Concat(properties).ToList();
        }

        private static List<MemberInfo> GetAllPublicInstanceMembers(Type wrapperType)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;

            var methods = wrapperType.GetMethods(flags)
                .Where(m => !m.IsSpecialName)
                .Cast<MemberInfo>();

            var properties = wrapperType.GetProperties(flags)
                .Cast<MemberInfo>();

            return methods.Concat(properties).ToList();
        }

        private static bool HasMatchingMember(List<MemberInfo> wrapperMembers, MemberInfo baseMember)
        {
            if (baseMember is MethodInfo baseMethod)
            {
                var baseParams = baseMethod.GetParameters().Select(p => p.ParameterType.Name).ToArray();

                return wrapperMembers.OfType<MethodInfo>().Any(wm =>
                    wm.Name == baseMethod.Name &&
                    wm.GetParameters().Select(p => p.ParameterType.Name).SequenceEqual(baseParams));
            }

            if (baseMember is PropertyInfo baseProperty)
            {
                return wrapperMembers.OfType<PropertyInfo>().Any(wp => wp.Name == baseProperty.Name && wp.DeclaringType != baseProperty.DeclaringType);
            }

            return false;
        }

        private static string DescribeMember(MemberInfo member)
        {
            if (member is MethodInfo method)
            {
                var parameters = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name));
                return $"{method.Name}({parameters})";
            }

            if (member is PropertyInfo property)
            {
                return $"{property.PropertyType.Name} {property.Name} {{ get; }}";
            }

            return member.Name;
        }
    }
}
