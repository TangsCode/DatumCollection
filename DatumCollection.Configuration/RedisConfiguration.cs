using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Configuration
{
    /// <summary>
    /// redis配置项
    /// </summary>
    public class RedisConfiguration
    {
        public string ConnectionString { get; set; }

        public string InstanceName { get; set; }
    }
}
