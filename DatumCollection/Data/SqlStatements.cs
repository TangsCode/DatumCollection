using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data
{
    /// <summary>
    /// SQL语句
    /// </summary>
    public class SqlStatements
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DatabaseSql { get; set; }

        /// <summary>
        /// 创建表的SQL
        /// </summary>
        public string CreateTableSql { get; set; }

        /// <summary>
        /// 创建数据库SQL
        /// </summary>
        public string CreateDatabaseSql { get; set; }

        /// <summary>
        /// 插入SQL
        /// </summary>
        public string InsertSql { get; set; }

        /// <summary>
        /// 插入忽略重复SQL
        /// </summary>
        public string InsertIgnoreDuplicateSql { get; set; }

        /// <summary>
        /// 更新SQL
        /// </summary>
        public string UpdateSql { get; set; }

        /// <summary>
        /// 插入或者更新SQL
        /// </summary>
        public string InsertOrUpdateSql { get; set; }

    }
}
