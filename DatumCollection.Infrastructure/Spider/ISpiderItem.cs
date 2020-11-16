using DatumCollection.Infrastructure.Web;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Infrastructure.Spider
{
    /// <summary>
    /// defines the spider item select behavior
    /// </summary>
    public interface ISpiderItem
    {
        /// <summary>
        /// spider specific ISpiderConfig
        /// that generates selectors and determine the target on dynamic pages
        /// </summary>
        ISpiderConfig SpiderConfig { get; }

        /// <summary>
        /// get the collections of spider element
        /// </summary>
        /// <returns></returns>
        Task<ISpider> Spider(SpiderAtom atom);

    }

    public interface ISelector
    {
        Task<T> SelectAsync<T>(string path);
    }

    public class XPathSelector : ISelector
    {
        public Task<T> SelectAsync<T>(string path)
        {
            throw new NotImplementedException();
        }
    }
}
