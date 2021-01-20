using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DatumCollection.MessageQueue
{
    /// <summary>
    /// 消息
    /// </summary>
    [Serializable]
    public class Message
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// 消息数据
        /// </summary>
        public dynamic Data { get; set; }

        /// <summary>
        /// 消息发布时间
        /// </summary>
        public long PublishTime { get; set; }
    }

    public enum MessageType
    {
        [Description("爬虫请求")]
        SpiderRequest,
    }

    public enum ErrorMessageType
    {
        SpiderTaskError,
        SpiderAtomError,
        CollectorError,
        ExtractorError,
        StorageError
    }
}
