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
        private CompositeDisposable Disposable = new CompositeDisposable();
        private IEventAggregator EventAggregator { get; }
        private IPowerLogService PowerLogService;
        public IPreferenceService Preference { get; }

        public ReactivePropertySlim<DateTime> DisplayDate { get; } = new ReactivePropertySlim<DateTime>();
        public ReactiveProperty<DateTime?> SelectedDate { get; } 

        public CalendarControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IPowerLogService powerLog, IPreferenceService preference) :
            base(regionManager)
        {
            EventAggregator = eventAggregator;
            Preference = preference;
            PowerLogService = powerLog;
            SelectedDate = powerLog.ScannedDate.ToReactiveProperty().AddTo(Disposable);
            SelectedDate.Subscribe(OnDateChanged);
        }

        public void Dispose()
        {
            Disposable.Dispose();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            DisplayDate.Value = DateTime.Today;
        }

        private void OnDateChanged(DateTime? date)
        {
            if (date.HasValue)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DisplayDate.Value = date.Value;
                    PowerLogService.LastSelectedDate = date.Value;
                    EventAggregator.GetEvent<DateChangedEvent>().Publish(date);
                });
            }
        }
    }
}
