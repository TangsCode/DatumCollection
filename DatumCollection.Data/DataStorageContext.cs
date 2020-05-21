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
            Parameters = new List<object>().ToArray();            
            UseQueryString = false;
        }
        /// <summary>
        /// 操作类型
        /// </summary>
        public Operation Operation { get; set; }
        
        public Table MainTable { get; set; } 

        public object[] Parameters { get; set; }

        public string QueryString {
            get
            {
                return QueryString;
            }
            set
            {
                if (!string.IsNullOrEmpty(QueryString)) { UseQueryString = true; }
                QueryString = value;
            }
        }

        public bool UseQueryString { get; set; }
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
