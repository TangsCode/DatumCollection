using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Data
{
    /// <summary>
    /// 数据存储接口
    /// </summary>
    interface IDataStorage : IDisposable
    {
        Task<DbExecutionResult> ExecuteAsync();

        IDbConnection GetConnection();
    }
}
