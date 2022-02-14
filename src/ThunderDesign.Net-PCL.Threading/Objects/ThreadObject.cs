using System;
using System.Collections.Generic;
using System.Text;

namespace ThunderDesign.Net.Threading.Objects
{
    public class ThreadObject
    {
        #region variables
        protected readonly static object _Locker = new object();
        #endregion
    }
}
