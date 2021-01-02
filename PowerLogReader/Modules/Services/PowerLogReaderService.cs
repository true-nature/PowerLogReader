﻿using PowerLogReader.Core;
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

        public override async void ScanEventLogsAsync()
        {
            ScanCompleted = false;
            var oldest = DateTime.Today - TimeSpan.FromDays(Preference.MaxDays);
            ScannedDate.Value = oldest;
            AllPowerLogs.Clear();
            DateTime? lastDate = null;
#pragma warning disable IDE0059 // 値の不必要な代入
            long maxTimeDiff = Preference.MaxDays * 86400000L;
#pragma warning restore IDE0059 // 値の不必要な代入
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
            PowerLogEntry pwle = null;
            EventRecord record = reader.ReadEvent();
            while (record != null && !ScanCompleted)
            {
                pwle = ToPowerLogEntry(record);
                if (pwle != null)
                {
                    AllPowerLogs.Add(pwle);
                    if (lastDate != pwle.Timestamp.Date)
                    {
                        ScannedDate.Value = lastDate;
                        lastDate = pwle.Timestamp.Date;
                        await Task.Yield();
                    }
                }
                record = reader.ReadEvent();
            }
            if (AllPowerLogs.Count > 0 && !ScanCompleted)
            {
                ScannedDate.Value = AllPowerLogs[AllPowerLogs.Count - 1].Timestamp.Date;
            }
            ScanCompleted = true;
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
