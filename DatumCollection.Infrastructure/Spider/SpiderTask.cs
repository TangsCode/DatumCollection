using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Infrastructure.Spider
{
    public class SpiderTask
    {
        public Guid Id { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime FinishTime { get; set; }

        public TimeSpan ElapsedTime { get; }

        public SpiderTask()
        {
            Id = Guid.NewGuid();
            BeginTime = DateTime.Now;
        }
        
    }
}
