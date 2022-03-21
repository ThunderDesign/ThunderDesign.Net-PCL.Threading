using SimpleContacts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.DataCollections;
using ThunderDesign.Net.ToolBox.Extentions;

namespace SimpleContacts.Services
{
    public static class ContactsService
    {
        #region properties
        public static ushort UniqueId
        {
            get { return (ushort)Interlocked.Increment(ref _UniqueId); }
            private set { Interlocked.Exchange(ref _UniqueId, value); }
        }
        #endregion

        #region methods
        public async static Task<ContactsModelList> GetContactsAsync()
        {
            return await Task.Run<ContactsModelList>(() =>
            {
                ContactsModelList.Instance.Clear();
                UniqueId = 0;
                ContactsModelList.Instance.Add(new ContactsModel() { Id = UniqueId, FirstName = "Stephan", LastName = "Enrico" });
                ContactsModelList.Instance.Add(new ContactsModel() { Id = UniqueId, FirstName = "Nichol", LastName = "Valentine" });
                ContactsModelList.Instance.Add(new ContactsModel() { Id = UniqueId, FirstName = "Peggie", LastName = "Rawstorn" });
                ContactsModelList.Instance.Add(new ContactsModel() { Id = UniqueId, FirstName = "Jed", LastName = "Isherwood" });
                ContactsModelList.Instance.Add(new ContactsModel() { Id = UniqueId, FirstName = "Monika", LastName = "Conen" });
                return ContactsModelList.Instance;
            }).ConfigureAwait(false);
        }

        public async static Task<ContactsModel> GetContactAsync(ushort Id)
        {
            ContactsModel result = null;

            await Task.Run(() =>
            {
                if (Id == 0)
                {
                    result = new ContactsModel();
                }
                else if (ContactsModelList.Instance.TryGetValue(Id, out var liveContactsModel))
                {
                    result = new ContactsModel();
                    result.Mirror(liveContactsModel);
                }
            }).ConfigureAwait(false);

            return result;
        }

        public async static Task<bool> SaveContactAsync(ContactsModel contactsModel)
        {
            bool result = false;

            await Task.Run(() =>
            {
                if (contactsModel.Id == 0)
                {
                    contactsModel.Id = UniqueId;
                    ContactsModelList.Instance.Add(contactsModel);
                    result = true;
                }
                else
                {
                    if (ContactsModelList.Instance.TryGetValue(contactsModel.Id, out var liveContactModel))
                    {
                        liveContactModel.Mirror(contactsModel);
                        result = true;
                    }
                }
            }).ConfigureAwait(false);

            return result;
        }

        public async static Task<bool> DeleteContactAsync(ushort Id)
        {
            bool result = false;

            await Task.Run(() =>
            {
                if (ContactsModelList.Instance.ContainsKey(Id))
                    result = ContactsModelList.Instance.Remove(Id);
            }).ConfigureAwait(false);

            return result;
        }
        #endregion

        #region variables
        private static int _UniqueId = 0;
        #endregion
    }
}
