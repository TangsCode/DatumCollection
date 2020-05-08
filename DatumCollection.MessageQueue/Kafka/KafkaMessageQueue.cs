using Confluent.Kafka;
using DatumCollection.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.MessageQueue.Kafka
{
    /// <summary>
    /// Kafka消息队列
    /// </summary>
    public class KafkaMessageQueue : IMessageQueue
    {
        private ILogger<KafkaMessageQueue> _logger;

        private SystemOptions _options;

        private IProducer<string, string> _producer;

        private IConsumer<string, string> _consumer;

        public KafkaMessageQueue(
            ILogger<KafkaMessageQueue> logger,
            SystemOptions options
            )
        {
            _logger = logger;
            var config = new ProducerConfig { BootstrapServers = options.KafkaBootstrapServers };
            _producer = new ProducerBuilder<string, string>(config).Build();
            var consumerConfig = new ConsumerConfig
            {
                GroupId = options.KafkaConsumerGroup,
                BootstrapServers = options.KafkaBootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        }
        public Task PublishAysnc(string topic, Message message)
        {
            try
            {
                var dr = _producer.ProduceAsync(topic, new Message<string, string> { Value = JsonConvert.SerializeObject(message) });
                return dr;
            }
            catch (ProduceException<string, string> e)
            {
                _logger.LogError("message produce error:{0}", e.ToString());
            }
            return Task.CompletedTask;
        }

        public void Subscribe(string topic, Action<Message> consume)
        {
            try
            {
                var cr = _consumer.Consume();
                var message = JsonConvert.DeserializeObject<Message>(cr.Value);
                consume(message);
            }
            catch (ConsumeException e)
            {
                _logger.LogError("message consume error:{0}", e.ToString());
            }
        }

        public void Unsubscribe(string topic)
        {
            try
            {
                _consumer.Unsubscribe();
            }
            catch (Exception e)
            {
                _logger.LogError("kafka unsubscribe error:{0}", e.ToString());
            }
        }
    }
}
