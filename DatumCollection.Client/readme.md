
usage for spider client:

<code>
	static void Main(string[] args)
        {
            CreateSpiderHostBuilder(args).Build().Run();
        }

    static ISpiderHostBuilder CreateSpiderHostBuilder(string[] args) =>
        new SpiderHostBuilderFactory().CreateDefaultBuilder(args).UseStartUp<Startup>();
</code>

you can contruct your startup by inheritating fron interface <code>ISatrtUp</code>,
implementing member <code>IServiceProvider ConfigureServices(IServiceCollection services);</code>

spider client configuration:
detail configuration items please take a look at appsettings.json.