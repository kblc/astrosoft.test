using System.Collections.Generic;
using System.Collections.Specialized;

namespace Astrosoft.DataContracts
{
    public interface INotifyCollection<T> : ICollection<T>, INotifyCollectionChanged
    {
    }
}
