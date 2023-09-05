using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface ICollectionThreadSafe : IList, ICollection, IEnumerable
    {
    }

    public interface ICollectionThreadSafe<T> : IList<T>, ICollection<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, ICollectionThreadSafe
    {
        #region methods
        T GetItemByIndex(int index);
        #endregion
    }
}
