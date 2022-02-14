using System;
using System.Collections.Generic;
using System.Text;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IBindableDataObject : IDataObject, IBindableObject
    {
    }

    public interface IBindableDataObject<Key> : IDataObject<Key>, IBindableDataObject
    {
    }
}
