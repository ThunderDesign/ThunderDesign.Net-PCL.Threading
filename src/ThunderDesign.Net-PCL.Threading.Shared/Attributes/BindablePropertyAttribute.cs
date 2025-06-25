using System;
using System.Collections.Generic;
using System.Linq;
using ThunderDesign.Net.Threading.Enums;

namespace ThunderDesign.Net.Threading.Attributes
{
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
            string[] alsoNotify = null,
            AccessorAccessibility getter = AccessorAccessibility.Public,
            AccessorAccessibility setter = AccessorAccessibility.Public)
        {
            ApplyBindablePropertyAttribute(readOnly, threadSafe, notify, alsoNotify ?? new string[0], getter, setter);
        }

        public BindablePropertyAttribute(
            bool readOnly,
            bool threadSafe,
            bool notify,
            IEnumerable<string> alsoNotify,
            AccessorAccessibility getter = AccessorAccessibility.Public,
            AccessorAccessibility setter = AccessorAccessibility.Public)
        {
            ApplyBindablePropertyAttribute(readOnly, threadSafe, notify, alsoNotify?.ToArray() ?? new string[0], getter, setter);
        }
        
        public BindablePropertyAttribute(
            bool readOnly,
            bool threadSafe,
            bool notify,
            string alsoNotify,
            AccessorAccessibility getter = AccessorAccessibility.Public,
            AccessorAccessibility setter = AccessorAccessibility.Public)
        {
            ApplyBindablePropertyAttribute(readOnly, threadSafe, notify, string.IsNullOrEmpty(alsoNotify) ? new string[0] : new string[] { alsoNotify }, getter, setter);
        }

        public void ApplyBindablePropertyAttribute(
            bool readOnly,
            bool threadSafe,
            bool notify,
            string[] alsoNotify,
            AccessorAccessibility getter,
            AccessorAccessibility setter)
        {
            ReadOnly = readOnly;
            ThreadSafe = threadSafe;
            Notify = notify;
            AlsoNotify = alsoNotify ?? new string[0];
            Getter = getter;
            Setter = setter;
        }
    }
}