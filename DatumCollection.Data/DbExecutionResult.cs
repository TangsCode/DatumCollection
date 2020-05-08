using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Data
{
    /// <summary>
    /// DB执行结果 
    /// </summary>
    public class DbExecutionResult
    {
        /// <summary>
        /// 错误代码
        /// 默认返回0(成功)
        /// </summary>
        public int ErrorCode { get; set; }

        public dynamic Data { get; set; }
    }

    public enum ErrorCode
    {
        DbConnectionFailed = 1001,
        SqlSyntaxError = 1002,
    }
}
