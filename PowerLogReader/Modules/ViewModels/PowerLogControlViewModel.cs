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
        private CompositeDisposable Disposable = new CompositeDisposable();
        private IEventAggregator EventAggregator { get; set; }
        private IPowerLogService PowerLogService { get; set; }

        public ObservableCollection<PowerLogEntry> PowerLogs { get; } = new ObservableCollection<PowerLogEntry>();
        public ReactivePropertySlim<PowerLogEntry> Summary { get; } = new ReactivePropertySlim<PowerLogEntry>();

        public PowerLogControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IPowerLogService powerLog) :
            base(regionManager)
        {
            EventAggregator = eventAggregator;
            EventAggregator.GetEvent<DateChangedEvent>().Subscribe(OnDateChanged).AddTo(Disposable);
            PowerLogService = powerLog;
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
