using DatumCollection.Configuration;
using DatumCollection.Infrastructure.Abstraction;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.Infrastructure.Web;
using DatumCollection.MessageQueue;
using DatumCollection.Utility.HtmlParser;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Pipline.Extractors
{
    public class DefaultExtractor : IExtractor
    {
        private readonly ILogger<DefaultExtractor> _logger;
        private readonly IMessageQueue _mq;
        private readonly SpiderClientConfiguration _config;
        
        public DefaultExtractor(
            ILogger<DefaultExtractor> logger,
            IMessageQueue mq,
            SpiderClientConfiguration config)
        {
            _logger = logger;
            _mq = mq;
            _config = config;
        }

        public async Task ExtractAsync(SpiderAtom atom)
        {
            try
            {
                atom.Model = await atom.SpiderItem.Spider(atom);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                atom.SpiderStatus = SpiderStatus.ExtractError;
                await _mq.PublishAsync(_config.TopicStatisticsFail, new Message
                {
                    MessageType = ErrorMessageType.CollectorError.ToString(),
                    Data = atom
                });
            }            
        }
    }
}
