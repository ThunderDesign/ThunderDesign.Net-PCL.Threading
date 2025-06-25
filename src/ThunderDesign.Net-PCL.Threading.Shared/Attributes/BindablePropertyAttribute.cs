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
            AccessorAccessibility getter = AccessorAccessibility.Public,
            AccessorAccessibility setter = AccessorAccessibility.Public)
        {
            ApplyBindablePropertyAttribute(readOnly, threadSafe, notify, new string[0], getter, setter);
        }

        public BindablePropertyAttribute(
            bool readOnly = false,
            bool threadSafe = true,
            bool notify = true,
            string alsoNotify = null,
            AccessorAccessibility getter = AccessorAccessibility.Public,
            AccessorAccessibility setter = AccessorAccessibility.Public)
        {
            ApplyBindablePropertyAttribute(readOnly, threadSafe, notify, string.IsNullOrEmpty(alsoNotify) ? new string[0] : new string[] { alsoNotify }, getter, setter);
        }

        public BindablePropertyAttribute(
            string[] alsoNotify = null,
            bool readOnly = false,
            bool threadSafe = true,
            bool notify = true,
            AccessorAccessibility getter = AccessorAccessibility.Public,
            AccessorAccessibility setter = AccessorAccessibility.Public)
        {
            ApplyBindablePropertyAttribute(readOnly, threadSafe, notify, alsoNotify ?? new string[0], getter, setter);
        }

        public BindablePropertyAttribute(
            bool readOnly = false,
            bool threadSafe = true,
            IEnumerable<string> alsoNotify = null,
            AccessorAccessibility getter = AccessorAccessibility.Public,
            AccessorAccessibility setter = AccessorAccessibility.Public,
            bool notify = true)
        {
            ApplyBindablePropertyAttribute(readOnly, threadSafe, notify, alsoNotify?.ToArray() ?? new string[0], getter, setter);
        }
        
        public void ApplyBindablePropertyAttribute(
            bool readOnly = false,
            bool threadSafe = true,
            bool notify = true,
            string[] alsoNotify = null,
            AccessorAccessibility getter = AccessorAccessibility.Public,
            AccessorAccessibility setter = AccessorAccessibility.Public)
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