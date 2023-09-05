using System.Collections.Generic;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.DataCollections
{
    public class ObservableDataCollection<T> : ObservableCollectionThreadSafe<T>, IObservableDataCollection<T> where T : IBindableDataObject
    {
        #region constructors
        public ObservableDataCollection(bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base(waitOnNotifyPropertyChanged, waitOnNotifyCollectionChanged) { }

        public ObservableDataCollection(IEnumerable<T> collection, bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base(collection, waitOnNotifyPropertyChanged, waitOnNotifyCollectionChanged) { }

        public ObservableDataCollection(List<T> list, bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base(list, waitOnNotifyPropertyChanged, waitOnNotifyCollectionChanged) { }
        #endregion
    }
}
