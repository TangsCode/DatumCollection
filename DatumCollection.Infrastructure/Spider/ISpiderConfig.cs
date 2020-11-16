using DatumCollection.Infrastructure.Selectors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Infrastructure.Spider
{
    public interface ISpiderConfig
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
        Task<IEnumerable<SelectorAttribute>> GetSpiderSelectors();
    }

    public enum ConfigSource
    {
        FromDatabase,
        FromAttribute
    }
}
