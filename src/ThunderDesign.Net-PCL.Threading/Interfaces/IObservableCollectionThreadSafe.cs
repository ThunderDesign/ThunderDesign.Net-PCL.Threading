using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IObservableCollectionThreadSafe : ICollectionThreadSafe, IBindableCollection
    {
        #region methods
        void Move(int oldIndex, int newIndex);
        #endregion
    }

    public interface IObservableCollectionThreadSafe<T> : ICollectionThreadSafe<T>, IObservableCollectionThreadSafe
    {
    }
}
