using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Infrastructure.Web
{
    public class HttpRequest
    {
        public string Url { get; set; }

        public string Method { get; set; }

        public ContentType ContentType { get; set; }

        public Encoding Encoding { get; set; }
        
    }
}
