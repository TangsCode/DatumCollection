using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DatumCollection.Data.Model
{
    /// <summary>
    /// 列信息
    /// </summary>
    public class Column
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public int Length { get; set; } = 255;

        public bool Required { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsUnqiue { get; set; }

        public PropertyInfo PropertyInfo { get; set; }
    }
}
