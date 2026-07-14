using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;
using ThunderDesign.Net.Threading.Objects;

namespace ThunderDesign.Net.Threading.DataObjects
{
    public class DataObject<TKey> : ThreadObject, IDataObject<TKey> where TKey : notnull
    {
        #region properties
        public TKey Id
        {
            get { return GetId(); }
            set { SetId(value); }
        }

        object IDataObject.Id
        {
            get { return GetId(); }
            set { SetId((TKey)value); }
        }
        #endregion

        #region methods
        protected virtual TKey GetId()
        {
            return this.GetProperty(ref _idRef, _Locker);
        }

        protected virtual void SetId(TKey value)
        {
            this.SetProperty(ref _idRef, value, _Locker);
        }
        #endregion

        #region variables
        protected TKey _idRef = default!;
        #endregion
    }
}
