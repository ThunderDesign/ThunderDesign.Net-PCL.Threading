using System;

namespace ThunderDesign.Net_PCL.Threading.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class BindablePropertyAttribute : Attribute
    {
        public bool ReadOnly { get; }
        public bool ThreadSafe { get; }
        public bool Notify { get; }
        public string[] AlsoNotify { get; }

        public BindablePropertyAttribute(
            bool readOnly = false,
            bool threadSafe = true,
            bool notify = true,
            string[] alsoNotify = null)
        {
            ReadOnly = readOnly;
            ThreadSafe = threadSafe;
            Notify = notify;
            AlsoNotify = alsoNotify ?? new string[0];
        }
    }
}