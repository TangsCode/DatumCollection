using DatumCollection.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Collectors
{
    /// <summary>
    /// 采集结果
    /// </summary>
    public class CollectResult
    {
        /// <summary>
        /// 是否成功
        /// true    成功
        /// false   失败
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 成功数
        /// </summary>
        public AtomicInteger SuccessCount { get; set; }

        /// <summary>
        /// 失败数
        /// </summary>
        public AtomicInteger FailureCount { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// 返回数据
        /// 数据类型dynamic动态索引所需字段
        /// </summary>
        public dynamic Data { get; set; }

        /// <summary>
        /// 任务数据
        /// 任务完成后用于更新记录
        /// </summary>
        public dynamic ScheduleData { get; set; }

        public CollectResult(int totalCount) : this()
        {
            TotalCount = totalCount;
        }

        public CollectResult()
        {
            Success = false;
            SuccessCount = new AtomicInteger();
            FailureCount = new AtomicInteger();
        }

    }
}
