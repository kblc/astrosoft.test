using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Astrosoft.DataContracts
{
    public interface ISource : IDisposable
    {
        /// <summary>
        /// Read items from source
        /// </summary>
        /// <param name="itemCountPerPage">Item count per page</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="filter">Filter items</param>
        /// <param name="source">Source</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Read result</returns>
        SourceReadResult Read(long itemCountPerPage, long pageIndex, Func<IItem, bool> filter, string source, CancellationToken cancellationToken);
    }
}
