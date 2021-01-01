using PowerLogReader.Core;
using System;
using System.ComponentModel;

namespace PowerLogReader.Modules.Services
{
    public interface IPreferenceService: INotifyPropertyChanged
    {
        int RoundUnit { get; set; }
        RoundingRule Rule { get; set; }
        TimeSpan DayOffset { get; set; }
        int StartMargin { get; set; }
        int EndMargin { get; set; }
        DayOfWeek FirstDayOfWeek { get; set; }
        long MaxDays { get; set; }
    }
}
