using PowerLogReader.Core;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;

namespace PowerLogReader.Modules.Services
{
    public class PowerLogReaderService : PowerLogServiceBase
    {
        public PowerLogReaderService(IPreferenceService preference) : base(preference)
        {
        }

        public override async Task ScanEventLogsAsync()
        {
            ScanCompleted.Value = false;
            BlackoutDates.Clear();
            var oldest = DateTime.Today - TimeSpan.FromDays(Preference.MaxDays);
            var beforeBlackout = oldest;
            ScannedDate.Value = oldest;
            AllPowerLogs.Clear();
            DateTime? lastDate = null;
            long maxTimeDiff = Preference.MaxDays * 86400000L;
            var queryStr = "<QueryList><Query Id=\"0\" Path=\"System\">"
                + "<Select Path =\"System\">"
                + "*[System[Provider[@Name = 'Microsoft-Windows-Kernel-Boot'"
                + " or @Name = 'Microsoft-Windows-Kernel-General'"
                + " or @Name = 'Microsoft-Windows-Kernel-Power'"
                + " or @Name = 'EventLog'] "
                + " and TimeCreated[timediff(@SystemTime) &lt;= " + $"{maxTimeDiff}"
                + "]]] </Select></Query></QueryList>";
            var query = new EventLogQuery("System", PathType.LogName, queryStr);
            EventLogReader reader = new EventLogReader(query);
            EventRecord record = reader.ReadEvent();
            while (record != null && !ScanCompleted.Value)
            {
                PowerLogEntry pwle = ToPowerLogEntry(record);
                if (pwle != null)
                {
                    AllPowerLogs.Add(pwle);
                    if (lastDate != pwle.Timestamp.Date)
                    {
                        UpdateBlackoutDateRange(beforeBlackout, lastDate);
                        beforeBlackout = lastDate ?? beforeBlackout;
                        ScannedDate.Value = lastDate;
                        lastDate = pwle.Timestamp.Date;
                        await Task.Yield();
                    }
                }
                record = reader.ReadEvent();
            }
            if (AllPowerLogs.Count > 0 && !ScanCompleted.Value)
            {
                ScannedDate.Value = AllPowerLogs[AllPowerLogs.Count - 1].Timestamp.Date;
                UpdateBlackoutDateRange(ScannedDate.Value, DateTime.Today + TimeSpan.FromDays(1));
            }
            ScanCompleted.Value = true;
        }

        private PowerLogEntry ToPowerLogEntry(EventRecord record)
        {
            PowerLogEntry pwle = null;
            if (record.IsPowerOnEvent())
            {
                pwle = new PowerLogEntry { Id = record.Id, Provider = this.GetSourceName(record.ProviderName), Timestamp = record.TimeCreated.Value - Preference.DayOffset, StartTime = record.TimeCreated.Value };
            }
            else if (record.IsPowerOffEvent())
            {
                pwle = new PowerLogEntry { Id = record.Id, Provider = this.GetSourceName(record.ProviderName), Timestamp = record.TimeCreated.Value - Preference.DayOffset, EndTime = record.TimeCreated.Value };
            }
            return pwle;
        }
    }
}
