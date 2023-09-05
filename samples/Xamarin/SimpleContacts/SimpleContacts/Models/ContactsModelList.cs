using System;
using System.Collections.Generic;
using System.Text;
using ThunderDesign.Net.Threading.DataCollections;

namespace SimpleContacts.Models
{
    public class ContactsModelList : ObservableDataDictionary<ushort, ContactsModel>
    {
        #region properties
        public static ContactsModelList Instance
        {
            get
            {
                lock (_InstanceLocker)
                {
                    return _Instance ?? (_Instance= new ContactsModelList());
                }
            }
        }
        #endregion

        #region variables
        protected readonly static object _InstanceLocker = new object();
        private static ContactsModelList _Instance = null;
        #endregion
    }
}
