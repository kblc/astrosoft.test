using System.ComponentModel;

namespace Astrosoft.DataContracts
{
    public interface IBusy : INotifyPropertyChanged
    {
        bool IsBusy { get; }
    }
}
