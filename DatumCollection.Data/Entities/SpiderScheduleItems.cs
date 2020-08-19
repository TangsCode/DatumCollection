using DatumCollection.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data.Entities
{
    [Schema("ScheduleItems")]
    public class SpiderScheduleItems : SystemBase
    {
        [Column(Name = "FK_SpiderSchedule_ID", Type = "uniqueidentifier")]
        public Guid FK_SpiderSchedule_ID { get; set; }

        [Column(Name = "FK_SpiderItem_ID", Type = "uniqueidentifier")]
        public Guid FK_SpiderItem_ID { get; set; }

        [JoinTable("FK_SpiderSchedule_ID")]
        public SpiderScheduleSetting SpiderScheduleSetting { get; set; }
    }
}
