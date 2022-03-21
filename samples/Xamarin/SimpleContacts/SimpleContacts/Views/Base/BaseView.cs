using SimpleContacts.Interfaces;
using SimpleContacts.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using ThunderDesign.Net.Threading.HelperClasses;
using Xamarin.Forms;

namespace SimpleContacts.Views.Base
{
    public abstract class BaseView<T> : ContentPage where T : BaseViewModel, new()
    {
        #region constructors
        public BaseView() : base()
        {
            ViewModel = new T();
            BindingContext = ViewModel;
            RefreshViewCommand = new Command(OnRefreshViewCommand);
        }
        #endregion

        #region properties
        public IBaseViewModel ViewModel { get; protected set; }
        public ICommand RefreshViewCommand { get; protected set; }
        #endregion

        #region methods
        protected virtual void OnRefreshViewCommand()
        {
            ThreadHelper.RunAndForget(async () => await ViewModel.LoadViewModelAsync(true).ConfigureAwait(false));
        }
        #endregion
    }
}
