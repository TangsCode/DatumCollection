using DatumCollection.Core.Spider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Core.Pipline
{
    public delegate Task PiplineDelegate(SpiderContext context);
}
