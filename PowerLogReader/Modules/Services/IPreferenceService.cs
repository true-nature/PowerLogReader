using PowerLogReader.Core;
using System;
using System.ComponentModel;

namespace PowerLogReader.Modules.Services
{
    public interface IPreferenceService : INotifyPropertyChanged
    {
        int RoundUnit { get; set; }
        RoundingRule Rounding { get; set; }
        TimeSpan DayOffset { get; set; }
        int StartMargin { get; set; }
        int EndMargin { get; set; }
        DayOfWeek FirstDayOfWeek { get; set; }
        int MaxDays { get; set; }
        bool EnableBlackoutDates { get; set; }
    }
}
