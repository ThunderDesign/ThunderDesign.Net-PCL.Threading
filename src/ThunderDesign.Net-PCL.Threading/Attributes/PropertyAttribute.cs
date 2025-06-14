using System;
using System.Collections.Generic;
using System.Text;

namespace ThunderDesign.Net_PCL.Threading.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class PropertyAttribute : Attribute
    {
        public bool ThreadSafe { get; }
        public bool ReadOnly { get; }

        public PropertyAttribute(bool threadSafe = true, bool readOnly = false)
        {
            ThreadSafe = threadSafe; 
            ReadOnly = readOnly;
        }
    }
}
