using System.Collections.Specialized;
using ThunderDesign.Net.Threading.HelperClasses;

namespace ThunderDesign.Net.Threading.Extentions
{
    public static class INotifyCollectionChangedExtension
    {
        public static void NotifyCollectionChanged(
            this INotifyCollectionChanged sender,
            NotifyCollectionChangedEventHandler handler,
            NotifyCollectionChangedEventArgs args,
            bool notifyAndWait = true)
        {
            // Calling 'Invoke' can cause DeadLocks and 'BeginInvoke' can cause System.PlatformNotSupportedException errors so calling Invoke from within a Thread
            //handler?.Invoke(sender, args);
            //handler?.BeginInvoke(sender, args, ar => { }, null);
            if (notifyAndWait)
                handler?.Invoke(sender, args);
            else
                ThreadHelper.RunAndForget(() => handler?.Invoke(sender, args));
        }
    }
}
