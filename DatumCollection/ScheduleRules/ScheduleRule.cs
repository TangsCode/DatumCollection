using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.ScheduleRules
{
    /// <summary>
    /// Schedule运行规则
    /// 规则：
    /// 1.根据Interval和IntervalBy来确定触发间隔时间。
    /// 2.根据StartDate和EndDate来约束运行期间。
    /// 3.根据StartTime和EndTime来约束当天运行时间。
    /// </summary>
    public abstract class ScheduleRule
    {
        /// <summary>
        /// 默认创建每天执行的计划
        /// </summary>
        public ScheduleRule()
        {
            Frequency = 1;
            FrequencyUnit = FrequencyUnit.Day;            
        }

        /// <summary>
        /// 间隔频率
        /// </summary>
        protected int Frequency { get; set; }

        /// <summary>
        /// 间隔单位
        /// </summary>
        protected FrequencyUnit FrequencyUnit { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        protected DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        protected DateTime EndTime { get; set; }

        /// <summary>
        /// 间隔分钟数
        /// </summary>
        protected int IntervalMinutes { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        protected DateTime SatrtDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        protected DateTime EndDate { get; set; }

    }

    /// <summary>
    /// 频率单位
    /// </summary>
    public enum FrequencyUnit
    {
        Second,
        Minute,
        Day,
        Week,
        Month,
        Season,
        Year
    }
}
