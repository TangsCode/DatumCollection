using DatumCollection.Configuration;
using DatumCollection.Infrastructure.Abstraction;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.Infrastructure.Web;
using DatumCollection.MessageQueue;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Pipline.Collectors
{
    /// <summary>
    /// default collector using http client
    /// not adapted to asynchronous web pages
    /// </summary>
    public class DefaultCollector : ICollector
    {
        private readonly ILogger<DefaultCollector> _logger;
        private readonly IMessageQueue _mq;
        private readonly SpiderClientConfiguration _config;

        public DefaultCollector(
            ILogger<DefaultCollector> logger,
            IMessageQueue mq,
            SpiderClientConfiguration config)
        {
            _logger = logger;
            _mq = mq;
            _config = config;
        }

        public async Task CollectAsync(SpiderAtom atom)
        {
            WebResponse response = null;
            var httpResponse = new HttpResponse{ ContentType = atom.Request.ContentType };
            try 
            {                
                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(atom.Request.Url);
                webRequest.Method = atom.Request.Method;
                response = await webRequest.GetResponseAsync();
                string stremReader = new StreamReader(response.GetResponseStream(), atom.Request.Encoding).ReadToEnd();
                httpResponse.Content = stremReader;
                httpResponse.Success = ((HttpWebResponse)response).StatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                _logger.LogError("collect data error with {collector}", nameof(DefaultCollector));
                httpResponse.Success = false;
                httpResponse.ErrorMsg = e.ToString();
            }
            finally
            {
                response?.Close();
            }
        }

        public void Dispose()
        {
            
        }
    }
}
