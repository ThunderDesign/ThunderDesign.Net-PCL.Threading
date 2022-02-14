using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IObservableCollectionThreadSafe : ICollection
    {
    }

    public interface IObservableCollectionThreadSafe<T> : ICollection<T>, IObservableCollectionThreadSafe
    {
        #region methods
        T GetItemByIndex(int index);
        #endregion
    }
}
