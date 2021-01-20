using DatumCollection.Infrastructure.Abstraction;
using DatumCollection.Infrastructure.Spider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Pipline.Statistics
{
    /// <summary>
    /// 默认统计器
    /// </summary>
    public class DefaultStatistics : IStatistics
    {
        public Task Analyze(SpiderContext conetxt)
        {
            throw new NotImplementedException();
        }
    }
}
