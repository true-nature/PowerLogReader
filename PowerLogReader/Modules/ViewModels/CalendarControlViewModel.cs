using PowerLogReader.Events;
using PowerLogReader.Modules.Services;
using PowerLogReader.Mvvm;
using Prism.Events;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;

namespace PowerLogReader.Modules.ViewModels
{
    public class CalendarControlViewModel : RegionViewModelBase, IDisposable
    {
        private readonly CompositeDisposable Disposable = new CompositeDisposable();
        private IEventAggregator EventAggregator { get; }
        private readonly IPowerLogService PowerLogService;
        public IPreferenceService Preference { get; }

        public ReactivePropertySlim<DateTime?> SelectedDate { get; } = new ReactivePropertySlim<DateTime?>(DateTime.Today, ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<DateTime?> DisplayDate { get; }
        public ReadOnlyReactivePropertySlim<bool> ScanCompleted { get; }
        public ReadOnlyReactiveCollection<CalendarDateRange> BlackoutDates { get; }

        public CalendarControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IPowerLogService powerLog, IPreferenceService preference) :
            base(regionManager)
        {
            EventAggregator = eventAggregator;
            Preference = preference;
            PowerLogService = powerLog;
            DisplayDate = powerLog.ScannedDate.ToReactiveProperty().AddTo(Disposable);
            DisplayDate.Subscribe(OnDisplayDateChanged);
            SelectedDate.Subscribe(OnSelectedDateChanged);
            ScanCompleted = powerLog.ScanCompleted.ToReadOnlyReactivePropertySlim().AddTo(Disposable);
            BlackoutDates = powerLog.BlackoutDates.ToReadOnlyReactiveCollection();
        }

        public void Dispose()
        {
            Disposable.Dispose();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        private void OnSelectedDateChanged(DateTime? date)
        {
            if (date.HasValue)
            {
                PowerLogService.LastSelectedDate = date.Value;
                EventAggregator.GetEvent<DateChangedEvent>().Publish(date);
            }
        }

        private void OnDisplayDateChanged(DateTime? date)
        {
            if (date.HasValue)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SelectedDate.Value = date.Value;
                });
            }
        }
    }
}
