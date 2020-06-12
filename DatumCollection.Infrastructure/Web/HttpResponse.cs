using DatumCollection.Infrastructure.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Infrastructure.Web
{
    public class HttpResponse
    {
        public bool Success { get; set; }

        public string ErrorMsg { get; set; }

        public ContentType ContentType { get; set; }

        public dynamic Content { get; set; }
    }
}
