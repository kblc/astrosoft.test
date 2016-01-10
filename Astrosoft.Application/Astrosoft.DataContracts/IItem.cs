using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Astrosoft.DataContracts
{
    public interface IItem : INotifyPropertyChanged
    {
        long Index { get; }

        DateTime Time { get; }
        char Type { get; }
        string System { get; }
        string Message { get; }
    }
}
