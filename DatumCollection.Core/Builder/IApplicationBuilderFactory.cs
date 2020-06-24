using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Core.Builder
{
    public interface IApplicationBuilderFactory
    {
        IApplicationBuilder CreateBuilder();
    }
}
