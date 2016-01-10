using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Astrosoft.DataContracts
{
    public interface IReadOnlyNotifyCollection<out T> : IEnumerable<T>, IEnumerable, INotifyCollectionChanged
    {
    }
}
