using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.Objects
{
    public class BindableObject : ThreadObject, IBindableObject
    {
        #region event handlers
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region properties
        protected virtual bool WaitWhenNotifying => false;
        #endregion

        #region methods
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.NotifyPropertyChanged(PropertyChanged, propertyName, WaitWhenNotifying);
        }
        #endregion
    }
}
