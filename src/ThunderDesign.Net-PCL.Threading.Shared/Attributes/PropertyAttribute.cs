using System;
using ThunderDesign.Net.Threading.Enums;

namespace ThunderDesign.Net.Threading.Attributes
{
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class PropertyAttribute : Attribute
    {
        public bool ThreadSafe { get; }
        public AccessorAccessibility Getter { get; }
        public AccessorAccessibility Setter { get; }

        public PropertyAttribute(
            bool threadSafe = true,
            AccessorAccessibility getter = AccessorAccessibility.Public,
            AccessorAccessibility setter = AccessorAccessibility.Public)
        {
            ThreadSafe = threadSafe;
            Getter = getter;
            Setter = setter;
        }
    }
#endif
}
