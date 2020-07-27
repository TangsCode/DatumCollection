using Dapper;
using DatumCollection.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using DatumCollection.Data.Attributes;

namespace DatumCollection.Data.SqlServer
{
    public class SqlServerStorage : IDataStorage
    {
        protected readonly ILogger<SqlServerStorage> _logger;

        protected readonly SpiderClientConfiguration _config;

        public SqlServerStorage(
            ILogger<SqlServerStorage> logger,
            SpiderClientConfiguration config
            )
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
                IDbTransaction transaction = null;
                try
                {
                    transaction = conn.BeginTransaction(); 
                    string sql = null;
                    if (context.UseSqlStatement)
                    {
                        sql = context.SqlStatement;
                    }
                    else
                    {
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
                                    if (context.Parameters?.GetType()
                                        ?.GetProperty(context.MainTable.Columns.FirstOrDefault(c => c.IsPrimaryKey).Name)
                                        ?.GetValue(context.Parameters, null) != null)
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
                                    sql = $@"select * from {context.MainTable.Schema.TableName} where {string.Join(",", context.Parameters?.GetType().GetProperties().Select(p => p.Name + "=@" + p.Name).ToArray())}";
                                }
                                break;
                            default:
                                break;
                        }
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


        public async Task<T> ExecuteScalarAsync<T>(DataStorageContext context)
        {
            IDbConnection conn = GetConnection();
            T result = default(T);
            using (conn)
            {
                IDbTransaction transaction = null;
                try
                {
                    transaction = conn.BeginTransaction();
                    result = await conn.ExecuteScalarAsync<T>(context.SqlStatement, context.Parameters, transaction);
                    transaction?.Commit();
                }
                catch (Exception e)
                {
                    _logger?.LogError(e.ToString());
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
                    string sql = $@"select * from {context.MainTable.Schema.TableName} where {string.Join(",", context.Parameters?.GetType().GetProperties().Select(p => p.Name + "=@" + p.Name).ToArray())}";
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

        public IDbConnection GetConnection()
        {
            try
            {
                IDbConnection conn = new SqlConnection(_config.ConnectionString);
                conn.Open();
                return conn;
            }
            catch (Exception e)
            {
                _logger?.LogError(e,"get connection failed with storage {storageType}",nameof(SqlServerStorage));
                return null;
            }
        }
        
        //public async Task<IEnumerable<TFirst>> Query<TFirst,TSecond,TThird>(
        //    Func<TFirst,TSecond,TThird,TFirst> map,
        //    Func<TFirst,bool> condition) where TFirst: class
        //{
        //    var metadata = await GetMetaData<TFirst>();
        //    IDbConnection conn = GetConnection();
        //    using (conn)
        //    {
        //        IDbTransaction transaction = null;
        //        IEnumerable<TFirst> result = null;
        //        try
        //        {
        //            transaction = conn.BeginTransaction();
        //            string sql = $@"select {string.Join(",", metadata.Columns.Select(p => p.Name).ToArray())} from {metadata.Schema.TableName} ";
        //            result = await conn.QueryAsync(sql, map, transaction: transaction);
        //            result = result.Where(d => condition(d));
        //            //transaction?.Commit();
        //        }
        //        catch (Exception e)
        //        {
        //            _logger?.LogError(e.ToString());
        //            transaction?.Rollback();
        //        }
        //        return result;
        //    }
        //}

        public async Task<IEnumerable<T>> Query<T>() where T : class
        {
            var metadata = await GetMetaData<T>();
            IDbConnection conn = GetConnection();
            using (conn)
            {
                IDbTransaction transaction = null;
                IEnumerable<T> result = null;
                try
                {
                    transaction = conn.BeginTransaction();
                    string sql = $@"select {string.Join(",", metadata.Columns.Select(p => p.Name).ToArray())} from {metadata.Schema.TableName} ";
                    result = await conn.QueryAsync<T>(sql, null, transaction);                    
                    //transaction?.Commit();
                }
                catch (Exception e)
                {
                    _logger?.LogError(e.ToString());
                    transaction?.Rollback();
                }
                return result;
            }
        }

        public async Task<IEnumerable<T>> Query<T>(Func<T, bool> condition) where T : class
        {
            var data = await Query<T>();
            return data?.Where(condition);
        }

        private async Task<DatabaseMetadata> GetMetaData<T>()
        {
            var metadata = new DatabaseMetadata();
            var classAttribute = System.Attribute.GetCustomAttributes(typeof(T));
            metadata.Schema = (SchemaAttribute)classAttribute.FirstOrDefault(a => a.GetType() == typeof(SchemaAttribute));
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.GetCustomAttributes(false) != null)
                {
                    metadata.Columns.Add((ColumnAttribute)property.GetCustomAttributes(false).FirstOrDefault());
                }
            }

            //ensure database object exists
            if (!IsDatabaseObjectExists(metadata.Schema.TableName) && _config.EnsureDatabaseObject)
            {
                StringBuilder createTableSql = new StringBuilder();
                StringBuilder columnSql = new StringBuilder();
                if (!string.IsNullOrEmpty(metadata.Schema.DataBase))
                {
                    createTableSql.AppendLine($"use [{metadata.Schema.DataBase}] go");
                }                
                createTableSql.AppendLine($"create table [{metadata.Schema.Schema}].[{metadata.Schema.TableName}](");
                foreach (var column in metadata.Columns)
                {
                    switch (column.Type)
                    {
                        case "char":
                        case "nchar":
                        case "varchar":
                        case "nvarchar":
                            columnSql.AppendLine($",[{column.Name}] {column.Type}({column.Length}) {(column.Required ? "not" : "")} null");
                            break;
                        case "int":
                        case "bit":
                        case "date":
                        case "time":
                        case "datetime":
                        case "smalldatetime":
                        case "uniqueidentifier":
                            columnSql.AppendLine($",[{column.Name}] {column.Type} {(column.Required ? "not" : "")} null");
                            break;                           
                        default:
                            break;
                    }                    
                }
                createTableSql.Append(columnSql.ToString().TrimStart(','));
                createTableSql.Append(")");

                var sqlContext = new DataStorageContext() { SqlStatement = createTableSql.ToString() };
                await ExecuteAsync(sqlContext);
            }

            return metadata;
        }

        public bool IsDatabaseObjectExists(string objectName)
        {
            var context = new DataStorageContext() { SqlStatement = $"select 1 from sysobjects where name = '{objectName}'"};
            var isExist = ExecuteScalarAsync<bool>(context).GetAwaiter().GetResult();
            return isExist;
        }
    }
}
