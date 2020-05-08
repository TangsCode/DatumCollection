using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data
{
    /// <summary>
    /// Schema信息
    /// </summary>
    public class Schema
    {
        /// <summary>
        /// 数据库
        /// </summary>
        public string DataBase { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        public Schema(string database, string table)
        {
            DataBase = database;
            TableName = table;
        }
    }
}
