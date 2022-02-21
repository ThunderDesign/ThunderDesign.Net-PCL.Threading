using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IBindableObject : INotifyPropertyChanged
    {
        #region methods
        void OnPropertyChanged([CallerMemberName] string propertyName = "");
        #endregion
    }
}
