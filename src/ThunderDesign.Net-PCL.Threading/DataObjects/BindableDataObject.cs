using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.DataObjects
{
    public class BindableDataObject<Key> : DataObject<Key>, IBindableDataObject<Key>
    {
        #region event handlers
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region methods
        protected override void SetId(Key value)
        {
            this.SetProperty(ref _Id, value, _Locker, true, nameof(Id));
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            this.NotifyPropertyChanged(PropertyChanged, propertyName);
        }
        #endregion
    }
}
