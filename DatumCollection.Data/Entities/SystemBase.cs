using DatumCollection.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data.Entities
{
    /// <summary>
    /// 系统基础字段
    /// </summary>
    public class SystemBase
    {
        [Column(IsPrimaryKey = true, Name = "ID", Type = "uniqueidentifier")]
        public Guid? ID { get; set; }
        [Column(Name = "IsValid", Type = "bit")]
        public bool IsValid { get; set; }
        [Column(Name = "IsDelete", Type = "bit")]
        public bool IsDelete { get; set; }
        [Column(Name = "CreateTime", Type = "datetime")]
        public DateTime CreateTime { get; set; }
        [Column(Name = "ModifyTime", Type = "datetime")]
        public DateTime? ModifyTime { get; set; }
        [Column(Name = "FK_SystemUser_Create_ID", Type = "uniqueidentifier")]
        public Guid? FK_SystemUser_Create_ID { get; set; }
        [Column(Name = "FK_SystemUser_Modify_ID", Type = "uniqueidentifier")]
        public Guid? FK_SystemUser_Modify_ID { get; set; }
        [Column(Name = "FK_SystemUserDepartment_Create_ID", Type = "uniqueidentifier")]
        public Guid? FK_SystemUserDepartment_Create_ID { get; set; }

        public SystemBase()
        {
            ID = Guid.NewGuid();
            CreateTime = DateTime.Now;
            ModifyTime = DateTime.Now;
            FK_SystemUser_Create_ID = Guid.Empty;
            FK_SystemUser_Modify_ID = Guid.Empty;
        }
    }
}
