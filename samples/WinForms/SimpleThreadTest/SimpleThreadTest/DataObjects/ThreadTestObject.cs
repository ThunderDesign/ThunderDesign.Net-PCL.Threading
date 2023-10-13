using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.DataObjects;
using ThunderDesign.Net.Threading.Extentions;

namespace SimpleThreadTest.DataObjects
{
    public class ThreadTestObject : BindableDataObject<int>
    {
        #region properties
        public string Name
        {
            get { return this.GetProperty(ref _Name, _Locker); }
            set 
            {
                //if (this.SetProperty(ref _Name, value, _Locker, true))
                //    OnPropertyChanged(nameof(DisplayText));
                lock(_Locker)
                {
                    if (this.SetProperty(ref _Name, value, true))
                        OnPropertyChanged(nameof(DisplayText));

                }
            }
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
            private set
            {
                if (this.SetProperty(ref _Counter, value, _Locker))
                    OnPropertyChanged(nameof(DisplayText));
            }
        }

        public string DisplayText
        {
            get { return Name + " = " + Counter.ToString(); }
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
