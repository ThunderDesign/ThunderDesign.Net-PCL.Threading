using System.Collections.Generic;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.DataCollections
{
    public class ObservableDataCollection<T> : ObservableCollectionThreadSafe<T>, IObservableDataCollection<T> where T : IBindableDataObject
    {
        #region constructors
        public ObservableDataCollection() : base() { }
        public ObservableDataCollection(IEnumerable<T> collection) : base(collection) { }
        public ObservableDataCollection(List<T> list) : base(list) { }
        #endregion
    }
}
