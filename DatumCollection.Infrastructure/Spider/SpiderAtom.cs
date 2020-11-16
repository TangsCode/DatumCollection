using DatumCollection.Infrastructure.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Infrastructure.Spider
{
    /// <summary>
    /// represents the spider unit independently
    /// </summary>
    public class SpiderAtom
    {
        /// <summary>
        /// http request 
        /// </summary>
        public HttpRequest Request { get; set; }

        /// <summary>
        /// http response
        /// </summary>
        public HttpResponse Response { get; set; }

        /// <summary>
        /// spider model
        /// </summary>
        public ISpider Model { get; set; }

        /// <summary>
        /// spider item that contains spider config
        /// </summary>
        public ISpiderItem SpiderItem { get; set; }

    }
}
