using DatumCollection.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Infrastructure.Spider
{
    [Schema("SpiderTask")]
    public class SpiderTask
    {
        [Column(Name = "ID", Type = "uniqueidentifier")]
        public Guid Id { get; set; }

        [Column(Name = "BeginTime", Type = "datetime")]
        public DateTime BeginTime { get; set; }

        [Column(Name = "FinishTime", Type = "datetime")]
        public DateTime FinishTime { get; set; }

        [Column(Name = "ElapsedTime", Type = "float")]
        public double ElapsedTime { get { return FinishTime == null ? 0 : (FinishTime - BeginTime).TotalSeconds; } }

        [Column(Name = "SuccessCount", Type = "int")]
        public int SuccessCount { get; set; }

        [Column(Name = "FailedCount", Type = "int")]
        public int FailedCount { get; set; }

        public SpiderTask()
        {
            Id = Guid.NewGuid();
            BeginTime = DateTime.Now;
        }
        
    }
}
