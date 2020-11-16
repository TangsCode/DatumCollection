using DatumCollection.Core.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DatumCollection.Core.Builder;
using DatumCollection.Data.Entities;

namespace DatumCollection.Core.Hosting
{
    public class Startup : IStartup
    {

        public void Configure(IApplicationBuilder app)
        {
            /*
             step one 
             collect data from websites
             so you have to register ICollector and inject the dependencies into services
             in ConfigureServices(IserviceCollection services)
             */
            app.UseCollector();

            /*
             step two 
             extract data from the content collected alerady
             */
            app.UseExtractor();

            /*
             step three 
             store data into database or files 
             */
            app.UseStorage();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMessageQueue();
            services.AddDataStorage(
                (c) => c.UseSqlServer());
            services.AddSpiderCollector(
                b => b.UseWebDriver());
            services.AddSpiderExtractor();
            services.AddSpider<ElectronicCommerceWebsiteSpider>();

            return services.BuildServiceProvider();            
        }
    }
}
