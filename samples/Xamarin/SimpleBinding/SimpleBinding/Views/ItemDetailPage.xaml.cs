using SimpleBinding.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace SimpleBinding.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}