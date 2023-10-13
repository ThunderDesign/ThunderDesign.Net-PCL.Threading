using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.DataObjects
{
    public class BindableDataObject<Key> : DataObject<Key>, IBindableDataObject<Key>
    {
        #region constructors
        public BindableDataObject(bool waitOnNotifying = true) : base()
        {
            _waitOnNotifyingRef = waitOnNotifying;
        }
        #endregion

        #region event handlers
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region properties
        public bool WaitOnNotifying
        {
            get { return this.GetProperty(ref _waitOnNotifyingRef, _Locker); }
            set { this.SetProperty(ref _waitOnNotifyingRef, value, _Locker, true); }
        }
        #endregion

        #region methods
        protected override void SetId(Key value)
        {
            this.SetProperty(ref _idRef, value, _Locker, true, nameof(Id));
        }

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.NotifyPropertyChanged(PropertyChanged, propertyName, WaitOnNotifying);
        }
        #endregion

        #region variables
        protected bool _waitOnNotifyingRef = true;
        #endregion
    }
}
