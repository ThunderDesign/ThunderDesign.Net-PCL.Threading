namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IObservableDataDictionary : IObservableDictionaryThreadSafe
    {
    }

    public interface IObservableDataDictionary<TKey, TValue> : IObservableDictionaryThreadSafe<TKey, TValue>, IObservableDataDictionary
    {
        #region methods
        new void Add(TKey key, TValue value);
        void Add(TValue value);
        #endregion
    }
}
