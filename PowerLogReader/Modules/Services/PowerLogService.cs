using PowerLogReader.Core;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PowerLogReader.Modules.Services
{
    public class PowerLogService : PowerLogServiceBase
    {
        public PowerLogService(IPreferenceService preference) : base(preference)
        {
        }

        public override async Task ScanEventLogsAsync()
        {
            ScanCompleted.Value = false;
            BlackoutDates.Clear();
            DateTime? lastDate = null;
            var oldest = DateTime.Today - TimeSpan.FromDays(Preference.MaxDays);
            var beforeBlackout = oldest;
            ScannedDate.Value = oldest;
            AllPowerLogs.Clear();
            var entries = new EventLog("System").Entries.Cast<EventLogEntry>().Where(x => x.TimeGenerated >= oldest);
            PowerLogEntry pwle = null;
            foreach (var entry in entries)
            {
                if (ScanCompleted.Value) { break; }
                pwle = ToPowerLogEntry(entry);
                if (pwle != null)
                {
                    AllPowerLogs.Add(pwle);
                    if (lastDate != pwle.Timestamp.Date)
                    {
                        UpdateBlackoutDateRange(beforeBlackout, lastDate);
                        beforeBlackout = lastDate.HasValue ? lastDate.Value : beforeBlackout;
                        ScannedDate.Value = lastDate;
                        lastDate = pwle.Timestamp.Date;
                        await Task.Yield();
                    }
                }
            }
            if (AllPowerLogs.Count > 0 && !ScanCompleted.Value)
            {
                ScannedDate.Value = AllPowerLogs[AllPowerLogs.Count - 1].Timestamp.Date;
                UpdateBlackoutDateRange(ScannedDate.Value, DateTime.Today);
            }
            ScanCompleted.Value = true;
        }

        private PowerLogEntry ToPowerLogEntry(EventLogEntry entry)
        {
            PowerLogEntry pwle = null;
            if (entry.IsPowerOnEvent())
            {
                pwle = new PowerLogEntry { Id = entry.InstanceId, Provider = this.GetSourceName(entry.Source), Timestamp = entry.TimeGenerated - Preference.DayOffset, StartTime = entry.TimeGenerated };
            }
            else if (entry.IsPowerOffEvent())
            {
                pwle = new PowerLogEntry { Id = entry.InstanceId, Provider = this.GetSourceName(entry.Source), Timestamp = entry.TimeGenerated - Preference.DayOffset, EndTime = entry.TimeGenerated };
            }
            return pwle;
        }
    }
}
