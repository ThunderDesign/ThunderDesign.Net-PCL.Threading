using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IObservableCollectionThreadSafe : IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
    }

    public interface IObservableCollectionThreadSafe<T> : IList<T>, IObservableCollectionThreadSafe
    {
        #region methods
        T GetItemByIndex(int index);
        #endregion
    }
}
