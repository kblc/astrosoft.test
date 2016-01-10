using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Astrosoft.DataContracts
{ 
    public class NotifyCollection<T> : ObservableCollection<T>, INotifyCollection<T>, IReadOnlyNotifyCollection<T>
    {
    }
}
