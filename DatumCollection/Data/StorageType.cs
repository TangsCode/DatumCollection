using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data
{
    public enum StorageType
    {
        /// <summary>
        /// 直接插入
        /// </summary>
        Insert,
        /// <summary>
        /// 插入不重复数据
        /// </summary>
        InsertIgnoreDuplicate,
        /// <summary>
        /// 插入或更新
        /// </summary>
        InsertOrUpdate,
        /// <summary>
        /// 更新
        /// </summary>
        Update
    }
}
