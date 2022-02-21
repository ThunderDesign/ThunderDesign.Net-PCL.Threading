using System;
using System.Collections.Generic;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Obsolete>")]
        [Obsolete("This is not supported in this class.", true)]
        private new void Add(TKey key, TValue value)
        {
            //Hiding base.Add(TKey key, TValue value);
        }

        public virtual void Add(TValue value)
        {
            base.Add(value.Id, value);
        }
        #endregion
    }
}
