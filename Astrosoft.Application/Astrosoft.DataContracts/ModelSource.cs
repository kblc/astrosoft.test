using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Astrosoft.DataContracts
{
    public class ModelSource : NotifyCollection<IItem>, IBusy, IPaginator, IReadOnlyNotifyCollection<IItem>, IDisposable, INotifyPropertyChanged
    {
        private const long ITEMS_PER_PAGE_DEFAULT = 100;

        private ISource source;
        private bool isBusy = false;
        private long? pageCount = null;
        private long currentPage = 0;
        private long itemCountPerPage = 0;
        private readonly IDictionary<string, IFilter> filters = new Dictionary<string, IFilter>();
        private string sourcePath = string.Empty;
        private string error = string.Empty;

        public ModelSource(ISource source, long itemsPerPage)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            this.source = source;
            this.ItemCountPerPage = itemsPerPage;
            SetPage(0, CancellationToken.None);
        }

        public string Source { get { return sourcePath; } private set { sourcePath = value; RaisePropertyChanged(); } }

        public string Error { get { return error; } private set { error = value; RaisePropertyChanged(); } }

        public Task<long> SetSource(string source, CancellationToken cancellationToken) 
            => ReloadFromSource(ItemCountPerPage, CurrentPage, source, cancellationToken);

        #region INotifyPropertyChanged

        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpr = propertyExpression.Body as MemberExpression;
            if (memberExpr == null)
                throw new ArgumentException("propertyExpression should represent access to a member");
            string memberName = memberExpr.Member.Name;
            base.OnPropertyChanged(new PropertyChangedEventArgs(memberName));
        }

        protected virtual void RaisePropertyChanged([ParenthesizePropertyName] string propertyName = "")
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    source.Dispose();
                }
                disposedValue = true;
            }
        }

        ~ModelSource()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
        #region IBusy

        public bool IsBusy
        {
            get { return isBusy; }
            private set { if (isBusy == value) return; isBusy = value; RaisePropertyChanged(); }
        }

        #endregion
        #region IPaginator properties

        public long? PageCount
        {
            get { return pageCount; }
            private set
            {
                if (pageCount == value)
                    return;
                pageCount = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => CanSetPage);
                RaisePropertyChanged(() => CanSetItemCountPerPage);
                RaisePropertyChanged(() => CanNextPage);
                RaisePropertyChanged(() => CanPrevPage);
            }
        }

        public long CurrentPage
        {
            get { return currentPage; }
            private set
            {
                if (currentPage == value)
                    return; currentPage = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => CanSetPage);
                RaisePropertyChanged(() => CanSetItemCountPerPage);
                RaisePropertyChanged(() => CanNextPage);
                RaisePropertyChanged(() => CanPrevPage);
            }
        }

        public long ItemCountPerPage
        {
            get { return itemCountPerPage; }
            private set { if (itemCountPerPage == value) return; itemCountPerPage = value; RaisePropertyChanged(); }
        }

        public bool CanSetPage => pageCount.HasValue;

        public bool CanSetItemCountPerPage => pageCount.HasValue;

        public bool CanNextPage => pageCount.HasValue && CurrentPage < pageCount.Value - 1;

        public bool CanPrevPage => pageCount.HasValue && CurrentPage > 0;

        #endregion
        #region IPaginator methods

        public Task<long> SetPage(long pageToSelect, CancellationToken cancellationToken)
            => ReloadFromSource(ItemCountPerPage, pageToSelect, Source, cancellationToken);

        public Task<long> SetItemCountPerPage(long itemCountPerPage, CancellationToken cancellationToken)
            => ReloadFromSource(itemCountPerPage, CurrentPage, Source, cancellationToken);

        public Task<long> NextPage(CancellationToken cancellationToken)
        {
            if (CanNextPage)
                return SetPage(Math.Min(CurrentPage + 1, PageCount.Value - 1), cancellationToken);
            else
                throw new Exception("Next not allowed");
        }

        public Task<long> PrevPage(CancellationToken cancellationToken)
        {
            if (CanPrevPage)
                return SetPage(Math.Max(CurrentPage - 1, 0), cancellationToken);
            else
                throw new Exception("Prev not allowed");
        }

        public Task<long> SetFilter(PropertyFilter[] incomingFilters, CancellationToken cancellationToken)
        {
            foreach (var incomingFilter in incomingFilters)
            {
                if (filters.ContainsKey(incomingFilter.PropertyName))
                {
                    if (incomingFilter.Filter == null)
                        filters.Remove(incomingFilter.PropertyName);
                    else
                        filters[incomingFilter.PropertyName] = incomingFilter.Filter;
                }
                else
                {
                    if (incomingFilter.Filter != null)
                        filters.Add(incomingFilter.PropertyName, incomingFilter.Filter);
                }
            }
            return ReloadFromSource(ItemCountPerPage, CurrentPage, Source, cancellationToken);
        }

        /// <summary>
        /// Filter (exclude) item from result
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <returns>True to exclude item. Otherwise false</returns>
        private bool Filter(IItem item)
        {
            var filterResult = false;
            foreach(var filter in filters) {
                var p = item.GetType().GetProperty(filter.Key);
                if (p != null)
                {
                    var value = p.GetValue(item, null);
                    filterResult |= !filter.Value.isFiltered(value);

                    if (filterResult)
                        break;
                }
            }

            return filterResult;
        }

        /// <summary>
        /// Read data from source
        /// </summary>
        /// <param name="itemCountPerPage">Item count per page</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task with selected page index</returns>
        private Task<long> ReloadFromSource(long itemCountPerPage, long pageIndex, string source, CancellationToken cancellationToken)
        {
            var task = Task.Factory.StartNew(() => this.source.Read(itemCountPerPage, pageIndex, Filter, source, cancellationToken), cancellationToken);

            IsBusy = true;

            return task.ContinueWith((t) => 
            {
                try {

                    cancellationToken.ThrowIfCancellationRequested();
                    if (t.Exception != null)
                        throw t.Exception;

                    this.PageCount = t.Result.PageCount;
                    this.CurrentPage = t.Result.PageIndex;
                    this.Source = t.Result.Source;
                    this.Clear();
                    foreach (var i in t.Result.Items)
                        this.Add(i);

                    return t.Result.PageIndex;
                }
                finally
                {
                    IsBusy = false;
                }
            }, cancellationToken, TaskContinuationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion
    }
}
