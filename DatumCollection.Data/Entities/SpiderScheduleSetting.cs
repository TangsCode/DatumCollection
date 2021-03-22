using DatumCollection.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
 
namespace DatumCollection.Data.Entities
{
    [Schema("ScheduleSetting")]
    public class SpiderScheduleSetting : SystemBase
    {
        [Column(Name ="Interval",Type = "int")]
        /// <summary>
        /// interval between every schedule action
        /// </summary>
        public int Interval { get; set; }

        [Column(Name ="SpiderFrequency",Type = "int")]
        /// <summary>
        /// spider frequency unit
        /// </summary>
        public SpiderFrequency SpiderFrequency { get; set; }

        [Column(Name = "ScheduleDayOfWeek", Type = "int")]
        public int ScheduleDayOfWeek { get; set; }

        [Column(Name = "ScheduleMonthOfYear", Type = "int")]
        public int ScheduleMonthOfYear { get; set; }

        [Column(Name ="StartTime", Type = "datetime")]
        /// <summary>
        /// start time of every day
        /// </summary>
        public DateTime StartTime { get; set; }

        [Column(Name = "EndTime", Type = "datetime")]
        /// <summary>
        /// end time of every day
        /// </summary>
        public DateTime EndTime { get; set; }

        [Column(Name = "StartDate", Type = "datetime")]
        /// <summary>
        /// start date of schedule setting
        /// </summary>
        public DateTime StartDate { get; set; }

        [Column(Name ="EndDate", Type = "datetime")]
        /// <summary>
        /// end date of schedule setting
        /// </summary>
        public DateTime EndDate { get; set; }

        [Column(Name = "IsEnabled", Type = "bit")]
        public bool IsEnabled { get; set; }

        [JoinTable("ID", "FK_SpiderSchedule_ID", JoinType.Left)]
        public IEnumerable<SpiderScheduleItems> SpiderScheduleItems { get; set; }

        #region 数据是否采集
        [Column(Name = "getPrice", Type = "bit")]
        public bool getPrice { get; set; }

        [Column(Name = "getPostage", Type = "bit")]
        public bool getPostage { get; set; }

        [Column(Name = "getPreferential", Type = "bit")]
        public bool getPreferential { get; set; }

        [Column(Name = "getCoupon", Type = "bit")]
        public bool getCoupon { get; set; }

        [Column(Name = "getImageFile", Type = "bit")]
        public bool getImageFile { get; set; }

        [Column(Name = "getTaxFee", Type = "bit")]
        public bool getTaxFee { get; set; }

        [Column(Name = "getScreenshot", Type = "bit")]
        public bool getScreenshot { get; set; }
        #endregion
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
