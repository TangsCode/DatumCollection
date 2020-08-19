using DatumCollection.Infrastructure.Abstraction;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.Infrastructure.Web;
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
        
        public DefaultExtractor(ILogger<DefaultExtractor> logger)
        {
            _logger = logger;
        }

        public async Task<ISpider> ExtractAsync(SpiderAtom atom)
        {
            try
            {
                var model = await atom.SpiderItem.Spider(atom);
                return model;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }            
        }
    }
}
