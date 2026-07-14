using System;
using System.Collections.Generic;
using System.Text;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IGatedThreadLock : ISingleThreadLock, IDisposable
    {
    }
}
