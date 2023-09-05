using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThunderDesign.Net.Threading.HelperClasses;

namespace ThunderDesign.Net.Threading.Extentions
{
    public delegate void InvokePropertyChangedDelegate(INotifyPropertyChanged sender, PropertyChangedEventArgs args);
    public static class INotifyPropertyChangedExtension
    {
        public static void NotifyPropertyChanged(
            this INotifyPropertyChanged sender,
            PropertyChangedEventHandler handler,
            [CallerMemberName] string propertyName = "",
            bool notifyAndWait = true)
        {
            sender.NotifyPropertyChanged(handler, new PropertyChangedEventArgs(propertyName), notifyAndWait);
        }

        public static void NotifyPropertyChanged(
            this INotifyPropertyChanged sender,
            PropertyChangedEventHandler handler,
            PropertyChangedEventArgs args,
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

        public static bool SetProperty<T>(
            this INotifyPropertyChanged sender,
            ref T backingStore,
            T value,
            PropertyChangedEventHandler propertyChangedEventHandler,
            [CallerMemberName] string propertyName = "",
            bool notifyAndWait = true)
        {
            return sender.SetProperty(ref backingStore, value, null, propertyChangedEventHandler, propertyName, notifyAndWait);
        }

        public static bool SetProperty<T>(
            this INotifyPropertyChanged sender,
            ref T backingStore,
            T value,
            object lockObj,
            PropertyChangedEventHandler propertyChangedEventHandler,
            [CallerMemberName] string propertyName = "",
            bool notifyAndWait = true)
        {

            if (sender.SetProperty(ref backingStore, value, lockObj))
            {
                sender.NotifyPropertyChanged(propertyChangedEventHandler, propertyName, notifyAndWait);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
