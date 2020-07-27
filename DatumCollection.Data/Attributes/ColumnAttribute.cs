using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DatumCollection.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public string Type { get; set; } = "nvarchar";

        public int Length { get; set; } = 200;

        public bool Required { get; set; }

        public bool IsPrimaryKey { get; set; } = false;

        public bool IsUnqiue { get; set; } = false;

        public PropertyInfo PropertyInfo { get; set; }
    }
}
