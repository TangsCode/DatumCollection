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
            IDbConnection conn = GetConnection();
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
                                sql = $@"insert into {context.MainTable.Schema.TableName}
                                            ({string.Join(",", context.MainTable.Columns.Select(c => c.Name).ToArray())}) values
                                            ({string.Join(",", context.MainTable.Columns.Select(c => "@" + c.Name).ToArray())})";

                            }
                            break;
                        case Operation.InsertOrUpdate:
                            {
                                //if @parameters does not contain primary key column, insert data
                                //else update data contained in @parameters by primary key
                                if (context.Parameters[0]?.GetType()
                                    ?.GetProperty(context.MainTable.Columns.FirstOrDefault(c => c.IsPrimaryKey).Name)
                                    ?.GetValue(context.Parameters[0], null) != null)
                                {
                                    sql = $@"update {context.MainTable.Schema.TableName} set
                                            {string.Join(",", context.MainTable.Columns.Where(c => !c.IsPrimaryKey).Select(c => c.Name + "=@" + c.Name).ToArray())}
                                            where {string.Join(" and ", context.MainTable.Columns.Where(c => c.IsPrimaryKey).Select(c => c.Name + "=@" + c.Name).ToList())}";
                                }
                                else
                                {
                                    sql = $@"insert into {context.MainTable.Schema.TableName}
                                            ({string.Join(",", context.MainTable.Columns.Select(c => c.Name).ToArray())}) values
                                            ({string.Join(",", context.MainTable.Columns.Select(c => "@" + c.Name).ToArray())})";
                                }
                            }
                            break;
                        case Operation.Update:
                            {
                                sql = $@"update {context.MainTable.Schema.TableName} set
                                            {string.Join(",", context.MainTable.Columns.Where(c => !c.IsPrimaryKey).Select(c => c.Name + "=@" + c.Name).ToArray())}
                                            where {string.Join(" and ", context.MainTable.Columns.Where(c => c.IsPrimaryKey).Select(c => c.Name + "=@" + c.Name).ToList())}";
                            }
                            break;
                        case Operation.Delete:
                            {
                                sql = $@"delete from {context.MainTable.Schema.TableName} where {string.Join(" and ", context.MainTable.Columns.Where(c => c.IsPrimaryKey).Select(c => c.Name + "=@" + c.Name).ToList())}";
                            }
                            break;
                        case Operation.Query:
                            {
                                sql = $@"select * from {context.MainTable.Schema.TableName} where {string.Join(",", context.Parameters[0]?.GetType().GetProperties().Select(p => p.Name + "=@" + p.Name).ToArray())}";
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
                }
            }

            return result;
        }

        public async Task<IEnumerable<T>> Query<T>(DataStorageContext context)
        {
            IDbConnection conn = GetConnection();
            using (conn)
            {
                IDbTransaction transaction = conn.BeginTransaction();
                IEnumerable<T> result = null;
                try
                {
                    string sql = $@"select * from {context.MainTable.Schema.TableName} where {string.Join(",", context.Parameters[0]?.GetType().GetProperties().Select(p => p.Name + "=@" + p.Name).ToArray())}";
                    if (context.UseQueryString) { sql = context.QueryString; }
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

        public IDbConnection GetConnection()
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
        
    }
}
