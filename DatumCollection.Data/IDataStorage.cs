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
        /// <para>数据增删改</para>
        ///  根据提供的数据存储上下文<see cref="DataStorageContext" />推断Sql语句,
        ///  执行Sql语句返回结果<seealso cref="DbExecutionResult"/>
        ///  <para>成功则<see cref="DbExecutionResult.ErrorCode"/> = 0, 失败则返回对应的<see cref="ErrorCode"/></para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<DbExecutionResult> ExecuteAsync(DataStorageContext context);

        /// <summary>
        /// 执行返回单个结果<see cref="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<T> ExecuteScalarAsync<T>(DataStorageContext context);
         
        /// <summary>
        /// <para>数据查询</para>
        /// <para>优先级：<see cref="DataStorageContext.SqlStatement"/> > <see cref="DataStorageContext.Parameters"/></para>
        /// <para>根据上下文<see cref="DataStorageContext.MainTable"/>查询全部字段</para>        
        /// 根据上下文查询所有满足<see cref="DataStorageContext.Parameters"/>筛选条件的数据
        /// </summary>
        /// <typeparam name="T">strong type</typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> Query<T>(DataStorageContext context);

        /// <summary>
        /// 查询数据
        /// 根据实体<typeparamref name="T"/>查询,反射获取架构和字段信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> Query<T>() where T : class;

        /// <summary>
        /// 查询数据
        /// 根据实体<typeparamref name="T"/>查询，反射获取数据表架构和字段信息
        /// 根据<paramref name="condition"/>筛选数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">数据满足条件</param>
        /// <returns></returns>
        Task<IEnumerable<T>> Query<T>(Func<T, bool> condition) where T : class;

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection();

        /// <summary>
        /// 数据库对象是否存在
        /// </summary>
        /// <returns></returns>
        bool IsDatabaseObjectExists(string name);
    }
}
