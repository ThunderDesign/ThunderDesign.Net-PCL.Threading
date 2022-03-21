using SimpleContacts.Models;
using SimpleContacts.ViewModels;
using SimpleContacts.Views.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SimpleContacts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactsView : BaseView<ContactsViewModel>
    {
        #region constructors
        public ContactsView() : base()
        {
            CreateViewCommandAsync = new AsyncCommand(() => OnCreateViewCommandAsync(), allowsMultipleExecutions: false);
            ReviseViewCommandAsync = new AsyncCommand<ContactsModel>(model => OnReviseViewCommandAsync(model), allowsMultipleExecutions: false);
            DeleteViewCommandAsync = new AsyncCommand<ContactsModel>(model => OnDeleteViewCommandAsync(model), allowsMultipleExecutions: false);
            InitializeComponent();
        }
        #endregion

        #region properties
        public IAsyncCommand CreateViewCommandAsync { get; protected set; }
        public IAsyncCommand<ContactsModel> ReviseViewCommandAsync { get; protected set; }
        public IAsyncCommand<ContactsModel> DeleteViewCommandAsync { get; protected set; }
        #endregion

        #region methods
        private async Task OnCreateViewCommandAsync()
        {
            ContactPopup popup = new ContactPopup(0);
            await Shell.Current.Navigation.ShowPopupAsync(popup);
        }

        protected async Task OnReviseViewCommandAsync(ContactsModel model)
        {
            ContactPopup popup = new ContactPopup(model.Id);
            await Shell.Current.Navigation.ShowPopupAsync(popup);
        }

        protected async Task OnDeleteViewCommandAsync(ContactsModel model)
        {
            if (await DisplayAlert("Question?", $"Delet '{model.FullName}'?", "Yes", "No"))
                if (ViewModel != null && ViewModel is ContactsViewModel viewModel)
                    await viewModel.DeleteViewModelAsync(model.Id);
        }
        #endregion
    }
}