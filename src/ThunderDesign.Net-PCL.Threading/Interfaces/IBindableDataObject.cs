namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IBindableDataObject : IDataObject, IBindableObject
    {
    }

    public interface IBindableDataObject<TKey> : IDataObject<TKey>, IBindableDataObject where TKey : notnull
    {
    }
}
