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
        public bool IsValid { get; set; }
        public bool IsDelete { get; set; }
        public string CreateTime { get; set; }
        public string ModifyTime { get; set; }
        public string FK_SystemUser_Create_ID { get; set; }
        public string FK_SystemUser_Modify_ID { get; set; }
        public string FK_SystemUserDepartment_Create_ID { get; set; }
    }
}
