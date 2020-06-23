using DatumCollection.Configuration;
using DatumCollection.Infrastructure.Abstraction;
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

        public Task<HttpResponse> CollectAsync(HttpRequest request)
        {
            WebResponse response = null;
            var httpResponse = new HttpResponse{ ContentType = request.ContentType };
            try 
            {                
                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(request.Url);
                webRequest.Method = request.Method;
                response = webRequest.GetResponse();
                string stremReader = new StreamReader(response.GetResponseStream(), request.Encoding).ReadToEnd();
                httpResponse.Content = stremReader;
                httpResponse.Success = ((HttpWebResponse)response).StatusCode == HttpStatusCode.OK;
                return Task.FromResult(httpResponse);
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
            return Task.FromResult(httpResponse);
        }
    }
}
