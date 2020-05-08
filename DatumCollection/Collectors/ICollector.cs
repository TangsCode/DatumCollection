using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Collectors
{
    /// <summary>
    /// 采集抽象接口
    /// </summary>
    public interface ICollector : IDisposable
    {
        /// <summary>
        /// 采集
        /// </summary>
        /// <returns></returns>
        CollectResult Collect(CollectContext context);

        /// <summary>
        /// 异步采集
        /// </summary>
        /// <returns></returns>
        Task<CollectResult> CollectAsync(CollectContext context);
        
    }
}
