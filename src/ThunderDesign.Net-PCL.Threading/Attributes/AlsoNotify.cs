using System;

namespace ThunderDesign.Net_PCL.Threading.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class AlsoNotifyAttribute : Attribute
    {
        public string PropertyName { get; }

        public AlsoNotifyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}