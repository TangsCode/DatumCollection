using DatumCollection.Utility.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Configuration
{
    /// <summary>
    /// 系统选项
    /// 读取客户端目录下json文件获取配置信息
    /// </summary>
    public class SystemOptions
    {
        private readonly IConfiguration _configuration;

        public SystemOptions(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 剩余CPU百分比
        /// 此配置用来限制可用计算资源的最大比例
        /// </summary>
        public virtual int FreeCpuLimitPercent => int.Parse(_configuration["FreeCpuLimitPercent"]);

        /// <summary>
        /// 剩余内存百分比
        /// 此配置用来限制可用内存资源的最大比例
        /// </summary>
        public virtual int FreeMemoryLimitPercent => int.Parse(_configuration["FreeMemoryLimitPercent"]);

        /// <summary>
        /// 图片下载路径
        /// </summary>
        public virtual string ImageDownloadPath => _configuration["ImageDownloadPath"];

        #region kafka setting 
        public virtual string KafkaBootstrapServers => _configuration["KafkaBootstrapServers"];

        public virtual string KafkaConsumerGroup => _configuration["KafkaConsumerGroup"];
        #endregion

        #region webdriver setting

        public virtual string Browser => _configuration["Browser"];
        /// <summary>
        /// Screenshot file stored folder
        /// </summary>
        public virtual string ScreenshotPath => _configuration["ScreenshotPath"];

        /// <summary>
        /// Web driver executable file absolute folder
        /// </summary>
        public virtual string WebDriverPath => _configuration["WebDriverPath"];

        /// <summary>
        /// Web driver timeout in seconds
        /// </summary>
        public virtual int WebDriverTimeoutSeconds =>
            _configuration["WebDriverTimeoutSeconds"].NotNull() ?
            int.Parse(_configuration["WebDriverTimeoutSeconds"]) : 60;

        public virtual int WebDriverProcessCount =>
            _configuration["WebDriverProcessCount"].NotNull() ?
            int.Parse(_configuration["WebDriverProcessCount"]) : 10;


        #endregion

        #region Database configuration
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public virtual string ConnectionString => _configuration["ConnectionString"];

        #endregion

        #region Email setting
        /// <summary>
        /// 邮件服务地址
        /// </summary>
        public virtual string EmailHost => _configuration["EmailHost"];

        /// <summary>
        /// 邮件用户名
        /// </summary>
        public virtual string EmailFrom => _configuration["EmailFrom"];

        /// <summary>
        /// 邮件密码
        /// </summary>
        public virtual string EmailPassword => _configuration["EmailPassword"];

        /// <summary>
        /// 邮件显示名称
        /// </summary>
        public virtual string EmailDisplayName => _configuration["EmailDisplayName"];

        /// <summary>
        /// 邮件服务端口
        /// </summary>
        public virtual int EmailPort => int.Parse(_configuration["EmailPort"]);
        #endregion

        public T GetConfigInfo<T>(string key)
        {
            var value = _configuration[key];
            if (value.IsNull()) { return default(T); }
            var ret = Convert.ChangeType(value, typeof(T));
            return (T)ret;
        }
    }
}
