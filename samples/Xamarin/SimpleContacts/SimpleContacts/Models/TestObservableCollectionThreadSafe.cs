using SimpleContacts.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using ThunderDesign.Net.Threading.Collections;

namespace SimpleContacts.Models
{
    public class TestObservableCollectionThreadSafe<T> : ObservableCollectionThreadSafe<T>, ITestObservableCollectionThreadSafe<T>
    {
    }
}
