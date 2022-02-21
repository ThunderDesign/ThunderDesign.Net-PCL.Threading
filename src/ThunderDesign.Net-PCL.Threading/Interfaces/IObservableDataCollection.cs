namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IObservableDataCollection : IObservableCollectionThreadSafe
    {
    }

    public interface IObservableDataCollection<T> : IObservableCollectionThreadSafe<T>, IObservableDataCollection
    {
    }
}
