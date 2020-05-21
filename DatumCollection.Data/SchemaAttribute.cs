using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data
{
    [AttributeUsage(AttributeTargets.Class)]
    class SchemaAttribute : Attribute
    {
        public string Database { get; set; }

        public string Table { get; set; }

        public SchemaAttribute(string database, string table)
        {
            Database = database;
            Table = table;
        }
    }
}
