using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Core.Hosting
{
    public interface ISpiderHostBuilder
    {
        ISpiderHost Build();

        ISpiderHostBuilder ConfigureServices(Action<IServiceCollection> configureServices);

        ISpiderHostBuilder ConfigureLogService(Action<ILoggingBuilder> configure);

        ISpiderHostBuilder ConfigureAppConfig(Action<IConfigurationBuilder> configure);
        
    }
}
