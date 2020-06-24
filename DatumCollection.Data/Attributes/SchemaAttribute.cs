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
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        public SchemaAttribute(string table, string database = null)
        { 
            DataBase = database;
            TableName = table;
        }
    }
}
