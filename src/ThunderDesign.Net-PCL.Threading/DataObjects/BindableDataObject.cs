﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.DataObjects
{
    public class BindableDataObject<Key> : DataObject<Key>, IBindableDataObject<Key>
    {
        #region event handlers
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region properties
        protected virtual bool WaitWhenNotifying => false;
        #endregion

        #region methods
        protected override void SetId(Key value)
        {
            this.SetProperty(ref _Id, value, _Locker, true, nameof(Id));
        }

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.NotifyPropertyChanged(PropertyChanged, propertyName, WaitWhenNotifying);
        }
        #endregion
    }
}
