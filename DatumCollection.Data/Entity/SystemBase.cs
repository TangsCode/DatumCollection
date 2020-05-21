using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data.Entity
{
    /// <summary>
    /// 系统基础字段
    /// </summary>
    public class SystemBase
    {
        public Guid ID { get; set; }
        public bool IsValid { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifyTime { get; set; }
        public Guid? FK_SystemUser_Create_ID { get; set; }
        public Guid? FK_SystemUser_Modify_ID { get; set; }
        public Guid? FK_SystemUserDepartment_Create_ID { get; set; }
    }
}
