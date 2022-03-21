using SimpleContacts.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Objects;

namespace SimpleContacts.ViewModels.Base
{
    public abstract class BaseViewModel : ThreadObject, IBaseViewModel
    {
        #region constructors
        public BaseViewModel() : base()
        {
        }
        #endregion

        #region event handlers
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region properties
        public bool IsBusy
        {
            get { return this.GetProperty(ref _IsBusy, _Locker); }
            set { this.SetProperty(ref _IsBusy, value, _Locker, PropertyChanged); }
        }
        #endregion

        #region methods
        public abstract Task<bool> LoadViewModelAsync(bool forceRefresh = false);

        public virtual void OnPropertyChanged(string propertyName)
        {
            this.NotifyPropertyChanged(PropertyChanged, propertyName);
        }
        #endregion

        #region variables
        protected bool _IsBusy = false;
        #endregion
    }
}
