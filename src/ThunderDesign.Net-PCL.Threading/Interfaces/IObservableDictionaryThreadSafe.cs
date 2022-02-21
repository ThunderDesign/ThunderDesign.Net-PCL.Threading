using System.Collections.Specialized;
using System.ComponentModel;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IObservableDictionaryThreadSafe : IDictionaryThreadSafe, INotifyCollectionChanged, INotifyPropertyChanged
    {
    }

    public interface IObservableDictionaryThreadSafe<TKey, TValue> : IDictionaryThreadSafe<TKey, TValue>, IObservableDictionaryThreadSafe
    {
    }
}
