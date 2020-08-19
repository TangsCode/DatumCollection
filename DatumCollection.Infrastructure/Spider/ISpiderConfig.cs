using DatumCollection.Infrastructure.Selectors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Infrastructure.Spider
{
    public interface ISpiderConfig
    {
        Task<IEnumerable<SelectorAttribute>> GetSpiderSelectors();
    }

    public enum ConfigSource
    {
        FromDatabase,
        FromAttribute
    }
}
