using System;
using System.Collections.Generic;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.DataCollections
{
    public class ObservableDataDictionary<TKey, TValue> : ObservableDictionaryThreadSafe<TKey, TValue>, IObservableDataDictionary<TKey, TValue> where TValue : IBindableDataObject<TKey>
    {
        #region constructors
        public ObservableDataDictionary(bool waitOnNotifying = true) : base(waitOnNotifying) { }

        public ObservableDataDictionary(int capacity, bool waitOnNotifying = true) : base(capacity, waitOnNotifying) { }

        public ObservableDataDictionary(IEqualityComparer<TKey> comparer, bool waitOnNotifying = true) : base(comparer, waitOnNotifying) { }

        public ObservableDataDictionary(IDictionary<TKey, TValue> dictionary, bool waitOnNotifying = true) : base(dictionary, waitOnNotifying) { }

        public ObservableDataDictionary(int capacity, IEqualityComparer<TKey> comparer, bool waitOnNotifying = true) : base(capacity, comparer, waitOnNotifying) { }

        public ObservableDataDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer, bool waitOnNotifying = true) : base(dictionary, comparer, waitOnNotifying) { }
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
