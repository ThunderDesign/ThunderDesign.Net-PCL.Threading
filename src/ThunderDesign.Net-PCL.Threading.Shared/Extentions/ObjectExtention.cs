using System.Collections.Generic;
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
using ThunderDesign.Net_PCL.ToolBox.Helpers;
#endif

namespace ThunderDesign.Net.Threading.Extentions
{
    public static class ObjectExtention
    {
        public static T GetProperty<T>(
            this object sender,
            ref T backingStore,
            object? lockObj = null)
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
                    System.Threading.Monitor.Exit(lockObj!);
            }
        }

        public static bool SetProperty<T>(
            this object sender,
            ref T backingStore,
            T value,
            object? lockObj = null)
        {
            bool lockWasTaken = false;
            try
            {
                if (lockObj != null)
                    System.Threading.Monitor.Enter(lockObj, ref lockWasTaken);

#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
                if (EqualityHelper.Equality<T>(ref backingStore, value))
                    return false;
#else
                if (EqualityComparer<T>.Default.Equals(backingStore, value))
                    return false;
#endif
                backingStore = value;
            }
            finally
            {
                if (lockWasTaken)
                    System.Threading.Monitor.Exit(lockObj!);
            }

            return true;
        }
    }
}
