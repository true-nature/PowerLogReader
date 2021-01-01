using PowerLogReader.Core;
using Reactive.Bindings;
using System;

namespace PowerLogReader.Modules.Services
{
    public interface IPowerLogService
    {
        void ScanEventLogsAsync();
        void AbortScan();
        ReactivePropertySlim<DateTime?> ScannedDate { get; }
        DateTime LastSelectedDate { get; set; }
        bool ScanCompleted { get; }
        PowerLogEntry[] GetPowerLogEntries(DateTime date);

        DateTime GetNormalizeStart(DateTime src);
        DateTime GetNormalizeEnd(DateTime src);
    }
}
