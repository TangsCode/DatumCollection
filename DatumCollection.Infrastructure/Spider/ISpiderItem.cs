using DatumCollection.Infrastructure.Selectors;
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
        /// get the target selector 
        /// determine the timing to scrap when specific element is shown on the page
        /// </summary>
        /// <returns></returns>
        Task<SelectorAttribute> GetTargetSelector();

        /// <summary>
        /// get all spider selectors on the page
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SelectorAttribute>> GetAllSelectors();

        /// <summary>
        /// get the collections of spider element
        /// </summary>
        /// <returns></returns>
        Task<ISpider> Spider(SpiderAtom atom);

    }

}
