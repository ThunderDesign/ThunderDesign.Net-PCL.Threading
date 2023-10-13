using System;
using System.Collections.Generic;
using System.Text;
using ThunderDesign.Net.Threading.DataObjects;
using ThunderDesign.Net.Threading.Extentions;

namespace SimpleBinding.Models
{
    public class ThreadTestModel : BindableDataObject<int>
    {
        #region properties
        public string Name
        {
            get { return this.GetProperty(ref _Name, _Locker); }
            set { this.SetProperty(ref _Name, value, _Locker, true); }
        }

        public int IncCounterWaitTimeSeconds
        {
            get { return this.GetProperty(ref _IncCounterWaitTimeSeconds, _Locker); }
            set { this.SetProperty(ref _IncCounterWaitTimeSeconds, value, _Locker, true); }
        }

        public int DecCounterWaitTimeSeconds
        {
            get { return this.GetProperty(ref _DecCounterWaitTimeSeconds, _Locker); }
            set { this.SetProperty(ref _DecCounterWaitTimeSeconds, value, _Locker, true); }
        }

        public long Counter
        {
            get { return this.GetProperty(ref _Counter, _Locker); }
            private set { this.SetProperty(ref _Counter, value, _Locker); }
        }
        #endregion

        #region variables
        protected string _Name = String.Empty;
        protected int _IncCounterWaitTimeSeconds = 1;
        protected int _DecCounterWaitTimeSeconds = 1;
        protected long _Counter = 0;
        #endregion
    }
}
