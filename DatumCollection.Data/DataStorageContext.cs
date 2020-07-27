using DatumCollection.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data
{
    /// <summary>
    /// 数据存储上下文
    /// </summary>
    public class DataStorageContext
    {
        public DataStorageContext()
        {                
        }
        /// <summary>
        /// 操作类型
        /// </summary>
        public Operation Operation { get; set; }
        
        public Table MainTable { get; set; } 

        public object Parameters { get; set; }

        public string SqlStatement { get; set; }

        public bool UseSqlStatement { get { return !string.IsNullOrEmpty(SqlStatement); } }
    }

    public enum Operation
    {
        Insert,
        InsertOrUpdate,
        Update,
        Delete,
        Query
    }
}
