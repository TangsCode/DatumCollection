using DatumCollection.Configuration;
using DatumCollection.MessageQueue;
using DatumCollection.Infrastructure.Abstraction;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.Data;

namespace DatumCollection.Pipline.Storage
{
    public class DefaultStorage : IStorage
    {
        private readonly ILogger<DefaultStorage> _logger;
        private readonly IMessageQueue _mq;
        private readonly SpiderClientConfiguration _config;
        private readonly IDataStorage _storage;

        public DefaultStorage(
            ILogger<DefaultStorage> logger,
            IMessageQueue mq,
            SpiderClientConfiguration config,
            IDataStorage storage)
        {
            _logger = logger;
            _mq = mq;
            _config = config;
            _storage = storage;
        }

        public async Task Store(SpiderAtom atom)
        {
            try
            {
                await _storage.Insert(new[] { atom.Model });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                await _mq.PublishAsync(_config.TopicStatisticsFail, new Message
                {
                    MessageType = ErrorMessageType.StorageError.ToString(),
                    Data = atom
                });
            }
        }
    }
}
