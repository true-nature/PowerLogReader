using PowerLogReader.Core;
using PowerLogReader.Events;
using PowerLogReader.Modules.Services;
using PowerLogReader.Mvvm;
using Prism.Events;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace PowerLogReader.Modules.ViewModels
{
    public class PowerLogControlViewModel : RegionViewModelBase, IDisposable
    {
        private readonly CompositeDisposable Disposable = new CompositeDisposable();
        private IEventAggregator EventAggregator { get; }
        private IPowerLogService PowerLogService { get; }
        private IPreferenceService Preference { get; }

        public ObservableCollection<PowerLogEntry> PowerLogs { get; } = new ObservableCollection<PowerLogEntry>();
        public ReactivePropertySlim<PowerLogEntry> Summary { get; } = new ReactivePropertySlim<PowerLogEntry>();
        public ReactivePropertySlim<RoundingRule> Rounding { get; } = new ReactivePropertySlim<RoundingRule>();

        public PowerLogControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IPowerLogService powerLog, IPreferenceService preference) :
            base(regionManager)
        {
            EventAggregator = eventAggregator;
            EventAggregator.GetEvent<DateChangedEvent>().Subscribe(OnDateChanged, ThreadOption.UIThread).AddTo(Disposable);
            PowerLogService = powerLog;
            Preference = preference;
        }

        public void Dispose()
        {
            Disposable.Dispose();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Task.Run(() => PowerLogService.ScanEventLogsAsync());
        }

        private void OnDateChanged(DateTime? date)
        {
            PowerLogs.Clear();
            Rounding.Value = Preference.Rounding;
            var summary = new PowerLogEntry();
            if (date.HasValue)
            {
                var entries = PowerLogService.GetPowerLogEntries(date.Value);
                PowerLogs.AddRange(entries);
                if (PowerLogs.Count > 0)
                {
                    var startTime = PowerLogs.FirstOrDefault(x => x.StartTime != null)?.StartTime;
                    if (startTime.HasValue)
                    {
                        summary.StartTime = PowerLogService.GetNormalizeStart(startTime.Value);
                    }
                    var endTime = PowerLogs.Reverse().FirstOrDefault(x => x.EndTime != null)?.EndTime;
                    if (endTime.HasValue)
                    {
                        summary.EndTime = PowerLogService.GetNormalizeEnd(endTime.Value);
                    }
                }
            }
            Summary.Value = summary;
        }
    }
}
