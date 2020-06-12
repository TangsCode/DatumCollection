using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Infrastructure.Spider
{
    public delegate Task PiplineDelegate(SpiderContext context);
}
