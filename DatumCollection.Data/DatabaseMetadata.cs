using DatumCollection.Data.Attributes;
using DatumCollection.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data
{
    public class DatabaseMetadata
    {
        public SchemaAttribute Schema;

        public HashSet<ColumnAttribute> Columns;

        public DatabaseMetadata()
        {
            Columns = new HashSet<ColumnAttribute>();
        }
    }
}
