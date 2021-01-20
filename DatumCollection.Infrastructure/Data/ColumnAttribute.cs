using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DatumCollection.Infrastructure.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public string Type { get; set; } = "nvarchar";

        public int Length { get; set; } = 200;

        public int Precision { get; set; }

        public int Scale { get; set; }

        public bool Required { get; set; }

        public bool IsPrimaryKey { get; set; } = false;

        public bool IsUnqiue { get; set; } = false;

        public ColumnAttribute(string name, string type)
        {
            Name = name;
            Type = type;
        }
        public PropertyInfo PropertyInfo { get; set; }
    }
}
