using System;
using System.Collections.Generic;
using System.Text;

namespace ThunderDesign.Net_PCL.Threading.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class PropertyAttribute : Attribute
    {
        public bool ReadOnly { get; }
        public bool ThreadSafe { get; }

        public PropertyAttribute(bool readOnly = false, bool threadSafe = true)
        {
            ReadOnly = readOnly;
            ThreadSafe = threadSafe; 
        }
    }
}
