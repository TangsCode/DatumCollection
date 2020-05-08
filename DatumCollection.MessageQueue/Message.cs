using System;
using System.Collections.Generic;
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
        public DateTime PublishTime { get; set; }
    }
}
