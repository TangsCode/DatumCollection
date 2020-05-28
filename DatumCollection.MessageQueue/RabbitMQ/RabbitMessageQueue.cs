using DatumCollection.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.MessageQueue.RabbitMQ
{
    /// <summary>
    /// RabbitMQ implementation
    /// </summary>
    public class RabbitMessageQueue : IMessageQueue
    {
        private ILogger<RabbitMessageQueue> _logger;

        private SpiderClientConfiguration _config;

        IConnection _connection;

        private IBasicConsumer _consumer;

        public RabbitMessageQueue(
            ILogger<RabbitMessageQueue> logger,
            SpiderClientConfiguration config
            )
        {
            _logger = logger;
            _config = config;

            var factory = new ConnectionFactory() { HostName = "" };
            _connection = factory.CreateConnection();
        }
        public Task PublishAysnc(string topic, Message message)
        {
            using (var channel = _connection.CreateModel())
            {
                try
                {
                    channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);
                    var body = Encoding.UTF8.GetBytes("");
                    channel.BasicPublish(exchange: "", routingKey: "", basicProperties: null, body: body);
                }
                catch (Exception e)
                {
                    _logger.LogError("message produce error:{0}", e.ToString());
                }                
            }
            
            return Task.CompletedTask;
        }

        public void Subscribe(string topic, Action<Message> consume)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "", type: ExchangeType.Fanout);

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                  exchange: "logs",
                                  routingKey: "");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);
            }
        }

        public void Unsubscribe(string topic)
        {
            
        }
    }
}
