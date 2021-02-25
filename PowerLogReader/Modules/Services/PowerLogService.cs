using PowerLogReader.Core;
using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

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
                        UpdateBlackoutDateRange(lastDate, pwle.Timestamp.Date);
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

        private void ScanLogs()
        {
            ScanCompleted.Value = false;
#pragma warning disable IDE0059 // 値の不必要な代入
            long oldest = Preference.MaxDays * 86400000L;
#pragma warning restore IDE0059 // 値の不必要な代入
            var queryStr = $"<QueryList><Query Id='0' Path='System'>"
                + "<Select Path ='System'>"
                + "*[System[Provider[@Name = 'Microsoft-Windows-Kernel-Boot'"
                + " or @Name = 'Microsoft-Windows-Kernel-General'"
                + " or @Name = 'Microsoft-Windows-Kernel-Power']]] "
                + " and TimeCreated[timediff(@SystemTime) &lt;= {oldest}]]]"
                + " </Select></ Query></QueryList>";
            var query = new EventLogQuery("System", PathType.LogName, queryStr);
            EventLogReader reader = new EventLogReader(query);
            EventRecord record = reader.ReadEvent();
            while (record != null && !ScanCompleted.Value)
            {
                record = reader.ReadEvent();
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
