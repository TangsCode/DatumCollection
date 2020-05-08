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
        public Operation Operation { get; set; }

        
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
