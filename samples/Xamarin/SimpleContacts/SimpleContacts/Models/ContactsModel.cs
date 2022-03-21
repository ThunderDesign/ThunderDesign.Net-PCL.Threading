using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThunderDesign.Net.Threading.DataObjects;
using ThunderDesign.Net.Threading.Extentions;

namespace SimpleContacts.Models
{
    public class ContactsModel : BindableDataObject<ushort>
    {
        #region properties
        public string FirstName
        {
            get { return this.GetProperty(ref _FirstName, _Locker); }
            set { this.SetProperty(ref _FirstName, value, _Locker, true); }
        }

        public string LastName
        {
            get { return this.GetProperty(ref _LastName, _Locker); }
            set { this.SetProperty(ref _LastName, value, _Locker, true); }
        }

        public string FullName
        {
            get { return GetFullName(); }
        }
        #endregion

        #region methods
        protected virtual string GetFullName()
        {
            return $"{LastName}, {FirstName}";
        }

        public override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            string[] array = { nameof(this.FirstName), nameof(this.LastName) };
            if (array.Contains(propertyName, StringComparer.OrdinalIgnoreCase))
            {
                base.OnPropertyChanged(nameof(this.FullName));
            }
        }
        #endregion

        #region variables
        protected string _FirstName = String.Empty;
        protected string _LastName = String.Empty;
        #endregion
    }
}
