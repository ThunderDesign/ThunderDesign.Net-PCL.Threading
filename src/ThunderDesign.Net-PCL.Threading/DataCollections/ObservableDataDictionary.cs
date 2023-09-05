using System;
using System.Collections.Generic;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.DataCollections
{
    public class ObservableDataDictionary<TKey, TValue> : ObservableDictionaryThreadSafe<TKey, TValue>, IObservableDataDictionary<TKey, TValue> where TValue : IBindableDataObject<TKey>
    {
        #region constructors
        public ObservableDataDictionary(bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base(waitOnNotifyPropertyChanged, waitOnNotifyCollectionChanged) { }

        public ObservableDataDictionary(int capacity, bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base(capacity, waitOnNotifyPropertyChanged, waitOnNotifyCollectionChanged) { }

        public ObservableDataDictionary(IEqualityComparer<TKey> comparer, bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base(comparer, waitOnNotifyPropertyChanged, waitOnNotifyCollectionChanged) { }

        public ObservableDataDictionary(IDictionary<TKey, TValue> dictionary, bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base(dictionary, waitOnNotifyPropertyChanged, waitOnNotifyCollectionChanged) { }

        public ObservableDataDictionary(int capacity, IEqualityComparer<TKey> comparer, bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base(capacity, comparer, waitOnNotifyPropertyChanged, waitOnNotifyCollectionChanged) { }

        public ObservableDataDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer, bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base(dictionary, comparer, waitOnNotifyPropertyChanged, waitOnNotifyCollectionChanged) { }
        #endregion

        #region methods
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Obsolete>")]
        [Obsolete("This is not supported in this class.", true)]
        private new void Add(TKey key, TValue value)
        {
            //Hiding base.Add(TKey key, TValue value);
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            this.Add(value);
        }

        public void Add(TValue value)
        {
            base.Add(value.Id, value);
        }
        #endregion
    }
}
