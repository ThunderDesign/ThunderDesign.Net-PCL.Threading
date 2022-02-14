using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IBindableObject : INotifyPropertyChanged
    {
        #region methods
        void OnPropertyChanged([CallerMemberName] string propertyName = "");
        #endregion
    }
}
