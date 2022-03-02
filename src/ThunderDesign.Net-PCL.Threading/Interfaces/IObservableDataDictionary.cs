namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IObservableDataDictionary : IObservableDictionaryThreadSafe
    {
    }

    public interface IObservableDataDictionary<TKey, TValue> : IObservableDictionaryThreadSafe<TKey, TValue>, IObservableDataDictionary
    {
        #region methods
        void Add(TValue value);
        #endregion
    }
}
