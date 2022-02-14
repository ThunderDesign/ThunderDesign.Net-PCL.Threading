using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace ThunderDesign.Net.Threading.Extentions
{
    public static class INotifyCollectionChangedExtension
    {
        public static void NotifyCollectionChanged(
            this INotifyCollectionChanged sender,
            NotifyCollectionChangedEventHandler handler,
            NotifyCollectionChangedEventArgs args)
        {
            //handler?.BeginInvoke(sender, args, ar => { }, null);
            handler?.Invoke(sender, args);
        }
    }
}
