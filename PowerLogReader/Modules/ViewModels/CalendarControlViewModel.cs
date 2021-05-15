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
        }

        public Tuple<DateTime, DateTime>[] GetBlackoutDates()
        {
            return PowerLogService.BlackoutDateArray;
        }

        public void Dispose()
        {
            Disposable.Dispose();
            GC.SuppressFinalize(this);
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
                var newDate = date.Value;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SelectedDate.Value = newDate;
                });
            }
        }
    }
}
