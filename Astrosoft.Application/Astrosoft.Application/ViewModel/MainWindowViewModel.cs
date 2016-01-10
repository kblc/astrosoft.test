using Astrosoft.Application.Additional;
using Astrosoft.DataContracts;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Astrosoft.Application.ViewModel
{
    public class MainWindowViewModel : DependencyObject
    {
        private Timer updateFilterTimer;

        private readonly IDictionary<string, IFilter> filters = new Dictionary<string, IFilter>();

        private void SetFilter(string propertyName, IFilter filter)
        {
            if (updateFilterTimer != null)
            {
                updateFilterTimer.Dispose();
                updateFilterTimer = null;
            }

            if (filters.ContainsKey(propertyName))
            {
                if (filter == null)
                    filters.Remove(propertyName);
                else
                    filters[propertyName] = filter;
            }
            else
            {
                if (filter != null)
                    filters.Add(propertyName, filter);
            }

            updateFilterTimer = new Timer((s) => 
            {
                var scheduler = (TaskScheduler)s;
                updateFilterTimer.Dispose();
                updateFilterTimer = null;

                Task.Factory.StartNew(() => Source.SetFilter(filters.Select(f => new PropertyFilter(f.Key, f.Value)).ToArray(), CancellationToken.None), CancellationToken.None, TaskCreationOptions.None, scheduler);
            }, TaskScheduler.FromCurrentSynchronizationContext(), TimeSpan.FromMilliseconds(200), TimeSpan.FromTicks(0));
        }

        #region Source

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(DataContracts.ModelSource),
            typeof(MainWindowViewModel), new PropertyMetadata(null, (s, e) => 
            {
                var source = (MainWindowViewModel)s;
                if (e.OldValue != null)
                    ((INotifyPropertyChanged)e.OldValue).PropertyChanged -= source.SourcePropertyChanged;
                if (e.NewValue != null)
                    ((INotifyPropertyChanged)e.NewValue).PropertyChanged += source.SourcePropertyChanged;

            }));

        private void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            prevPageCommand?.RaiseCanExecuteChanged();
            nextPageCommand?.RaiseCanExecuteChanged();
            selectSourceCommand?.RaiseCanExecuteChanged();
        }

        public DataContracts.ModelSource Source
        {
            get { return (DataContracts.ModelSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        #endregion

        private DelegateCommand selectSourceCommand = null;
        public ICommand SelectSourceCommand { get { return selectSourceCommand ?? (selectSourceCommand = new DelegateCommand(o =>
        {
            var ofd = new OpenFileDialog() { Filter = "Log files|*.log|All files|*.*" };
            if (ofd.ShowDialog() == true)
            {
                Source.SetSource(ofd.FileName, CancellationToken.None);
            }
        }, o => !Source.IsBusy)); } }

        private DelegateCommand prevPageCommand = null;
        public ICommand PrevPageCommand
        {
            get
            {
                return prevPageCommand ?? (prevPageCommand = new DelegateCommand(o =>
                {
                    Source.PrevPage(CancellationToken.None);
                }, o => !Source.IsBusy && Source.CanPrevPage));
            }
        }

        private DelegateCommand nextPageCommand = null;
        public ICommand NextPageCommand
        {
            get
            {
                return nextPageCommand ?? (nextPageCommand = new DelegateCommand(o =>
                {
                    Source.NextPage(CancellationToken.None);
                }, o => !Source.IsBusy && Source.CanNextPage));
            }
        }

        private DelegateCommand setMesageFilterCommand = null;
        public ICommand SetMesageFilterCommand
        {
            get
            {
                return setMesageFilterCommand ?? (setMesageFilterCommand = new DelegateCommand(o =>
                {
                    var prms = (TextChangedEventArgs)o;
                    var tb = (TextBox)prms.Source;
                    SetFilter(nameof(IItem.Message), Filter.GetFilterContains(tb.Text));
                }, o => !Source.IsBusy));
            }
        }
        

    }
}
