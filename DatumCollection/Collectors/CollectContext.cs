using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DatumCollection.Collectors
{
    /// <summary>
    /// 采集上下文
    /// </summary>
    [DebuggerDisplay("{Schedule}{CollectType}{Sources}")]
    public class CollectContext
    {
        /// <summary>
        /// 计划配置
        /// to-do(暂时不用)
        /// </summary>
        public dynamic Schedule { get; set; }

        /// <summary>
        /// 数据源列表
        /// </summary>
        public IEnumerable<dynamic> Sources { get; set; }

        public CollectType CollectType { get; set; }

        /// <summary>
        ///构造函数
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="collectType">采集类型 默认为商品采集</param>
        public CollectContext(IEnumerable<dynamic> sources, dynamic schedule)
        {
            Sources = sources;
            Schedule = schedule;
        }
    }
}
