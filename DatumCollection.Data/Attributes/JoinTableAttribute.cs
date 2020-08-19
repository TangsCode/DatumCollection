using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data.Attributes
{
    public class JoinTableAttribute : Attribute
    {
        public JoinType JoinType { get; set; }

        public string ForeignKey { get; set; }

        public string ProviderKey { get; set; }

        public JoinTableAttribute(string providerKey, string foreignKey = "ID", JoinType joinType = JoinType.Left)
        {
            ProviderKey = providerKey;
            ForeignKey = foreignKey;
            JoinType = joinType;
        }
    }

    public enum JoinType
    {
        Left,
        Right,
        Inner
    }
}
