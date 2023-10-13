using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface ICollectionThreadSafe : IList
    {
    }

    public interface ICollectionThreadSafe<T> : IList<T>, IReadOnlyList<T>, ICollectionThreadSafe
    {
        #region properties
        new T this[int index] { get; set; }
        new int Count { get; }
        #endregion

        #region methods
        new void RemoveAt(int index);
        new void Clear();
        T GetItemByIndex(int index);
        #endregion
    }
}
