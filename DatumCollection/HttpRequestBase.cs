using DatumCollection.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;

namespace DatumCollection
{
    public abstract class HttpRequestBase
    {
        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; }

        public bool UseInterface { get; set; }

        public string DataInterface { get; set; }

        #region Headers
        /// <summary>
        /// 浏览器识别
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Encoding { get; set; }
        /// <summary>
        /// 接收文件类型
        /// </summary>
        public string Accept { get; set; }
        /// <summary>
        /// POST请求数据类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 是否使用Cookie
        /// </summary>
        public bool UseCookies { get; set; }
        /// <summary>
        /// Cookie
        /// </summary>
        public string Cookie { get; set; }
        /// <summary>
        /// 请求头
        /// </summary>
        public WebHeaderCollection Headers { get; } = new WebHeaderCollection();
        /// <summary>
        /// 请求方法
        /// </summary>
        public HttpMethod Method { get; set; }

        #endregion

        #region Body
        /// <summary>
        /// 请求体
        /// </summary>
        public string Body { get; set; }
        #endregion

        public HttpRequestBase()
        {
            Method = HttpMethod.Get;
            Encoding = "utf-8";
        }

        public void AddHeaders(string key,string value)
        {
            if (Headers[key] != null)
            {
                Headers.Add(key, value);
            }
        }

        public CookieContainer GetCookieContainer()
        {
            if (this.Cookie.IsNull()) { return null; }
            Uri uri = new Uri(Url);
            string domain = uri.Host;
            CookieContainer cookieContainer = new CookieContainer();
            string[] cookieArr = Cookie.Split(';');
            foreach (var str in cookieArr)
            {
                string[] cookieNameValue = str.Split('=');
                Cookie cookie = new Cookie(cookieNameValue[0].ToString().Trim(), cookieNameValue[1].ToString().Trim());
                cookie.Domain = domain;
                cookieContainer.Add(cookie);
            }
            return cookieContainer;
        }

    }
}
