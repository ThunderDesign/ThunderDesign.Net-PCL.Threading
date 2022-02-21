using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ThunderDesign.Net.Threading.Extentions
{
    public delegate void InvokePropertyChangedDelegate(INotifyPropertyChanged sender, PropertyChangedEventArgs args);
    public static class INotifyPropertyChangedExtension
    {
        public static void NotifyPropertyChanged(
            this INotifyPropertyChanged sender,
            PropertyChangedEventHandler handler,
            [CallerMemberName] string propertyName = "")
        {
            sender.NotifyPropertyChanged(handler, new PropertyChangedEventArgs(propertyName));
        }

        public static void NotifyPropertyChanged(
            this INotifyPropertyChanged sender,
            PropertyChangedEventHandler handler,
            PropertyChangedEventArgs args)
        {
            //ThreadHelper.RunAndForget(() => handler?.Invoke(sender, args));
            //handler?.BeginInvoke(sender, args, ar => { }, null);
            handler?.Invoke(sender, args);
        }

        public static bool SetProperty<T>(
            this INotifyPropertyChanged sender,
            ref T backingStore,
            T value,
            PropertyChangedEventHandler propertyChangedEventHandler,
            [CallerMemberName] string propertyName = "")
        {
            return sender.SetProperty(ref backingStore, value, null, propertyChangedEventHandler, propertyName);
        }

        public static bool SetProperty<T>(
            this INotifyPropertyChanged sender,
            ref T backingStore,
            T value,
            object lockObj,
            PropertyChangedEventHandler propertyChangedEventHandler,
            [CallerMemberName] string propertyName = "")
        {

            if (sender.SetProperty(ref backingStore, value, lockObj))
            {
                sender.NotifyPropertyChanged(propertyChangedEventHandler, propertyName);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
