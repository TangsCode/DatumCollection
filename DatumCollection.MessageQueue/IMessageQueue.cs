using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.MessageQueue
{
    /// <summary>
    /// 消息队列接口
    /// </summary>
    public interface IMessageQueue : IDisposable
    {
        /// <summary>
        /// 推送消息到指定topic
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task PublishAsync(string topic, Message message);

        /// <summary>
        /// 订阅消息并消费
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="subscribe"></param> 
        void Subscribe(string topic, Action<Message> consume);

        /// <summary>
        /// 取消订阅
        /// Kafka将消费者当前订阅集全部取消
        /// </summary>
        /// <param name="topic"></param>
        void Unsubscribe(string topic);
    }
}
