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
        #region constructors
        public BindableObject(bool waitOnNotifyPropertyChanged = true) : base() 
        {
            _WaitOnNotifyPropertyChanged = waitOnNotifyPropertyChanged;
        }
        #endregion
        #region event handlers
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region properties
        public bool WaitOnNotifyPropertyChanged
        {
            get { return this.GetProperty(ref _WaitOnNotifyPropertyChanged, _Locker); }
            set { this.SetProperty(ref _WaitOnNotifyPropertyChanged, value, _Locker, true); }
        }
        #endregion

        #region methods
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.NotifyPropertyChanged(PropertyChanged, propertyName, WaitOnNotifyPropertyChanged);
        }
        #endregion

        #region variables
        protected bool _WaitOnNotifyPropertyChanged = true;
        #endregion
    }
}
