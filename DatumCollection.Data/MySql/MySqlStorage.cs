using DatumCollection.Configuration;
using DatumCollection.Data.SqlServer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace DatumCollection.Data.MySql
{
    public class MySqlStorage : IDataStorage
    {

        protected readonly ILogger<SqlServerStorage> _logger;

        protected readonly SpiderClientConfiguration _config;

        public MySqlStorage(
            ILogger<SqlServerStorage> logger,
            SpiderClientConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public void Dispose()
        {
            
        }

        public async Task<DbExecutionResult> ExecuteAsync(DataStorageContext context)
        {
            IDbConnection conn = GetDbConnection();
            DbExecutionResult result = new DbExecutionResult();
            using (conn)
            {
                IDbTransaction transaction = conn.BeginTransaction();
                try
                {
                    string sql = null;
                    switch (context.Operation)
                    {
                        case Operation.Insert:
                            {
                                sql = $@"insert into {context.Metadata.Schema.TableName}
                                            ({string.Join(",", context.Metadata.Columns.Select(c => c.Name).ToArray())}) values
                                            ({string.Join(",", context.Metadata.Columns.Select(c => "@" + c.Name).ToArray())})";

                            }
                            break;
                        case Operation.InsertOrUpdate:
                            {
                                //if @parameters does not contain primary key column, insert data
                                //else update data contained in @parameters by primary key
                                if (context.Parameters?.GetType()
                                    ?.GetProperty(context.Metadata.Columns.FirstOrDefault(c => c.IsPrimaryKey).Name)
                                    ?.GetValue(context.Parameters, null) != null)
                                {
                                    sql = $@"update {context.Metadata.Schema.TableName} set
                                            {string.Join(",", context.Metadata.Columns.Where(c => !c.IsPrimaryKey).Select(c => c.Name + "=@" + c.Name).ToArray())}
                                            where {string.Join(" and ", context.Metadata.Columns.Where(c => c.IsPrimaryKey).Select(c => c.Name + "=@" + c.Name).ToList())}";
                                }
                                else
                                {
                                    sql = $@"insert into {context.Metadata.Schema.TableName}
                                            ({string.Join(",", context.Metadata.Columns.Select(c => c.Name).ToArray())}) values
                                            ({string.Join(",", context.Metadata.Columns.Select(c => "@" + c.Name).ToArray())})";
                                }
                            }
                            break;
                        case Operation.Update:
                            {
                                sql = $@"update {context.Metadata.Schema.TableName} set
                                            {string.Join(",", context.Metadata.Columns.Where(c => !c.IsPrimaryKey).Select(c => c.Name + "=@" + c.Name).ToArray())}
                                            where {string.Join(" and ", context.Metadata.Columns.Where(c => c.IsPrimaryKey).Select(c => c.Name + "=@" + c.Name).ToList())}";
                            }
                            break;
                        case Operation.Delete:
                            {
                                sql = $@"delete from {context.Metadata.Schema.TableName} where {string.Join(" and ", context.Metadata.Columns.Where(c => c.IsPrimaryKey).Select(c => c.Name + "=@" + c.Name).ToList())}";
                            }
                            break;
                        case Operation.Query:
                            {
                                sql = $@"select * from {context.Metadata.Schema.TableName} where {string.Join(",", context.Parameters?.GetType().GetProperties().Select(p => p.Name + "=@" + p.Name).ToArray())}";
                            }
                            break;
                        default:
                            break;
                    }
                    result.RowsAffected = await conn.ExecuteAsync(sql, context.Parameters, transaction);
                    transaction?.Commit();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    transaction?.Rollback();
                    result.Success = false;
                }
            }

            return result;
        }

        public Task<T> ExecuteScalarAsync<T>(DataStorageContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> Query<T>(DataStorageContext context)
        {
            IDbConnection conn = GetDbConnection();
            using (conn)
            {
                IDbTransaction transaction = conn.BeginTransaction();
                IEnumerable<T> result = null;
                try
                {
                    string sql = $@"select * from {context.Metadata.Schema.TableName} where {string.Join(",", context.Parameters?.GetType().GetProperties().Select(p => p.Name + "=@" + p.Name).ToArray())}";
                    if (context.UseSqlStatement) { sql = context.SqlStatement; }
                    result = await conn.QueryAsync<T>(sql, context.Parameters, transaction);
                }
                catch (Exception e)
                {
                    _logger?.LogError(e.ToString());
                    transaction?.Rollback();
                }
                return result;
            }
        }

        public Task<IEnumerable<T>> Query<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> Query<T>(Func<T, bool> condition) where T : class
        {
            throw new NotImplementedException();
        }

        public IDbConnection GetDbConnection()
        {
            try
            {
                IDbConnection conn = new MySqlConnection(_config.ConnectionString);
                conn.Open();
                return conn;
            }
            catch (Exception e)
            {
                _logger.LogError(string.Format("databases get connection failed:{0}", e.ToString()));               
            }
            return null;
        }

        public bool IsDatabaseObjectExists(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TFirst>> Query<TFirst, TSecond>(Func<TFirst, TSecond, TFirst> map, Func<TFirst, bool> condition = null) where TFirst : class where TSecond: class
        {
            throw new NotImplementedException();
        }

        public Task<DbExecutionResult> Insert<T>(IEnumerable<T> entity)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<TFirst>> Query<TFirst, TSecond, TThird>(Func<TFirst, TSecond, TThird, TFirst> map, Func<TFirst, bool> condition = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
        {
            throw new NotImplementedException();
        }

        public Task<DbExecutionResult> Delete<T>(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<dynamic>> QueryDynamicList<T>()
        {
            throw new NotImplementedException();
        }
         
        public Task<IEnumerable<T>> RecursiveQuery<T>(object param = null, int depth = 0) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>> RecursiveQueryWithType(Type type, object param = null, int depth = 1)
        {
            throw new NotImplementedException();
        }

        public Task<DbExecutionResult> Update<T>(object entity)
        {
            throw new NotImplementedException();
        }
    }
}
