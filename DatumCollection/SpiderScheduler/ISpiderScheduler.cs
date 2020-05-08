using DatumCollection.Collectors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.SpiderScheduler
{
    /// <summary>
    /// 调度器接口
    /// </summary>
    public interface ISpiderScheduler
    {
        bool IsSatisfiedCondition { get; }

        (bool Success,dynamic[] Sources) Dequeue(int count = 1);

        int Enqueue(IEnumerable<dynamic> sources);

        Task HandleScheduleTask(CollectContext context);

    }
}
