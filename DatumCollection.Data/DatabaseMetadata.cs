using DatumCollection.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data
{
    public class DatabaseMetadata
    {
        public SchemaAttribute Schema { get; set; }

        public HashSet<ColumnAttribute> Columns { get; set; }

        public HashSet<RelationObject> RelationObjects { get; set; }

        public DatabaseMetadata()
        {
            Columns = new HashSet<ColumnAttribute>();
            RelationObjects = new HashSet<RelationObject>();
        }
    }

    public class RelationObject
    {
        public JoinTableAttribute JoinTable { get; set; }

        public DatabaseMetadata MetaData { get; set; }

    }    
}
