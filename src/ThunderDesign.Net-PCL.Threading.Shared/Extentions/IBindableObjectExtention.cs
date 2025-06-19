using System.Runtime.CompilerServices;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.Extentions
{
    public static class IBindableObjectExtention
    {
        public static bool SetProperty<T>(
            this IBindableObject sender,
            ref T backingStore,
            T value,
            bool notifyPropertyChanged,
            [CallerMemberName] string propertyName = "")
        {
            return sender.SetProperty(ref backingStore, value, null, notifyPropertyChanged, propertyName);
        }

        public static bool SetProperty<T>(
            this IBindableObject sender,
            ref T backingStore,
            T value,
            object lockObj,
            bool notifyPropertyChanged,
            [CallerMemberName] string propertyName = "")
        {
            if (sender.SetProperty(ref backingStore, value, lockObj))
            {
                if (notifyPropertyChanged)
                    sender.OnPropertyChanged(propertyName);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
