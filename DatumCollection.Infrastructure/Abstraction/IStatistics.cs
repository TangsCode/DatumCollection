using DatumCollection.Infrastructure.Spider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Infrastructure.Abstraction
{
    /// <summary>
    /// 统计数据接口
    /// </summary>
    public interface IStatistics
    {
        Task Analyze(SpiderContext conetxt);
    }
}
