using PowerLogReader.Core;
using Reactive.Bindings;
using System;
using System.Threading.Tasks;

namespace PowerLogReader.Modules.Services
{
    public interface IPowerLogService
    {
        Task ScanEventLogsAsync();
        void AbortScan();
        ReactivePropertySlim<DateTime?> ScannedDate { get; }
        DateTime LastSelectedDate { get; set; }
        ReactivePropertySlim<bool> ScanCompleted { get; }
        PowerLogEntry[] GetPowerLogEntries(DateTime date);
        Tuple<DateTime, DateTime>[] BlackoutDateArray { get; }

        DateTime GetNormalizeStart(DateTime src);
        DateTime GetNormalizeEnd(DateTime src);
    }
}
