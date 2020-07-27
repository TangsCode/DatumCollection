using DatumCollection.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

        private EventingBasicConsumer _consumer;

        private IModel _channel;

        public RabbitMessageQueue(
            ILogger<RabbitMessageQueue> logger,
            SpiderClientConfiguration config
            )
        {
            _logger = logger;
            _config = config;

            var factory = new ConnectionFactory() { HostName = "" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _consumer = new EventingBasicConsumer(_channel);
            _channel.ExchangeDeclare(exchange: _config.RabbitMQExchange, type: ExchangeType.Fanout);
        }

        public void Dispose()
        {
            _channel?.Dispose();            
        }

        public Task PublishAsync(string topic, Message message)
        {
            try
            {                
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                _channel.BasicPublish(exchange: _config.RabbitMQExchange, routingKey: topic, basicProperties: null, body: body);
            }
            catch (Exception e)
            {
                _logger.LogError("message produce error:{0}", e.ToString());
            }

            return Task.CompletedTask;
        }

        public void Subscribe(string topic, Action<Message> consume)
        {
            try
            {
                var queueName = _config.RabbitMQQueue + topic;
                _channel.QueueBind(queue: queueName,
                                  exchange: _config.RabbitMQExchange,
                                  routingKey: topic);                

                _consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(body.ToArray()));
                    consume(message);
                };
                _channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: _consumer);
            }
            catch (Exception e)
            {
                _logger.LogError("message consume error:{0}", e.ToString());
            }
        }

        public void Unsubscribe(string topic)
        {
            _channel.ExchangeUnbind(_config.RabbitMQExchange, _config.RabbitMQExchange, topic);
        }
    }
}
