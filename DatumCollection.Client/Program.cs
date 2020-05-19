

using DatumCollection.Core.Hosting;

namespace DatumCollection.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateSpiderHostBuilder(args).Build().Run();
        }

        static ISpiderHostBuilder CreateSpiderHostBuilder(string[] args) =>
            new SpiderHostBuilderFactory().CreateDefaultBuilder(args).UseStartUp<Startup>();

    }
}
