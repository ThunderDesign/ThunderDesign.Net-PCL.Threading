using System;
using System.Collections.Generic;
using System.Text;
using ThunderDesign.Net_PCL.Threading.Attributes;

namespace SimpleContacts.Models
{
    internal partial class TestModel
    {
        private void TestFirstName()
        {
            //this.FirstName
        }

        [BindableProperty]
        string _firstName = "John";
    }
}
