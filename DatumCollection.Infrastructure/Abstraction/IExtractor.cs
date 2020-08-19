using DatumCollection.Infrastructure.Spider;
using DatumCollection.Infrastructure.Web;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Infrastructure.Abstraction
{
    public interface IExtractor
    {
        /// <summary>
        /// extract reponse data and format as T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="atom"></param>
        /// <returns></returns>
        Task<ISpider> ExtractAsync(SpiderAtom atom);
    }
}
