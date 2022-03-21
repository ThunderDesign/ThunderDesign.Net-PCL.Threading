using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using SimpleContacts.Models;
using SimpleContacts.Services;
using SimpleContacts.ViewModels.Base;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.HelperClasses;

namespace SimpleContacts.ViewModels
{
    public class ContactsViewModel : BaseViewModel
    {
        #region constructors
        public ContactsViewModel() : base()
        {
            ThreadHelper.RunAndForget(async () => await LoadViewModelAsync(false).ConfigureAwait(false));
        }
        #endregion

        #region properties
        public ContactsModelList ViewModelData
        {
            get { return this.GetProperty(ref _ViewModelData, _Locker); }
            set { this.SetProperty(ref _ViewModelData, value, _Locker, true); }
        }
        #endregion

        #region methods
        public override async Task<bool> LoadViewModelAsync(bool forceRefresh = false)
        {
            // Don't reload if we're already doing so...
            if (this.IsBusy)
            {
                return false;
            }

            try
            {
                // Show the "reload"-spinner and disable the reload-command (if needed).
                this.IsBusy = true;

                ViewModelData = await ContactsService.GetContactsAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                this.IsBusy = false;
            }
            return false;
        }

        public async Task<bool> DeleteViewModelAsync(ushort Id)
        {
            if (this.IsBusy)
            {
                return false;
            }

            try
            {
                return await ContactsService.DeleteContactAsync(Id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                this.IsBusy = false;
            }
            return false;
        }
        #endregion

        #region variables
        protected ContactsModelList _ViewModelData;
        #endregion
    }
}
