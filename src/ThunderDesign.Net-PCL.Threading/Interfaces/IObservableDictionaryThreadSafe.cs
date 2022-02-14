using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IObservableDictionaryThreadSafe : IDictionary, INotifyCollectionChanged, INotifyPropertyChanged
    {
    }

    public interface IObservableDictionaryThreadSafe<TKey, TValue> : IDictionary<TKey, TValue>, IObservableDictionaryThreadSafe
    {
    }
}
