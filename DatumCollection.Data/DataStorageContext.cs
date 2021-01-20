using DatumCollection.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatumCollection.Data
{
    /// <summary>
    /// 数据存储上下文
    /// </summary>
    public class DataStorageContext
    {
        public DataStorageContext()
        {
            JoinRelations = new HashSet<JoinRelation>();
            TableAliassNameMappings = new Dictionary<string, string>();
        }

        public DataStorageContext(DatabaseMetadata metadata): this()
        {
            Metadata = metadata;
            MainTable = new TableInfo
            {
                TableName = metadata.Schema.TableName,
                AliasName = metadata.Schema.TableName.FirstOrDefault().ToString().ToLower()
            };
            TableAliassNameMappings.Add(MainTable.AliasName, MainTable.TableName);
            AddRelationObjects(MainTable, metadata.RelationObjects);
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        public Operation Operation { get; set; }
        
        public DatabaseMetadata Metadata { get; set; } 

        public Dictionary<string,string> TableAliassNameMappings { get; set; }
         
        public TableInfo MainTable { get; set; }

        /// <summary>
        /// 查询关联表集合
        /// </summary>
        public HashSet<JoinRelation> JoinRelations { get; set; }

        public object Parameters { get; set; }

        public string SqlStatement { get; set; }

        public bool UseSqlStatement { get { return !string.IsNullOrEmpty(SqlStatement); } }

        public void AddRelationObjects(TableInfo origin, IEnumerable<RelationObject> objects) 
        {
            if (objects.Any())
            {
                foreach (var item in objects)
                {
                    var left = new JoinTable
                    {
                        TableName = origin.TableName,
                        AliasName = origin.AliasName,
                        JoinKey = item.JoinTable.ProviderKey
                    };
                    var right = new JoinTable
                    {
                        TableName = item.MetaData.Schema.TableName,
                        AliasName = item.MetaData.Schema.TableName.FirstOrDefault(c => !TableAliassNameMappings.ContainsKey(c.ToString().ToLower())).ToString(),
                        JoinKey = item.JoinTable.ForeignKey
                    };
                    var relation = new JoinRelation
                    {
                        Left = left,
                        Right = right,
                        JoinType = item.JoinTable.JoinType
                    };
                    if (!TableAliassNameMappings.ContainsKey(right.AliasName))
                    {
                        TableAliassNameMappings.Add(right.AliasName, right.TableName);
                    }
                    JoinRelations.Add(relation);
                    AddRelationObjects(right, item.MetaData.RelationObjects);
                }
            }
            
        }
    }

    public enum Operation
    {
        Insert,
        InsertOrUpdate,
        Update,
        Delete,
        Query
    }

    public class JoinRelation
    {
        public JoinTable Left { get; set; }

        public JoinTable Right { get; set; }

        public JoinType JoinType { get; set; }
    }

    public class JoinTable : TableInfo
    {
        public string JoinKey { get; set; }
    }

    public class TableInfo 
    {
        public string TableName { get; set; }

        public string AliasName { get; set; }

    }


}
