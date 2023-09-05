using System.Collections.Specialized;
using System.ComponentModel;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IObservableDictionaryThreadSafe : IDictionaryThreadSafe, IBindableCollection
    {
    }

    public interface IObservableDictionaryThreadSafe<TKey, TValue> : IDictionaryThreadSafe<TKey, TValue>, IObservableDictionaryThreadSafe
    {
    }
}
