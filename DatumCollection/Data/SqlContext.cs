using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace DatumCollection.Data
{
    /// <summary>
    /// SQL上下文
    /// </summary>
    public class SqlContext
    {        
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; internal set; }

        /// <summary>
        /// 主表别名
        /// 在关联其他表时需要
        /// </summary>
        public string AliasName { get; set; }

        /// <summary>
        /// 关联表
        /// </summary>
        public HashSet<JoinTable> JoinTables { get; set; }

        /// <summary>
        /// 属性
        /// 可用于查询时筛选字段，更新时更新字段
        /// </summary>
        public object Properties { get;internal set; }

        /// <summary>
        /// 主键
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        /// Where子句
        /// </summary>
        public string WhereClause { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public Operation Operation { get;internal set; }

        public HashSet<QueryField> QueryFields { get; set; }

        public SqlContext(
            string tableName, object properties = null, string aliasName = null,
            Operation operation = Operation.Query)
        {
            TableName = tableName;
            Properties = properties ?? new ExpandoObject();
            AliasName = aliasName;
            Operation = operation;
            JoinTables = new HashSet<JoinTable>();
            QueryFields = new HashSet<QueryField>();
        }

        public SqlContext AddJoinTable(JoinTable joinTable)
        {
            JoinTables.Add(joinTable);
            
            return this;
        }

        public SqlContext AddProperty(string key,object value)
        {
            var properties = (IDictionary<string, object>)Properties;
            properties.Add(key, value);

            return this;
        }

        public SqlContext AddQueryField(string tableName,string[] fields)
        {
            if(TableName != tableName && JoinTables.Where(t => t.Name == tableName) == null)
            {
                throw new Exception("查询字段未定义表名或表名错误");
            }
            foreach (var field in fields)
            {
                QueryFields.Add(new QueryField(tableName, field, null));
            }            

            return this;
        }

        public SqlContext UpdateQueryFieldAliasName(string fieldName,string aliasName)
        {
            if(QueryFields.Where(f => f.Name == fieldName) == null)
            {
                throw new Exception($"字段{fieldName}不存在");
            }
            if (string.IsNullOrEmpty(aliasName))
            {
                throw new Exception("字段别名不能为空");
            }
            QueryFields.Where(f => f.Name == fieldName).FirstOrDefault().AliasName = aliasName;

            return this;
        }
    }

}
