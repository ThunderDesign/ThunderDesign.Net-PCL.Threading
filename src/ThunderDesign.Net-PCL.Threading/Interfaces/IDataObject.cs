namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IDataObject
    {
        #region properties
        object Id { get; set; }
        #endregion
    }

    public interface IDataObject<TKey> : IDataObject where TKey : notnull
    {
        #region properties
        new TKey Id { get; set; }
        #endregion
    }
}
