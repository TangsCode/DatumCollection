using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data
{
    /// <summary>
    /// 查询字段
    /// </summary>
    public class QueryField
    {
        public QueryField(string tableName, string name,string aliasName = null)
        {
            TableName = tableName;
            Name = name;
            AliasName = aliasName;
        }

        public string TableName { get; set; }

        public string Name { get; set; }

        public string AliasName { get; set; }
    }
}
