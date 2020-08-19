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

        //public async Task<T> Spider<T>(HttpResponse response) where T : ISpider
        //{
        //    var result = System.Activator.CreateInstance<T>();

        //    var selectors = await SpiderConfig.GetSpiderSelectors();
        //    var props = typeof(T).GetProperties();

        //    foreach (var selector in selectors)
        //    {
        //        ISelector sel = null;
        //        switch (selector.Type)
        //        {
        //            case Selectors.SelectorType.XPath:
        //                sel = new XPathSelector();                        
        //                break;
        //            case Selectors.SelectorType.Html:
        //                break;
        //            case Selectors.SelectorType.Json:
        //                break;
        //            default:
        //                break;
        //        }
        //        props.FirstOrDefault(p => p.Name.ToLower() == selector.Key.ToLower()).SetValue(result, sel.SelectAsync<string>(selector.Path));
        //    }


        //    return result;
        //}

    }
}
