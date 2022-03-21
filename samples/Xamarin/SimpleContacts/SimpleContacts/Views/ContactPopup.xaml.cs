using SimpleContacts.ViewModels;
using SimpleContacts.Views.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SimpleContacts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactPopup : BasePopup<ContactViewModel>
    {
        #region constructors
        public ContactPopup(ushort id) : base()
        {
            ((ContactViewModel)ViewModel).LookupId = id;
            InitializeComponent();
        }
        #endregion

        #region properties
        public IAsyncCommand SaveCommandAsync { get; private set; }
        #endregion

        #region methods
        private void OnSaveClicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(EntryFirstName.Text))
            {
                ButtonSave.Focus();
                EntryFirstName.Focus();
                return;
            }
            else if (String.IsNullOrEmpty(EntryLastName.Text))
            {
                ButtonSave.Focus();
                EntryLastName.Focus();
                return;
            }
            else
            {
                if (ViewModel != null && ViewModel is ContactViewModel viewModel)
                {
                    if (Task.Run<bool>(async () => await viewModel.SaveViewModelAsync().ConfigureAwait(false)).Result)
                        this.Dismiss(null);
                }
            }
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            this.Dismiss(null);
        }
        #endregion
    }
}