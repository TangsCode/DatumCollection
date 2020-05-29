using DatumCollection.Utility.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Configuration
{
    /// <summary>
    /// 爬虫客户端配置
    /// 读取客户端目录下json文件获取配置信息
    /// </summary>
    public class SpiderClientConfiguration
    {
        private readonly IConfiguration _configuration;

        public SpiderClientConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region host configuration
        IConfigurationSection _host => _configuration.GetSection("spiderHost");

        public virtual int SpiderHostTimeout => int.Parse(_host.GetSection("timeout").Value);

        #endregion

        #region hardware resources restraint
        IConfigurationSection _hardware => _configuration.GetSection("hardwareResourcesRestraint");

        /// <summary>
        /// 剩余CPU百分比最小值
        /// 此配置用来限制可用计算资源的最大比例
        /// </summary>
        public virtual int CpuFreePercentageMinimum => int.Parse(_hardware.GetSection("cpuFreePercentageMinimum")?.Value);

        /// <summary>
        /// 剩余内存百分比最小值
        /// 此配置用来限制可用内存资源的最大比例
        /// </summary>
        public virtual int MemoryFreePercentageMinimum => int.Parse(_hardware.GetSection("memoryFreePercentageMinimum")?.Value);
        #endregion

        #region kafka setting 
        private IConfigurationSection _kafka => _configuration.GetSection("kafka");

        public virtual string KafkaBootstrapServers => _kafka.GetSection("bootstrapServers")?.Value;

        public virtual string KafkaConsumerGroup => _kafka.GetSection("consumerGroup")?.Value;
        #endregion

        #region RabbitMQ setting
        private IConfigurationSection _rabbitMQ => _configuration.GetSection("rabbitMQ");

        public virtual string RabbitMQExchange => _rabbitMQ.GetSection("exchange")?.Value;

        public virtual string RabbitMQQueue => _rabbitMQ.GetSection("queue")?.Value;
        #endregion

        #region webdriver setting
        IConfigurationSection _webdriver => _configuration.GetSection("webdriver");

        /// <summary>
        /// 浏览器
        /// </summary>
        public virtual string Browser => _webdriver.GetSection("browser")?.Value;

        /// <summary>
        /// 图片下载路径
        /// </summary>
        public virtual string ImageDownloadPath => _webdriver.GetSection("imageDownloadPath")?.Value;

        /// <summary>
        /// web driver executable file absolute folder
        /// </summary>
        public virtual string WebDriverExecutetableFilePath => _webdriver.GetSection("exePath")?.Value;

        /// <summary>
        /// Web driver timeout in seconds
        /// </summary>
        public virtual int WebDriverTimeoutInSeconds => _webdriver.GetSection("timeout") != null
            ? int.Parse(_webdriver.GetSection("timeout").Value) : 60;

        /// <summary>
        /// web driver process count
        /// </summary>
        public virtual int WebDriverProcessCount => _webdriver.GetSection("processes") != null
            ? int.Parse(_webdriver.GetSection("processes").Value) : 10;

        #endregion

        #region storage configuration
        IConfigurationSection _storage => _configuration.GetSection("storage");

        /// <summary>
        /// 数据库类型
        /// </summary>
        public virtual string Database => _storage.GetSection("database").Value;

        /// <summary>
        /// 存储类型
        /// </summary>
        public virtual string StorageType => _storage.GetSection("type").Value;

        /// <summary>
        /// 客户端版本
        /// </summary>
        public virtual string ClientVersion => _storage.GetSection("clientVersion").Value;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public virtual string ConnectionString => _storage.GetSection("connectionString").Value;

        #endregion

        #region Email setting
        IConfigurationSection _email => _configuration.GetSection("email");
        /// <summary>
        /// 邮件服务地址
        /// </summary>
        public virtual string EmailHost => _email.GetSection("host")?.Value;

        /// <summary>
        /// 邮件用户名
        /// </summary>
        public virtual string EmailFrom => _email.GetSection("from")?.Value;

        /// <summary>
        /// 邮件密码
        /// </summary>
        public virtual string EmailPassword => _email.GetSection("from")?.Value;

        /// <summary>
        /// 邮件显示名称
        /// </summary>
        public virtual string EmailDisplayName => _email.GetSection("displayName")?.Value;

        /// <summary>
        /// 邮件服务端口
        /// </summary>
        public virtual int EmailPort => int.Parse(_email.GetSection("port")?.Value);
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
