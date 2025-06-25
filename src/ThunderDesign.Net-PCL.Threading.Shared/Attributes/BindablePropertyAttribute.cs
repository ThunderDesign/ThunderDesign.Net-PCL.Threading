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
        public bool ReadOnly { get; private set; }
        public bool ThreadSafe { get; private set; }
        public bool Notify { get; private set; }
        public string[] AlsoNotify { get; private set; }
        public AccessorAccessibility Getter { get; private set; }
        public AccessorAccessibility Setter { get; private set; }

        public BindablePropertyAttribute(
            bool readOnly = false,
            bool threadSafe = true,
            bool notify = true,
            AccessorAccessibility getter = AccessorAccessibility.Public,
            AccessorAccessibility setter = AccessorAccessibility.Public)
        {
            ApplyBindablePropertyAttribute(Array.Empty<string>(), readOnly, threadSafe, notify, getter, setter);
        }

        public BindablePropertyAttribute(
            string alsoNotify,
            bool readOnly = false,
            bool threadSafe = true,
            bool notify = true,
            AccessorAccessibility getter = AccessorAccessibility.Public,
            AccessorAccessibility setter = AccessorAccessibility.Public)
        {
            ApplyBindablePropertyAttribute(string.IsNullOrEmpty(alsoNotify) ? Array.Empty<string>() : new string[1] { alsoNotify }, readOnly, threadSafe, notify, getter, setter);
        }

        public BindablePropertyAttribute(
            string[] alsoNotify,
            bool readOnly = false,
            bool threadSafe = true,
            bool notify = true,
            AccessorAccessibility getter = AccessorAccessibility.Public,
            AccessorAccessibility setter = AccessorAccessibility.Public)
        {
            ApplyBindablePropertyAttribute(alsoNotify ?? Array.Empty<string>(), readOnly, threadSafe, notify, getter, setter);
        }

        public BindablePropertyAttribute(
            IEnumerable<string> alsoNotify,
            bool readOnly = false,
            bool threadSafe = true,
            AccessorAccessibility getter = AccessorAccessibility.Public,
            AccessorAccessibility setter = AccessorAccessibility.Public,
            bool notify = true)
        {
            ApplyBindablePropertyAttribute(alsoNotify?.ToArray() ?? Array.Empty<string>(), readOnly, threadSafe, notify, getter, setter);
        }
        
        private void ApplyBindablePropertyAttribute(
            string[] alsoNotify,
            bool readOnly,
            bool threadSafe,
            bool notify,
            AccessorAccessibility getter,
            AccessorAccessibility setter)
        {
            ReadOnly = readOnly;
            ThreadSafe = threadSafe;
            Notify = notify;
            AlsoNotify = alsoNotify;
            Getter = getter;
            Setter = setter;
        }
    }
#endif
}