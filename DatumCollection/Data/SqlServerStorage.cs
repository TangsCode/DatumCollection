using Dapper;
using DatumCollection.Data.Entity;
using DatumCollection.EventBus;
using DatumCollection.Exceptions;
using DatumCollection.Utility.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatumCollection.Data
{
    /// <summary>
    /// Microsoft Sql Server Storage
    /// </summary>
    public class SqlServerStorage : IDataStorage
    {
        /// <summary>
        /// 消息队列
        /// </summary>
        protected readonly IEventBus _eventBus;

        /// <summary>
        /// 日志接口
        /// </summary>
        protected readonly ILogger<SqlServerStorage> _logger;

        /// <summary>
        /// 系统选项
        /// </summary>
        protected readonly SystemOptions _options;

        public SqlServerStorage(IEventBus eventBus,ILogger<SqlServerStorage> logger,SystemOptions options)
        {
            _eventBus = eventBus;
            _logger = logger;
            _options = options;            
        }

        /// <summary>
        /// 增删改操作
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public int Execute(SqlContext context)
        {
            //create new connection
            IDbConnection conn = GetDbConnection();
            using (conn)
            {
                IDbTransaction transaction = null;
                try
                {
                    transaction = conn.BeginTransaction();
                    string sql = null;
                    object param = context.Properties;
                    if (context.PrimaryKey.IsNull()) { context.PrimaryKey = "ID"; }
                    IDictionary<string, object> propertyValues = (IDictionary<string, object>)param;
                    switch (context.Operation)
                    {
                        case Operation.Insert:
                            {
                                sql = $@"insert into {context.TableName}
                                        ({string.Join(',', propertyValues.Keys)}) 
                                        values(@{string.Join(",@", propertyValues.Keys)})";
                            }
                            break;
                        case Operation.Update:
                            {                                
                                sql = $@"update {context.TableName}
                                        set {string.Join(',', propertyValues.Keys
                                        .Where(k => k != context.PrimaryKey)
                                        .Select(k => k + "=@" + k))}
                                        where {context.PrimaryKey} = @{context.PrimaryKey}";                                
                            }
                            break;
                        case Operation.Delete:
                            {
                                sql = $@"delete {context.TableName}
                                        where {context.PrimaryKey} = @{context.PrimaryKey}";
                            }
                            break;
                        case Operation.Query:
                            {
                                sql = $@"select * from {context.TableName}
                                        where {string.Join(',', propertyValues.Keys.Select(k => k + "=@" + k))}";
                            }
                            break;
                    }
                    _logger.LogDebug(sql);
                    int count = conn.Execute(sql, param, transaction);                    
                    transaction?.Commit();
                    return count;
                }
                catch (Exception e)
                {
                    transaction?.Rollback();
                    _logger?.LogError(e.ToString());
                    return 0;
                }
                finally
                {
                    transaction?.Dispose();
                }
            }
        }

        public async Task<int> ExecuteAsync(SqlContext context)
        {
            //create new connection
            IDbConnection conn = GetDbConnection();
            using (conn)
            {
                IDbTransaction transaction = null;
                try
                {
                    transaction = conn.BeginTransaction();
                    string sql = null;
                    object param = context.Properties;
                    if (context.PrimaryKey.IsNull()) { context.PrimaryKey = "ID"; }
                    IDictionary<string, object> propertyValues = (IDictionary<string, object>)param;
                    if (!propertyValues.ContainsKey(context.PrimaryKey) && context.Operation == Operation.Query)
                    {
                        throw new SpiderException("context fields does not contain primary key");
                    }
                    switch (context.Operation)
                    {
                        case Operation.Insert:
                            {
                                sql = $@"insert into {context.TableName}
                                        ({string.Join(',', propertyValues.Keys)}) 
                                        values(@{string.Join(",@", propertyValues.Keys)})";
                            }
                            break;
                        case Operation.Update:
                            {
                                sql = $@"update {context.TableName}
                                        set {string.Join(',', propertyValues.Keys
                                        .Where(k => k != context.PrimaryKey)
                                        .Select(k => k + "=@" + k))}
                                        where {context.PrimaryKey} = @{context.PrimaryKey}";
                            }
                            break;
                        case Operation.Delete:
                            {
                                sql = $@"delete {context.TableName}
                                        where {context.PrimaryKey} = @{context.PrimaryKey}";
                            }
                            break;
                        //case DataManpulation.Query:
                        //    {
                        //        sql = $@"select * from {context.TableName}
                        //                where {string.Join(',', propertyValues.Keys.Select(k => k + "=@" + k))}";
                        //    }
                        //    break;
                    }
                    int count = await conn.ExecuteAsync(sql, param, transaction);
                    transaction?.Commit();
                    return count;
                }
                catch (Exception e)
                {
                    transaction?.Rollback();
                    _logger?.LogError(e.ToString());
                    return 0;
                }
                finally
                {
                    transaction?.Dispose();
                }
            }
        }

        public int ExecuteSql(string sql, object properties = null)
        {
            IDbConnection conn = GetDbConnection();
            using (conn)
            {
                IDbTransaction transaction = null;
                try
                {
                    transaction = conn.BeginTransaction();
                    int count = conn.Execute(sql, properties, transaction);
                    transaction?.Commit();
                    return count;
                }
                catch (Exception e)
                {
                    transaction?.Rollback();
                    _logger?.LogError(e.ToString());
                    return 0;
                }
                finally
                {
                    transaction?.Dispose();
                }
            }
        }

        public async Task<int> ExecuteSqlAsync(string sql, object properties = null)
        {
            IDbConnection conn = GetDbConnection();
            using (conn)
            {
                IDbTransaction transaction = null;
                try
                {
                    transaction = conn.BeginTransaction();
                    int count = await conn.ExecuteAsync(sql, properties, transaction);
                    transaction?.Commit();
                    return count;
                }
                catch (Exception e)
                {
                    transaction?.Rollback();
                    _logger?.LogError(e.ToString());
                    return 0;
                }
                finally
                {
                    transaction?.Dispose();
                }
            }

        }
        public IDbConnection GetDbConnection()
        {
            try
            {                
                IDbConnection conn = new SqlConnection(_options.StorageConnectionString);
                conn.Open();
                return conn;
            }
            catch (Exception e)
            {
                _logger?.LogError(e.ToString());
                return null;
            }
        }

        public IEnumerable<TEntity> Query<TEntity>(SqlContext context)
        {
            IDbConnection conn = GetDbConnection();
            using (conn)
            {                              
                try
                {
                    StringBuilder sql = new StringBuilder();
                    string queryFields = context.QueryFields.Count > 0
                        ? string.Join(',', context.QueryFields.Select(
                            (f) =>
                        {
                            string aliasTableName = f.TableName == context.TableName
                            ? context.AliasName
                            : context.JoinTables.Where(t => t.Name == f.TableName).FirstOrDefault().AliasName;
                            return $"{aliasTableName}.{f.Name}";
                        })) : "*";
                    sql.AppendLine($"select {queryFields} from {context.TableName}");
                    if(context.JoinTables.Count > 0)
                    {
                        context.AliasName = context.AliasName;
                        sql.Append($" {context.AliasName}");
                        foreach (var table in context.JoinTables)
                        {
                            sql.AppendLine($@" {table.JoinType.ToString().ToLower()} join {table.Name} {table.AliasName}
                                                      {table.JoinCondition}");                            
                        }
                    }
                    if (context.WhereClause.NotNull())
                    {
                        sql.AppendLine($" where {context.WhereClause}");
                    }
                    IEnumerable<TEntity> result = conn.Query<TEntity>(sql.ToString(), context.Properties);
                    return result;
                }
                catch (Exception e)
                {                    
                    _logger?.LogError(e.ToString());
                    return null;
                }
            }
        }

    }
}
