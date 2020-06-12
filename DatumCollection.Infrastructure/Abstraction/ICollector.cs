using DatumCollection.Infrastructure.Web;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Infrastructure.Abstraction
{
    /// <summary>
    /// collector interface
    /// defines collect action <see cref="CollectAsync(HttpRequest)"/>
    /// </summary>
    public interface ICollector
    {
        Task<HttpResponse> CollectAsync(HttpRequest request);        
    }
}
