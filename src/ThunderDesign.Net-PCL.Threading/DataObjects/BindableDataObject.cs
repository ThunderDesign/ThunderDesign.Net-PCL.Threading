using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.DataObjects
{
    public class BindableDataObject<Key> : DataObject<Key>, IBindableDataObject<Key>
    {
        #region constructors
        public BindableDataObject(bool waitOnNotifyPropertyChanged = true) : base()
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
        protected override void SetId(Key value)
        {
            this.SetProperty(ref _Id, value, _Locker, true, nameof(Id));
        }

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
