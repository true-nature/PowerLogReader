using PowerLogReader.Core;
using Prism.Mvvm;
using System;

namespace PowerLogReader.Modules.Services
{
    public class PreferenceService : BindableBase, IPreferenceService
    {
        private int rountUnit = 5;
        public int RoundUnit
        {
            get => rountUnit > 0 ? rountUnit : 1;
            set
            {
                SetProperty(ref rountUnit, value);
                Properties.Settings.Default.RoundUnit = value;
                Properties.Settings.Default.Save();
            }
        }

        private RoundingRule rule = RoundingRule.None;
        public RoundingRule Rounding
        {
            get => rule;
            set
            {
                SetProperty(ref rule, value);
                Properties.Settings.Default.RoundingRule = value.ToString();
                Properties.Settings.Default.Save();
            }
        }

        private TimeSpan dayOffset = new TimeSpan(3, 0, 0);
        public TimeSpan DayOffset
        {
            get => dayOffset;
            set
            {
                SetProperty(ref dayOffset, value);
                Properties.Settings.Default.OffsetMinutes = (int)value.TotalMinutes;
                Properties.Settings.Default.Save();
            }
        }

        private DayOfWeek firstDay = DayOfWeek.Monday;
        public DayOfWeek FirstDayOfWeek
        {
            get => firstDay;
            set
            {
                SetProperty(ref firstDay, value);
                Properties.Settings.Default.FirstDayOfWeek = value.ToString();
                Properties.Settings.Default.Save();
            }
        }

        private int maxDays = 30;
        public int MaxDays
        {
            get => maxDays;
            set
            {
                SetProperty(ref maxDays, value);
                Properties.Settings.Default.MaxDays = value;
                Properties.Settings.Default.Save();
            }
        }

        private int startMargin = 3;
        public int StartMargin
        {
            get => startMargin;
            set
            {
                SetProperty(ref startMargin, value);
                Properties.Settings.Default.StartMargin = value;
                Properties.Settings.Default.Save();
            }
        }

        private int endMargin = 2;

        public int EndMargin
        {
            get => endMargin;
            set
            {
                SetProperty(ref endMargin, value);
                Properties.Settings.Default.EndMargin = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool enableBlackoutDates = true;
        public bool EnableBlackoutDates
        {
            get => enableBlackoutDates;
            set
            {
                SetProperty(ref enableBlackoutDates, value);
                Properties.Settings.Default.EnableBlackoutDates = value;
                Properties.Settings.Default.Save();
            }
        }


        public PreferenceService()
        {
            var settings = Properties.Settings.Default;
            rountUnit = settings.RoundUnit;
            Enum.TryParse<RoundingRule>(settings.RoundingRule, out rule);
            dayOffset = TimeSpan.FromMinutes(settings.OffsetMinutes);
            maxDays = settings.MaxDays;
            startMargin = settings.StartMargin;
            endMargin = settings.EndMargin;
            enableBlackoutDates = settings.EnableBlackoutDates;
            Enum.TryParse<DayOfWeek>(settings.FirstDayOfWeek, out firstDay);
        }

    }
}
