using System;
using System.Collections.Generic;
using System.Text;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.DataCollections
{
    public class ObservableDataDictionary<TKey, TValue> : ObservableDictionaryThreadSafe<TKey, TValue>, IObservableDataDictionary<TKey, TValue> where TValue : IBindableDataObject<TKey>
    {
        #region constructors
        public ObservableDataDictionary() : base() { }
        public ObservableDataDictionary(int capacity) : base(capacity) { }
        public ObservableDataDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }
        public ObservableDataDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
        public ObservableDataDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }
        public ObservableDataDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }
        #endregion

        #region methods
        public new void Add(TKey key, TValue value)
        {
            if (!EqualityComparer<TKey>.Default.Equals(value.Id, key))
                throw new ArgumentException("Key and Value.Id do not match");

            this.Add(value);
        }

        public void Add(TValue value)
        {
            base.Add(value.Id, value);
        }
        #endregion
    }
}
