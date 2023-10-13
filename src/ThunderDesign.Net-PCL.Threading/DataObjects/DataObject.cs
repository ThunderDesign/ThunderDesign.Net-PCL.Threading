using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;
using ThunderDesign.Net.Threading.Objects;

namespace ThunderDesign.Net.Threading.DataObjects
{
    public class DataObject<Key> : ThreadObject, IDataObject<Key>
    {
        #region properties
        public Key Id
        {
            get { return GetId(); }
            set { SetId(value); }
        }

        object IDataObject.Id
        {
            get { return GetId(); }
            set { SetId((Key)value); }
        }
        #endregion

        #region methods
        protected virtual Key GetId()
        {
            return this.GetProperty(ref _idRef, _Locker);
        }

        protected virtual void SetId(Key value)
        {
            this.SetProperty(ref _idRef, value, _Locker);
        }
        #endregion

        #region variables
        protected Key _idRef = default;
        #endregion
    }
}
