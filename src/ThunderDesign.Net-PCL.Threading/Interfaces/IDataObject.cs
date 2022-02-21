namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IDataObject
    {
        #region properties
        object Id { get; set; }
        #endregion
    }

    public interface IDataObject<Key> : IDataObject
    {
        #region properties
        new Key Id { get; set; }
        #endregion
    }
}
