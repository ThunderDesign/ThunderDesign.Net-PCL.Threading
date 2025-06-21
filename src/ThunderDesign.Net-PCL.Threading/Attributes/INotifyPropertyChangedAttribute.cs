using System;
using System.Collections.Generic;
using System.Text;

namespace ThunderDesign.Net_PCL.Threading.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class INotifyPropertyChangedAttribute : Attribute
    {
    }
}
