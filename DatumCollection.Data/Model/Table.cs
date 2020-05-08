using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data.Model
{
    public class Table
    {
        /// <summary>
        /// 列集合
        /// </summary>
        public HashSet<Column> Columns { get; set; }

        /// <summary>
        /// 数据库信息
        /// </summary>
        public Schema Schema { get; set; }

        public Table()
        {
            Columns = new HashSet<Column>();
        }
    }
}
