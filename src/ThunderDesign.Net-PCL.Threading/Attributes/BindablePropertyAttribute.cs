using System;

namespace ThunderDesign.Net_PCL.Threading.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class BindablePropertyAttribute : Attribute
    {
        public bool ThreadSafe { get; }
        public bool Notify { get; }
        public bool ReadOnly { get; }

        public BindablePropertyAttribute(bool threadSafe = true, bool notify = true, bool readOnly = false)
        {
            ThreadSafe = threadSafe;
            Notify = notify;
            ReadOnly = readOnly;
        }
    }
}