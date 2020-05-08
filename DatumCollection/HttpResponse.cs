using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DatumCollection
{
    /// <summary>
    /// HTTP响应
    /// </summary>
    public class HttpResponse
    {
        /// <summary>
        /// 响应状态
        /// </summary>
        public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;

        /// <summary>
        /// 响应内容
        /// </summary>
        public string ResponseContent { get; set; }

        /// <summary>
        /// 文本类型
        /// </summary>
        public ContentType ContentType { get; set; }

        /// <summary>
        /// 初始请求
        /// </summary>
        public HttpRequest OrginalRequest { get; set; }
    }

}
