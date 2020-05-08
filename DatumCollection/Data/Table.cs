using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data
{
    /// <summary>
    /// 数据表
    /// </summary>
    public class Table
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string TableAliasName { get; set; }

        public HashSet<QueryField> PrimaryKeys { get; set; }

        public HashSet<QueryField> QueryFields { get; set; }

        public Table(string tableName, string aliasName)
        {
            TableName = tableName;
            TableAliasName = aliasName;
            PrimaryKeys = new HashSet<QueryField>();
        }
    }
}
