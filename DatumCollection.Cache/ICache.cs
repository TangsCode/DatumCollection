using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Cache
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    interface ICache
    {
        Task<T> GetOrCreate<T>(object key, Func<Task<T>> createItem);
    }
}
