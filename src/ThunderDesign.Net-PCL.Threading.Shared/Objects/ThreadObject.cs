namespace ThunderDesign.Net.Threading.Objects
{
    public abstract class ThreadObject
    {
        #region variables
        protected readonly object _Locker = new object();
        #endregion
    }
}
