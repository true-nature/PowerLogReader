using PowerLogReader.Core;
using Reactive.Bindings;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PowerLogReader.Modules.Services
{
    public interface IPowerLogService
    {
        Task ScanEventLogsAsync();
        void AbortScan();
        ReactivePropertySlim<DateTime?> ScannedDate { get; }
        DateTime LastSelectedDate { get; set; }
        ObservableCollection<CalendarDateRange> BlackoutDates { get; }
        ReactivePropertySlim<bool> ScanCompleted { get; }
        PowerLogEntry[] GetPowerLogEntries(DateTime date);

        DateTime GetNormalizeStart(DateTime src);
        DateTime GetNormalizeEnd(DateTime src);
    }
}
