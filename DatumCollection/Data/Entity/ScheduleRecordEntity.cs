using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data.Entity
{
    public class ScheduleRecord : SystemBase
    {
        public string ID { get; set; }
        public string StartupTime { get; set; }
        public string FinishTime { get; set; }
        public decimal ElapsedMinutes { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public int TotalCount { get; set; }
        public string FK_ScheduleConfig_ID { get; set; }
    }
}
