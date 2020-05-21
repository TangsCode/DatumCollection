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
    public interface IDataStorage : IDisposable
    {
        /// <summary>
        /// <para>=====>数据增删改</para>
        ///  根据提供的数据存储上下文<see cref="DataStorageContext" />推断Sql语句,
        ///  执行Sql语句返回结果<seealso cref="DbExecutionResult"/>
        ///  <para>成功则<see cref="DbExecutionResult.ErrorCode"/> = 0, 失败则返回对应的<see cref="ErrorCode"/></para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<DbExecutionResult> ExecuteAsync(DataStorageContext context);

        /// <summary>
        /// <para>=====>数据查询</para>
        /// 根据上下文查询所有满足<see cref="DataStorageContext.Parameters"/>筛选条件的数据
        /// </summary>
        /// <typeparam name="T">strong type</typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> Query<T>(DataStorageContext context);

        IDbConnection GetConnection();
    }
}
