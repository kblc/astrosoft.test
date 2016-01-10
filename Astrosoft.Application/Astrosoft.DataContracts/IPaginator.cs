using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Astrosoft.DataContracts
{
    public interface IPaginator : INotifyPropertyChanged
    {
        long? PageCount { get; }
        long CurrentPage { get; }
        long ItemCountPerPage { get; }

        bool CanSetPage { get; }
        bool CanSetItemCountPerPage { get; }
        bool CanNextPage { get; }
        bool CanPrevPage { get; }

        /// <summary>
        /// Set page index
        /// </summary>
        /// <param name="pageToSelect">Zero based page index</param>
        /// <returns>Task with selected page index</returns>
        Task<long> SetPage(long pageToSelect, CancellationToken cancellationToken);

        /// <summary>
        /// Set item count per page
        /// </summary>
        /// <param name="itemCountPerPage">Item count per each page</param>
        /// <returns>Task with selected page index</returns>
        Task<long> SetItemCountPerPage(long itemCountPerPage, CancellationToken cancellationToken);

        /// <summary>
        /// Set next page
        /// </summary>
        /// <returns>Task with selected page index</returns>
        Task<long> NextPage(CancellationToken cancellationToken);

        /// <summary>
        /// Set prev page
        /// </summary>
        /// <returns>Task with selected page index</returns>
        Task<long> PrevPage(CancellationToken cancellationToken);

        /// <summary>
        /// Set filter for property
        /// </summary>
        /// <param name="filters">Filters to set</param>
        /// <returns>Task with selected page index</returns>
        Task<long> SetFilter(PropertyFilter[] filters, CancellationToken cancellationToken);
    }
}
