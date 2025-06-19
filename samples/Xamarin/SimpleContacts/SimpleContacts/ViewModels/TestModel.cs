using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Objects;
using ThunderDesign.Net_PCL.Threading.Attributes;

namespace SimpleContacts.ViewModels
{
    internal partial class TestModel// : INotifyPropertyChanged
    {
        //public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private void Test()
        { 
            //this.MiddleName = "John";
        }

        //public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        //{
        //    this.NotifyPropertyChanged(PropertyChanged, propertyName);
        //}

        //[BindableProperty(threadSafe:false,notify:false, alsoNotify: new string[] { "FullName" })]
        //private string _FirstName = String.Empty;

        //[BindableProperty(alsoNotify: new string[] { "FullName" })]
        //private string _lastName = String.Empty;

        [Property(readOnly: true, threadSafe: false)]
        private string _MiddleName = String.Empty;

        //private string FullName => $"{LastName}, {FirstName} {MiddleName}";
        //protected readonly object _Locker = new object();
    }
}
