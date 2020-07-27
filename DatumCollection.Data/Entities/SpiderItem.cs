using DatumCollection.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data.Entities
{
    [Schema("SpiderItem")]
    public class SpiderItem : SystemBase
    {
        [Column(Name = "Url", Type = "nvarchar")]
        public string Url { get; set; }

        [Column(Name = "Method", Type = "varchar", Length = 50)]
        public string Method { get; set; }

        [Column(Name = "ContentType", Type = "nvarchar")]
        public string ContentType { get; set; }

        [Column(Name = "Encoding", Type = "nvarchar")]
        public string Encoding { get; set; }
        
    }
}
