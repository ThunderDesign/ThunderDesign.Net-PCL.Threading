using SimpleBinding.ViewModels;
using SimpleBinding.Views;
using System;
using System.Collections.Generic;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net.Threading.Interfaces;
using Xamarin.Forms;

namespace SimpleBinding
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }
    }
}
