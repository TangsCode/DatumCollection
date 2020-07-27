using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class SchemaAttribute : Attribute
    {        
        /// <summary>
        /// 数据库
        /// </summary>
        public string DataBase { get; set; }

        /// <summary>
        /// 架构
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        public SchemaAttribute(string table, string schema = null, string database = null)
        { 
            DataBase = database;
            Schema = schema ?? "dbo";
            TableName = table;
        }
    }
}
