using DatumCollection.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.HostedServices.Schedule
{
    public static class SpiderScheduleExtension
    {
        public static bool OnSchedule(this SpiderScheduleSetting schedule)
        {
            if(schedule == null || !schedule.IsEnabled || DateTime.Now < schedule.StartDate || DateTime.Now > schedule.EndDate)
            {
                return false;
            }

            var dateSpan = DateTime.Now.Date.Subtract(schedule.StartDate);
            var timeSpan = DateTime.Now.TimeOfDay.Subtract(Convert.ToDateTime(schedule.StartTime).TimeOfDay);
            switch (schedule.SpiderFrequency)
            {
                case SpiderFrequency.Once:
                    if(dateSpan < TimeSpan.FromDays(1) && timeSpan < TimeSpan.FromMinutes(1))
                    {
                        return true;
                    }
                    break;
                case SpiderFrequency.Second:
                    if(timeSpan.Seconds % schedule.Interval == 0)
                    {
                        return true;
                    }
                    break;
                case SpiderFrequency.Minute:
                    if(timeSpan.Minutes % schedule.Interval == 0)
                    {
                        return true;
                    }
                    break;
                case SpiderFrequency.Day:
                    if(timeSpan < TimeSpan.FromMinutes(1) && timeSpan.Days % schedule.Interval == 0)
                    {
                        return true;
                    }
                    break;
                case SpiderFrequency.Week:
                    if(timeSpan < TimeSpan.FromMinutes(1) 
                        && timeSpan.Days % (schedule.Interval * 7) == 0
                        && DateTime.Now.DayOfWeek.GetHashCode() == schedule.ScheduleDayOfWeek)
                    {
                        return true;
                    }
                    break;
                case SpiderFrequency.Month:
                    if(timeSpan < TimeSpan.FromMinutes(1) && (DateTime.Now.Month - schedule.ScheduleMonthOfYear) % schedule.Interval == 0)
                    {
                        return true;
                    }
                    break;
                case SpiderFrequency.Season:
                    if(timeSpan < TimeSpan.FromMinutes(1) && (DateTime.Now.Month - schedule.ScheduleMonthOfYear) % 3 == 0)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }

            return false;
        }
    }
}
