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

        public SpiderTask()
        {
            Id = Guid.NewGuid();
            BeginTime = DateTime.Now;
        }
        
    }
}
