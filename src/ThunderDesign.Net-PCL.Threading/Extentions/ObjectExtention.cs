using System;
using System.Collections.Generic;
using System.Text;

namespace ThunderDesign.Net.Threading.Extentions
{
    public static class ObjectExtention
    {
        public static T GetProperty<T>(
            this object sender,
            ref T backingStore,
            object lockObj = null)
        {
            bool lockWasTaken = false;
            try
            {
                if (lockObj != null)
                    System.Threading.Monitor.Enter(lockObj, ref lockWasTaken);
                return backingStore;
            }
            finally
            {
                if (lockWasTaken)
                    System.Threading.Monitor.Exit(lockObj);
            }
        }

        public static bool SetProperty<T>(
            this object sender,
            ref T backingStore,
            T value,
            object lockObj = null)
        {
            bool lockWasTaken = false;
            try
            {
                if (lockObj != null)
                    System.Threading.Monitor.Enter(lockObj, ref lockWasTaken);
                if (EqualityComparer<T>.Default.Equals(backingStore, value))
                    return false;

                backingStore = value;
            }
            finally
            {
                if (lockWasTaken)
                    System.Threading.Monitor.Exit(lockObj);
            }
            return true;
        }
    }
}
