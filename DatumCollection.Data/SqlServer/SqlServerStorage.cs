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
using DatumCollection.Infrastructure.Data;
using System.Reflection;
using DatumCollection.Data.Entities;
using System.Collections.Concurrent;

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
            IDbConnection conn = GetDbConnection();
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
                    }
                                        
                    result.RowsAffected = await conn.ExecuteAsync(sql, context.Parameters, transaction);
                    transaction?.Commit();                    
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    transaction?.Rollback();
                    result.Success = false;
                    result.ErrorMsg = e.Message;
                }
            }

            return result;
        }

        public async Task<T> ExecuteScalarAsync<T>(DataStorageContext context)
        {
            IDbConnection conn = GetDbConnection();
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

        public async Task<IEnumerable<dynamic>> QueryDynamicList<T>()
        {
            var metadata = await GetMetaData<T>();
            var context = new DataStorageContext(metadata);
            IDbConnection conn = GetDbConnection();
            using (conn)
            {
                IDbTransaction transaction = null;
                IEnumerable<dynamic> result = null;
                try
                {
                    transaction = conn.BeginTransaction();
                    StringBuilder sql = new StringBuilder();
                    StringBuilder selectedColumns = new StringBuilder();
                    selectedColumns.Append(string.Join(",", metadata.Columns.Select(p => string.Concat(context.MainTable.AliasName, ".", p.Name)).ToArray()));
                    sql.AppendLine(string.Format("select * from {0} {1}", context.MainTable.TableName, context.MainTable.AliasName));
                    foreach (var relation in context.JoinRelations)
                    {
                        sql.AppendLine($" {relation.JoinType.ToString().ToLower()} join {relation.Right.TableName} {relation.Right.AliasName} on {relation.Right.AliasName}.{relation.Right.JoinKey}={relation.Left.AliasName}.{relation.Left.JoinKey}");                        
                        selectedColumns.Append("," + string.Join(",", relation.RelationMetadata.Columns.Select(p => string.Concat(relation.Right.AliasName, ".", p.Name) + $" as {relation.Right.AliasName}_{p.Name}").ToArray()));
                    }
                    string executeSql = sql.Replace("*", selectedColumns.ToString()).ToString();
                    result = await conn.QueryAsync(executeSql, transaction: transaction);
                }
                catch (Exception e)
                {
                    _logger?.LogError(e.ToString());
                    transaction?.Rollback();
                }
                return result;
            }
        }

        private ConcurrentDictionary<Type, PropertyInfo> _resultProp = new ConcurrentDictionary<Type, PropertyInfo>();
        private ConcurrentDictionary<Type, MethodInfo> _methods = new ConcurrentDictionary<Type, MethodInfo>();

        public async Task<IEnumerable<T>> RecursiveQuery<T>(object param = null, int depth = 1) where T: class
        {
            var metadata = await GetMetaData<T>();
            var context = new DataStorageContext(metadata);
            IDbConnection conn = GetDbConnection();
            using (conn)
            {
                IDbTransaction transaction = null;
                IEnumerable<T> result = null;
                try
                {
                    transaction = conn.BeginTransaction();                    
                    string maintableSql = $"select * from {context.MainTable.TableName}";
                    result = await conn.QueryAsync<T>(maintableSql, param, transaction: transaction);
                    if (metadata.RelationObjects.Any() && depth > 0)
                    {                        
                        //if any property is class
                        foreach (var relation in metadata.RelationObjects)
                        {
                            var prop = relation.RelationProperty;
                            MethodInfo method;
                            PropertyInfo resultPropInfo;
                            var fkprop = result.FirstOrDefault().GetType().GetProperty(relation.JoinTable.ProviderKey);
                            if (_methods.ContainsKey(prop.PropertyType)
                                ||(prop.PropertyType.IsGenericType &&
                                _methods.ContainsKey(prop.PropertyType.GetGenericArguments()[0])))
                            {
                                method = prop.PropertyType.IsGenericType
                                    ? _methods[prop.PropertyType.GetGenericArguments()[0]]
                                    : _methods[prop.PropertyType];
                            }
                            else
                            {
                                method = prop.PropertyType.IsGenericType
                                    ? this.GetType().GetMethod("RecursiveQuery").MakeGenericMethod(prop.PropertyType.GetGenericArguments()[0])
                                    : this.GetType().GetMethod("RecursiveQuery").MakeGenericMethod(prop.PropertyType);
                                
                                _methods.TryAdd(prop.PropertyType.IsGenericType 
                                    ? prop.PropertyType.GetGenericArguments()[0]
                                    : prop.PropertyType, method);
                            }
                            for (int i = 0; i < result.Count(); i++)
                            {
                                var queryParam = new Dictionary<string, object>();
                                queryParam[relation.JoinTable.ForeignKey] = fkprop.GetValue(result.ElementAt(i));
                                Task task = (Task)method.Invoke(this, new object[] { queryParam, depth - 1 });
                                task.Wait();
                                if (_resultProp.ContainsKey(task.GetType()))
                                {
                                    resultPropInfo = _resultProp[task.GetType()];
                                }
                                else
                                {
                                    resultPropInfo = task.GetType().GetProperty("Result");
                                    _resultProp.TryAdd(task.GetType(), resultPropInfo);
                                }
                                
                                prop.SetValue(result.ElementAt(i), prop.PropertyType.IsGenericType ?
                                    resultPropInfo.GetValue(task) 
                                    : ((IEnumerable<object>)resultPropInfo.GetValue(task)).FirstOrDefault());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger?.LogError(e.ToString());
                    transaction?.Rollback();
                }
                return result;
            }
        }

        public async Task<IEnumerable<object>> RecursiveQueryWithType(Type type, object param = null, int depth = 1)
        {
            var metadata = await GetMetaData<int>(type);
            var context = new DataStorageContext(metadata);
            IDbConnection conn = GetDbConnection();
            using (conn)
            {
                IDbTransaction transaction = null;
                IEnumerable<object> result = null;
                try
                {
                    transaction = conn.BeginTransaction();
                    string maintableSql = $"select * from {context.MainTable.TableName}";
                    result = await conn.QueryAsync(type, maintableSql, param, transaction: transaction);
                    if (metadata.RelationObjects.Any() && depth > 0)
                    {
                        depth--;
                        //if any property is class
                        foreach (var relation in metadata.RelationObjects)
                        {
                            var prop = relation.RelationProperty;
                            var fkprop = result.FirstOrDefault().GetType().GetProperty(relation.JoinTable.ProviderKey);
                            for (int i = 0; i < result.Count(); i++)
                            {
                                var queryParam = new Dictionary<string, object>();
                                queryParam[relation.JoinTable.ForeignKey] = fkprop.GetValue(result.ElementAt(i));
                                var propValue = await RecursiveQueryWithType(prop.PropertyType.IsGenericType
                                    ? prop.PropertyType.GetGenericArguments()[0]
                                    : prop.PropertyType, queryParam, depth);

                                prop.SetValue(result.ElementAt(i), prop.PropertyType.IsGenericType 
                                    ? propValue
                                    : propValue.FirstOrDefault());
                            }
                        }
                    }                    
                }
                catch (Exception e)
                {
                    _logger?.LogError(e.ToString());
                    transaction?.Rollback();
                }
                return result;
            }
        }

        public async Task<IEnumerable<T>> Query<T>(Func<T, bool> condition = null) where T : class
        {
            var metadata = await GetMetaData<T>();
            IDbConnection conn = GetDbConnection();
            using (conn)
            {
                IDbTransaction transaction = null;
                IEnumerable<T> result = null;
                try
                {
                    transaction = conn.BeginTransaction();
                    var aliasName = metadata.Schema.TableName.FirstOrDefault().ToString().ToLower();
                    StringBuilder sql = new StringBuilder();
                    StringBuilder column = new StringBuilder();
                    column.Append(string.Join(",", metadata.Columns.Select(p => string.Concat(aliasName, ".", p.Name)).ToArray()));
                    sql.AppendLine($@"select * from {metadata.Schema.TableName} {aliasName}");
                    if (metadata.RelationObjects.Any())
                    {
                        foreach (var relation in metadata.RelationObjects)
                        {
                            var relationAliasName = relation.MetaData.Schema.TableName.ToLower().FirstOrDefault(c => c.ToString() != aliasName).ToString();
                            column.Append("," + string.Join(",", relation.MetaData.Columns.Select(p => string.Concat(relationAliasName, ".", p.Name)).ToArray()));
                            sql.AppendLine($" {relation.JoinTable.JoinType.ToString().ToLower()} join {relation.MetaData.Schema.TableName} {relationAliasName} on {aliasName}.{relation.JoinTable.ProviderKey}={relationAliasName}.{relation.JoinTable.ForeignKey}");
                        }
                    }
                    sql.Replace("*", column.ToString());
                    result = await conn.QueryAsync<T>(sql.ToString(), null, transaction);
                    if (condition != null)
                    {
                        result = result.Where(e => condition(e));
                    }
                }
                catch (Exception e)
                {
                    _logger?.LogError(e.ToString());
                    transaction?.Rollback();
                }
                return result;
            }
        }
        public async Task<IEnumerable<TFirst>> Query<TFirst, TSecond>(
            Func<TFirst, TSecond, TFirst> map,
            Func<TFirst, bool> condition = null) where TFirst : class where TSecond: class
        {
            var metadata = await GetMetaData<TFirst>();
            var context = new DataStorageContext(metadata);
            IDbConnection conn = GetDbConnection();
            using (conn)
            {
                IDbTransaction transaction = null;
                IEnumerable<TFirst> result = null;
                try
                {
                    transaction = conn.BeginTransaction();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(string.Format("select * from {0} {1}", context.MainTable.TableName, context.MainTable.AliasName));
                    foreach (var relation in context.JoinRelations)
                    {
                        sql.AppendLine($" {relation.JoinType.ToString().ToLower()} join {relation.Right.TableName} {relation.Right.AliasName} on {relation.Right.AliasName}.{relation.Right.JoinKey}={relation.Left.AliasName}.{relation.Left.JoinKey}");
                    }
                    result = await conn.QueryAsync(sql.ToString(), map, transaction: transaction);
                    if (condition != null)
                    {
                        result = result.Where(d => condition(d));
                    }
                }
                catch (Exception e)
                {
                    _logger?.LogError(e.ToString());
                }
                return result;
            }
        }

        public async Task<IEnumerable<TFirst>> Query<TFirst, TSecond, TThird>(Func<TFirst, TSecond, TThird, TFirst> map, Func<TFirst, bool> condition = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
        {
            var metadata = await GetMetaData<TFirst>();
            var context = new DataStorageContext(metadata);
            IDbConnection conn = GetDbConnection();
            using (conn)
            {
                IDbTransaction transaction = null;
                IEnumerable<TFirst> result = null;
                try
                {
                    transaction = conn.BeginTransaction();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(string.Format("select * from {0} {1}", context.MainTable.TableName, context.MainTable.AliasName));
                    foreach (var relation in context.JoinRelations)
                    {
                        sql.AppendLine($" {relation.JoinType.ToString().ToLower()} join {relation.Right.TableName} {relation.Right.AliasName} on {relation.Right.AliasName}.{relation.Right.JoinKey}={relation.Left.AliasName}.{relation.Left.JoinKey}");
                    }
                    result = await conn.QueryAsync(sql.ToString(), map, transaction: transaction);
                    if (condition != null)
                    {
                        result = result.Where(d => condition(d));
                    }
                }
                catch (Exception e)
                {
                    _logger?.LogError(e.ToString());
                    transaction?.Rollback();
                }
                return result;
            }
        }

        #region entity operation
        public async Task<DbExecutionResult> Insert<T>(IEnumerable<T> entities)
        {
            var result = new DbExecutionResult();
            try
            {
                var metadata = await GetMetaData<T>(entities.FirstOrDefault().GetType());
                var context = new DataStorageContext
                {
                    Metadata = metadata,
                    Operation = Operation.Insert,
                    Parameters = entities
                };

                result = await ExecuteAsync(context);                
            }
            catch (Exception e)
            {
                _logger.LogError("error occured in inserting entities with {storage}\r\nmessage:{error}", nameof(SqlServerStorage), e.Message);
                result.Success = false;
            }
            return result;
        }

        public async Task<DbExecutionResult> Update<T>(T entity)
        {
            var result = new DbExecutionResult();
            try
            {
                var metadata = await GetMetaData<T>(entity.GetType());
                var context = new DataStorageContext
                {
                    Metadata = metadata,
                    Operation = Operation.Update,
                    Parameters = entity
                };

                result = await ExecuteAsync(context);
            }
            catch (Exception e)
            {
                _logger.LogError("error occured in updating entity with {storage}\r\nmessage:{error}", nameof(SqlServerStorage), e.Message);
                result.Success = false;
            }
            return result;
        }

        public async Task<DbExecutionResult> Delete<T>(T entity)
        {
            var result = new DbExecutionResult();
            try
            {
                var metadata = await GetMetaData<T>(entity.GetType());
                var context = new DataStorageContext
                {
                    Metadata = metadata,
                    Operation = Operation.Delete,
                    Parameters = entity
                };

                result = await ExecuteAsync(context);
            }
            catch (Exception e)
            {
                _logger.LogError("error occured in deleting entity with {storage}\r\nmessage:{error}", nameof(SqlServerStorage), e.Message);
                result.Success = false;
            }
            return result;
        }

        #endregion

        private ConcurrentDictionary<Type, DatabaseMetadata> _metadataCollection = new ConcurrentDictionary<Type, DatabaseMetadata>();
        private async Task<DatabaseMetadata> GetMetaData<T>(Type specifiedType = null)
        {
            
            var type = specifiedType ?? typeof(T);
            if (_metadataCollection.ContainsKey(type))
            {
                return _metadataCollection[type];
            }
            var metadata = new DatabaseMetadata();
            var classAttribute = System.Attribute.GetCustomAttributes(type);
            metadata.Schema = (SchemaAttribute)classAttribute.FirstOrDefault(a => a.GetType() == typeof(SchemaAttribute));
            var properties = type.GetProperties().OrderByDescending(p => p.Name.ToLower() == "id");
            foreach (var property in properties)
            {
                if (property.GetCustomAttribute(typeof(ColumnAttribute), false) != null)
                {
                    var column = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute), false);
                    //column.PropertyInfo = property;
                    metadata.Columns.Add(column);
                }
                if(property.GetCustomAttribute(typeof(JoinTableAttribute),false) != null)
                {
                    var relation = new RelationObject
                    {                        
                        JoinTable = (JoinTableAttribute)property.GetCustomAttribute(typeof(JoinTableAttribute), false),
                        RelationProperty = property
                    };
                    if (property.PropertyType.IsGenericType)
                    {
                        relation.MetaData = await GetMetaData<T>(property.PropertyType.GetGenericArguments()[0]);
                    }
                    else
                    {
                        relation.MetaData = await GetMetaData<T>(property.PropertyType);
                    }
                    metadata.RelationObjects.Add(relation);
                }
            }
            _metadataCollection.TryAdd(type, metadata);

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
                        case "float":
                        case "bit":
                        case "date":
                        case "time":
                        case "datetime":
                        case "smalldatetime":
                        case "uniqueidentifier":
                            columnSql.AppendLine($",[{column.Name}] {column.Type} {(column.Required ? "not" : "")} null");
                            break;
                        case "decimal":
                        case "numeric":
                            columnSql.AppendLine($",[{column.Name}] {column.Type}({column.Precision},{column.Scale}) {(column.Required ? "not" : "")} null");
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

        public IDbConnection GetDbConnection()
        {
            try
            {
                IDbConnection conn = new SqlConnection(_config.ConnectionString);                
                conn.Open();
                return conn;
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "get connection failed with storage {storageType}", nameof(SqlServerStorage));
                return null;
            }
        }
    }
}
