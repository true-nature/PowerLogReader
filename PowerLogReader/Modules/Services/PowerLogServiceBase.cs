using PowerLogReader.Core;
using Reactive.Bindings;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PowerLogReader.Modules.Services
{
    public abstract class PowerLogServiceBase : IPowerLogService
    {
        protected IPreferenceService Preference { get; }

        protected ObservableCollection<PowerLogEntry> AllPowerLogs { get; } = new ObservableCollection<PowerLogEntry>();

        public ReactivePropertySlim<DateTime?> ScannedDate { get; } = new ReactivePropertySlim<DateTime?>(null);

        public DateTime LastSelectedDate { get; set; }

        public bool ScanCompleted { get; protected set; }

        public PowerLogServiceBase(IPreferenceService preference)
        {
            Preference = preference;
            LastSelectedDate = DateTime.Today;
        }

        public abstract void ScanEventLogsAsync();

        public void AbortScan()
        {
            ScanCompleted = true;
        }

        public PowerLogEntry[] GetPowerLogEntries(DateTime date)
        {
            return AllPowerLogs.Where(x => x.Timestamp.Date == date.Date).OrderBy(x => x.Timestamp).ToArray();
        }

        public DateTime GetNormalizeStart(DateTime src)
        {
            var normalized = src - TimeSpan.FromMinutes(Preference.StartMargin);
            double minUnits = (normalized.Minute + normalized.Second / 60.0) / (double)Preference.RoundUnit;
            switch (Preference.Rule)
            {
                case RoundingRule.RoundingOff:
                    minUnits = Math.Round(minUnits);
                    break;
                case RoundingRule.Truncate:
                    minUnits = Math.Ceiling(minUnits);
                    break;
                case RoundingRule.RoundingUp:
                    minUnits = Math.Floor(minUnits);
                    break;
                default:
                    break;
            }
            int minute = (int)(minUnits * Preference.RoundUnit);
            if (minute >= 60)
            {
                normalized = normalized.AddHours(1);
                minute -= 60;
            }
            else if (minute < 0)
            {
                normalized = normalized.AddHours(-1);
                minute += 60;
            }
            return new DateTime(normalized.Year, normalized.Month, normalized.Day, normalized.Hour, minute, 0);
        }

        public DateTime GetNormalizeEnd(DateTime src)
        {
            var normalized = src + TimeSpan.FromMinutes(Preference.EndMargin);
            double minUnits = (normalized.Minute + normalized.Second / 60.0) / (double)Preference.RoundUnit;
            switch (Preference.Rule)
            {
                case RoundingRule.RoundingOff:
                    minUnits = Math.Round(minUnits);
                    break;
                case RoundingRule.Truncate:
                    minUnits = Math.Floor(minUnits);
                    break;
                case RoundingRule.RoundingUp:
                    minUnits = Math.Ceiling(minUnits);
                    break;
                default:
                    break;
            }
            int minute = (int)(minUnits * Preference.RoundUnit);
            if (minute >= 60)
            {
                normalized = normalized.AddHours(1);
                minute -= 60;
            }
            else if (minute < 0)
            {
                normalized = normalized.AddHours(-1);
                minute += 60;
            }
            return new DateTime(normalized.Year, normalized.Month, normalized.Day, normalized.Hour, minute, 0);
        }
    }
}
