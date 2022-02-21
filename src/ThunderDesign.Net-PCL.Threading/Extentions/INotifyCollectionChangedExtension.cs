using System.Collections.Specialized;

namespace ThunderDesign.Net.Threading.Extentions
{
    public static class INotifyCollectionChangedExtension
    {
        public static void NotifyCollectionChanged(
            this INotifyCollectionChanged sender,
            NotifyCollectionChangedEventHandler handler,
            NotifyCollectionChangedEventArgs args)
        {
            //ThreadHelper.RunAndForget(() => handler?.Invoke(sender, args));
            //handler?.BeginInvoke(sender, args, ar => { }, null);
            handler?.Invoke(sender, args);
        }
    }
}
