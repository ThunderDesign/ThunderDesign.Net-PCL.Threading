using System;
using System.Collections.Generic;
using System.Text;
using ThunderDesign.Net.Threading.Interfaces;

namespace SimpleContacts.Interfaces
{
    //public interface ITestObservableCollectionThreadSafe : IObservableCollectionThreadSafe
    //{
    //}

    public interface ITestObservableCollectionThreadSafe<T> : IObservableCollectionThreadSafe<T>
    {
    }
}
