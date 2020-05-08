using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Utility.Helper
{
    /// <summary>
    /// 时间帮助类
    /// </summary>
    public static class DateTimeHelper
    {
        private static readonly DateTimeOffset Epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        private const long TickPerMicrosecond = 10;

        /// <summary>
        /// 当天日期        
        /// </summary>
        public static string TodayString => DateTime.Now.ToString("yyyy-MM-dd");

        /// <summary>
        /// 当月日期
        /// </summary>
        public static string MonthString => FirstDayOfMonth.ToString("yyyy-MM-dd");

        /// <summary>
        /// 当周日期
        /// </summary>
        public static string MondayString => Monday.ToString("yyyy-MM-dd");

        /// <summary>
        /// 当月第一天
        /// </summary>
        public static DateTime FirstDayOfMonth
        {
            get
            {
                var nowDay = DateTime.Now.Date;
                return nowDay.AddDays(nowDay.Day * -1 + 1);
            }
        }

        /// <summary>
        /// 当月最后一天
        /// </summary>
        public static DateTime LastDayOfMonth => FirstDayOfMonth.AddMonths(1).AddDays(-1);

        /// <summary>
        /// 上个月第一天
        /// </summary>
        public static DateTime FirstDayOfLastMonth => FirstDayOfMonth.AddMonths(-1);

        /// <summary>
        /// 上个月最后一天
        /// </summary>
        public static DateTime LastDayOfLastMonth => LastDayOfMonth.AddMonths(-1);

        /// <summary>
        /// 星期一
        /// </summary>
        public static DateTime Monday
        {
            get
            {
                var now = DateTime.Now;
                var i = now.DayOfWeek - DayOfWeek.Monday == -1 ? 6 : now.DayOfWeek - DayOfWeek.Monday;
                var ts = new TimeSpan(i, 0, 0, 0);

                return now.Subtract(ts);
            }
        }

        #region Method
        /// <summary>
        /// 把Unix时间转换成DateTime
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime ToUnixTime(long unixTime)
        {
            return Epoch.AddTicks(unixTime * TickPerMicrosecond).DateTime;
        }

        /// <summary>
		/// 获取当前Unix时间
		/// </summary>
		/// <returns>Unix时间</returns>
		public static double GetCurrentUnixTimeNumber()
        {
            return DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;
        }
        #endregion
    }
}
