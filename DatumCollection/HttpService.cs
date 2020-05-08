using DatumCollection.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DatumCollection
{
    /// <summary>
    /// HTTP请求服务
    /// </summary>
    public static class HttpService
    {
        /// <summary>
        /// HTTP 请求
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static HttpResponse HttpRequest(HttpRequest httpRequest)
        {
            WebResponse response = null;
            try
            {
                var contentType = httpRequest.ContentType != null && httpRequest.UseInterface ? (ContentType)Enum.Parse(typeof(ContentType), httpRequest.ContentType) : ContentType.Html;
                string url = httpRequest.UseInterface ? 
                    httpRequest.DataInterface : httpRequest.Url;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Headers = httpRequest.Headers;
                request.UserAgent = httpRequest.UserAgent ?? CommonConst.defaultUserAgent;
                request.CookieContainer = httpRequest.UseCookies ? httpRequest.GetCookieContainer() : null;
                request.Method = httpRequest.Method.Method;
                response = request.GetResponse();                
                string stremReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(httpRequest.Encoding)).ReadToEnd();                
                return new HttpResponse
                {
                    Status = ((HttpWebResponse)response).StatusCode,
                    ResponseContent = stremReader,
                    ContentType = contentType,
                    OrginalRequest = httpRequest
                };
            }
            catch (Exception e)
            {
                Log.Logger.Error(e.Message);
                if (response == null) { return null; }
                return new HttpResponse
                {
                    Status = ((HttpWebResponse)response).StatusCode,
                    OrginalRequest = httpRequest
                };
                
            }
            finally
            {
                response?.Close();
            }
            
        }

    }
}
