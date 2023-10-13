using SimpleContacts.Interfaces;
using SimpleContacts.Models;
using SimpleContacts.Services;
using SimpleContacts.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.HelperClasses;

namespace SimpleContacts.ViewModels
{
    public class ContactViewModel : BaseViewModel
    {
        #region constructors
        public ContactViewModel() : base()
        {
            ThreadHelper.RunAndForget(async () => await LoadViewModelAsync(true).ConfigureAwait(false));
        }
        #endregion

        #region properties
        public ContactsModel ViewModelData
        {
            get { return this.GetProperty(ref _ViewModelData, _Locker); }
            set { this.SetProperty(ref _ViewModelData, value, _Locker, true); }
        }

        public ushort? LookupId
        {
            get { return this.GetProperty(ref _LookupId, _Locker); }
            set
            {
                lock (_WaitLocker)
                {
                    if (this.SetProperty(ref _LookupId, value, _Locker, true))
                        Monitor.PulseAll(_WaitLocker);
                }
            }
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
                ITestObservableCollectionThreadSafe<string> ts = new TestObservableCollectionThreadSafe<string>();
                if (ts.Count > 0)
                ts.Clear();
                // Show the "reload"-spinner and disable the reload-command (if needed).
                this.IsBusy = true;

                WaitFor_LookupId();

                ViewModelData = await ContactsService.GetContactAsync(LookupId ?? 0);
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

        public async Task<bool> SaveViewModelAsync()
        {
            if (this.IsBusy)
            {
                return false;
            }

            try
            {
                return await ContactsService.SaveContactAsync(ViewModelData).ConfigureAwait(false);
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

        protected void WaitFor_LookupId()
        {
            lock (_WaitLocker)
                while (!this.LookupId.HasValue)
                    Monitor.Wait(_WaitLocker);
        }
        #endregion

        #region variables
        private static readonly object _WaitLocker = new object();
        protected ContactsModel _ViewModelData;
        protected ushort? _LookupId = null;
        #endregion
    }
}
