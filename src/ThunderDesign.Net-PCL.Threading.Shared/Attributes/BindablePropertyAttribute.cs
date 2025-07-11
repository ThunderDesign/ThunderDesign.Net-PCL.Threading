using System;
using System.Collections.Generic;
using System.Linq;
using ThunderDesign.Net.Threading.Enums;

namespace ThunderDesign.Net.Threading.Attributes
{
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class BindablePropertyAttribute : Attribute
    {
        public bool ThreadSafe { get; }
        public bool Notify { get; }
        public string[] AlsoNotify { get; }
        public AccessorAccessibility Getter { get; }
        public AccessorAccessibility Setter { get; }

        public BindablePropertyAttribute(
            bool threadSafe = true,
            bool notify = true,
            string[] alsoNotify = null,
            AccessorAccessibility getter = AccessorAccessibility.Public,
            AccessorAccessibility setter = AccessorAccessibility.Public)
        {
            ThreadSafe = threadSafe;
            Notify = notify;
            AlsoNotify = alsoNotify ?? Array.Empty<string>();
            Getter = getter;
            Setter = setter;
        }
    }
#endif
}