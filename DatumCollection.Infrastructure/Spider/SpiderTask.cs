using DatumCollection.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Infrastructure.Spider
{
    [Schema("SpiderTask")]
    public class SpiderTask
    {
        [Column("ID","uniqueidentifier")]
        public Guid Id { get; set; }

        [Column("BeginTime","datetime")]
        public DateTime BeginTime { get; set; }

        [Column("FinishTime", "datetime")]
        public DateTime FinishTime { get; set; }

        [Column("ElapsedTime", "datetime")]
        public TimeSpan ElapsedTime { get { return FinishTime == null ? TimeSpan.Zero : FinishTime.Subtract(BeginTime); } }

        public SpiderTask()
        {
            Id = Guid.NewGuid();
            BeginTime = DateTime.Now;
        }
        
    }
}
