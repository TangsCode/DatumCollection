using DatumCollection.EventBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
    public interface IDataStorage
    {

        /// <summary>
        /// 同步执行
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        int Execute(SqlContext context);

        /// <summary>
        /// 异步执行
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task<int> ExecuteAsync(SqlContext context);

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int ExecuteSql(string sql, object properties = null);

        /// <summary>
        /// 异步执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        Task<int> ExecuteSqlAsync(string sql, object properties = null);

        /// <summary>
        /// 查询数据
        /// 1.根据上下文提供的表名和属性筛选单表行数据
        /// 2.若上下文WhereClause不为空，则添加查询条件
        /// 3.查询字段由QueryFields属性决定，未设置查询全字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(SqlContext context);

        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <returns></returns>
        IDbConnection GetDbConnection();
    }
}
