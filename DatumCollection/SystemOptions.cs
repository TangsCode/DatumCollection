using System;
using System.Collections.Generic;
using System.Text;
using DatumCollection.Data;
using DatumCollection.Utility.Extensions;
using Microsoft.Extensions.Configuration;

namespace DatumCollection
{
    /// <summary>
    /// 系统选项
    /// </summary>
    public class SystemOptions
    {
        private readonly IConfiguration _configuration;

        public SystemOptions(IConfiguration configuration)
        {
            _configuration = configuration;            
        }

        public T Get<T>(string key) where T :class
        {
            return _configuration.GetSection(key) as T;
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

        #region selenium相关配置

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
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public virtual string ConnectionString => _configuration["ConnectionString"];

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public virtual string StorageConnectionString => _configuration["StorageConnectionString"];

        /// <summary>
        /// 存储器类型: FullTypeName, AssemblyName
        /// </summary>
        public virtual string Storage => _configuration["Storage"];

        /// <summary>
        /// 是否忽略数据库相关的大写小
        /// </summary>
        public virtual bool StorageIgnoreCase => string.IsNullOrWhiteSpace(_configuration["IgnoreCase"]) ||
                                         bool.Parse(_configuration["StorageIgnoreCase"]);

        /// <summary>
        /// 存储器失败重试次数限制
        /// </summary>
        public virtual int StorageRetryTimes => string.IsNullOrWhiteSpace(_configuration["StorageRetryTimes"])
            ? 600
            : int.Parse(_configuration["StorageRetryTimes"]);

        /// <summary>
        /// 是否使用事务操作。默认不使用。
        /// </summary>
        public virtual bool StorageUseTransaction => !string.IsNullOrWhiteSpace(_configuration["StorageUseTransaction"]) &&
                                             bool.Parse(_configuration["StorageUseTransaction"]);

        /// <summary>
        /// 存储器类型
        /// </summary>
        public virtual StorageType StorageType => string.IsNullOrWhiteSpace(_configuration["StorageType"])
            ? StorageType.InsertIgnoreDuplicate
            : (StorageType)Enum.Parse(typeof(StorageType), _configuration["StorageType"]);

        /// <summary>
        /// MySql 文件类型
        /// </summary>
        public virtual string MySqlFileType => _configuration["MySqlFileType"];

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

        public virtual string TopicScheduleObserver => "ScheduleObserver";

        public virtual int ScheduleIdleWaitCount =>
            _configuration["ScheduleIdleWaitCount"].NotNull()
            ? int.Parse(_configuration["ScheduleIdleWaitCount"])
            : 60;

        /// <summary>
        /// 消息队列推送消息、文章话题、获取消息失败重试的次数
        /// 默认是 28800 次即 8 小时
        /// </summary>
        public virtual int MessageQueueRetryTimes => string.IsNullOrWhiteSpace(_configuration["MessageQueueRetryTimes"])
            ? 28800
            : int.Parse(_configuration["MessageQueueRetryTimes"]);

        /// <summary>
        /// 设置消息过期时间，每个消息发送应该带上时间，超时的消息不作处理
        /// 默认值 60 秒
        /// </summary>
        public virtual int MessageExpiredTime => string.IsNullOrWhiteSpace(_configuration["MessageExpiredTime"])
            ? 60
            : int.Parse(_configuration["MessageExpiredTime"]);

        public T GetConfigInfo<T>(string key)
        {
            var value = _configuration[key];
            if (value.IsNull()) { return default(T); }
            var ret = Convert.ChangeType(value, typeof(T));
            return (T)ret;
        }
    }
}
