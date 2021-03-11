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
        /// 查询实体
        /// 反射实体<typeparamref name="T"/>获取数据表架构和字段信息
        /// 根据<paramref name="condition"/>筛选数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">数据满足条件</param>
        /// <returns></returns>
        Task<IEnumerable<T>> Query<T>(Func<T, bool> condition = null) where T : class;
        
        /// <summary>
        /// 递归查询
        /// 反射实体<typeparamref name="T"/>获取数据表架构和字段属性，
        /// 属性为实体则继续递归查询，直至所有实体属性全部查询完毕。
        /// 根据<paramref name="condition"/>筛选数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">数据满足条件</param>
        /// <returns></returns>
        Task<IEnumerable<T>> RecursiveQuery<T>(object param = null, int depth = 0) where T: class;

        /// <summary>
        /// 递归查询
        /// 根据实体类型对class属性递归查询，直至所有实体属性全部查询完毕或达到递归深度。
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="param">查询参数</param>
        /// <param name="depth">递归深度</param>
        /// <returns></returns>
        Task<IEnumerable<object>> RecursiveQueryWithType(Type type, object param = null, int depth = 1);

        /// <summary>
        /// 查询动态列表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<IEnumerable<dynamic>> QueryDynamicList<T>();

        /// <summary>        
        /// 关联实体查询（实体层级2）
        /// </summary>
        /// <typeparam name="TFirst">实体1</typeparam>
        /// <typeparam name="TSecond">实体2</typeparam>
        /// <param name="map">实体关系</param>
        /// <param name="condition">条件筛选</param>
        /// <returns></returns>
        Task<IEnumerable<TFirst>> Query<TFirst, TSecond>(
            Func<TFirst, TSecond, TFirst> map,
            Func<TFirst, bool> condition = null) where TFirst : class where TSecond : class;

        /// <summary>
        /// 关联实体查询（实体层级3）
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <param name="map"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        Task<IEnumerable<TFirst>> Query<TFirst, TSecond, TThird>(
            Func<TFirst, TSecond, TThird, TFirst> map,
            Func<TFirst, bool> condition = null) where TFirst : class where TSecond : class where TThird : class;

        #region entity operation
        /// <summary>
        /// 插入实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<DbExecutionResult> Insert<T>(IEnumerable<T> entity);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<DbExecutionResult> Update<T>(object entity);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<DbExecutionResult> Delete<T>(T entity);

        #endregion

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        IDbConnection GetDbConnection();

        /// <summary>
        /// 数据库对象是否存在
        /// </summary>
        /// <returns></returns>
        bool IsDatabaseObjectExists(string name);
    }
}
