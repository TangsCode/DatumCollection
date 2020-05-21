using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DatumCollection.Data
{
    /// <summary>
    /// DB执行结果 
    /// </summary>
    public class DbExecutionResult
    {
        public DbExecutionResult()
        {
            ErrorCode = 0;
        }

        /// <summary>
        /// 错误代码
        /// 默认返回0(成功)
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// 受影响行数
        /// </summary>
        public int RowsAffected { get; set; }
    }

    public enum ErrorCode
    {
        [Description("数据库连接失败")]
        DbConnectionFailed = 1001,
        [Description("Sql语句错误")]
        SqlSyntaxError = 1002,
    }
}
