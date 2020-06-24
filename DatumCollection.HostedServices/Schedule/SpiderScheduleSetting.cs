using DatumCollection.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
 
namespace DatumCollection.HostedServices.Schedule
{
    [Schema("ScheduleSetting")]
    public class SpiderScheduleSetting
    {
        [Column(Name ="Interval")]
        /// <summary>
        /// interval between every schedule action
        /// </summary>
        public int Interval { get; set; }

        [Column(Name ="SpiderFrequency")]
        /// <summary>
        /// spider frequency unit
        /// </summary>
        public SpiderFrequency SpiderFrequency { get; set; }

        [Column(Name = "ScheduleDayOfWeek")]
        public int ScheduleDayOfWeek { get; set; }

        [Column(Name = "ScheduleMonthOfYear")]
        public int ScheduleMonthOfYear { get; set; }

        [Column(Name ="StartTime")]
        /// <summary>
        /// start time of every day
        /// </summary>
        public DateTime StartTime { get; set; }

        [Column(Name = "EndTime")]
        /// <summary>
        /// end time of every day
        /// </summary>
        public DateTime EndTime { get; set; }

        [Column(Name = "StartDate")]
        /// <summary>
        /// start date of schedule setting
        /// </summary>
        public DateTime StartDate { get; set; }

        [Column(Name ="EndDate")]
        /// <summary>
        /// end date of schedule setting
        /// </summary>
        public DateTime EndDate { get; set; }

        [Column(Name = "IsEnabled")]
        public bool IsEnabled { get; set; }
    }

    public enum SpiderFrequency
    {
        Once,
        Second,
        Minute,
        Day,
        Week,
        Month,
        Season
    }
}
