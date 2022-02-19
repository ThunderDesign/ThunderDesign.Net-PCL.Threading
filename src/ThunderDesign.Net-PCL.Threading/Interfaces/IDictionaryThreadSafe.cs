using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IDictionaryThreadSafe : IDictionary
    {
    }

    public interface IDictionaryThreadSafe<TKey, TValue> : IDictionary<TKey, TValue>, IDictionaryThreadSafe
    {
    }
}
