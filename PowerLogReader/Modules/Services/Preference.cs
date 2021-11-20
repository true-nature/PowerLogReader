using PowerLogReader.Core;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace PowerLogReader.Modules.Services
{
    [DataContract(Name="Preference", Namespace = "")]
    public class Preference
    {
        [DataMember]
        public int RoundUnit { get; set; } = 1;
        [DataMember]
        public RoundingRule Rounding { get; set; } = RoundingRule.None;
        [XmlIgnore]
        public TimeSpan DayOffset => TimeSpan.FromMinutes(DayOffsetMinutes);
        [DataMember]
        public int DayOffsetMinutes { get; set; } = 180;
        [DataMember]
        public int StartMargin { get; set; } = 3;
        [DataMember]
        public int EndMargin { get; set; } = 2;
        [DataMember]
        public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Monday;
        [DataMember]
        public int MaxDays { get; set; } = 30;
        [DataMember]
        public bool EnableBlackoutDates { get; set; } = true;
    }
}
