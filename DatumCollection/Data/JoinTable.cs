using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatumCollection.Data
{

    public class JoinTable
    {
        public JoinTable(string name,string aliasName,string joinCondition,
            JoinType joinType = JoinType.Left)
        {
            Name = name;
            AliasName = aliasName;
            JoinType = joinType;
            JoinCondition = joinCondition;
        }

        /// <summary>
        /// 表联结类型
        /// </summary>
        public JoinType JoinType { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string AliasName { get; set; }

        /// <summary>
        /// 查询字段集合
        /// </summary>
        public string[] QueryFields { get; set; }
       
        /// <summary>
        /// 联结条件
        /// </summary>
        public string JoinCondition { get; set; }

        public JoinTable AddQueryField(string[] fields)
        {
            foreach (var field in fields)
            {
                QueryFields.Append(field);
            }

            return this;
        }
       
    }

    public enum JoinType
    {
        Left,
        Right,
        Inner
    }
}
