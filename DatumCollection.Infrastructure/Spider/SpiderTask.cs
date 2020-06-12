using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Infrastructure.Spider
{
    public class SpiderTask
    {
        public string Id { get; set; }

        public DateTime BeginTime { get; internal set; }

        public DateTime FinishTime { get; internal set; }

        public TimeSpan ElapsedTime { get; }
        
    }
}
