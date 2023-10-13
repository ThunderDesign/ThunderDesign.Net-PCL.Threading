using System.Collections.Generic;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.DataCollections
{
    public class ObservableDataCollection<T> : ObservableCollectionThreadSafe<T>, IObservableDataCollection<T> where T : IBindableDataObject
    {
        #region constructors
        public ObservableDataCollection(bool waitOnNotifying = true) : base(waitOnNotifying) { }

        public ObservableDataCollection(IEnumerable<T> collection, bool waitOnNotifying = true) : base(collection, waitOnNotifying) { }

        public ObservableDataCollection(List<T> list, bool waitOnNotifying = true) : base(list, waitOnNotifying) { }
        #endregion
    }
}
