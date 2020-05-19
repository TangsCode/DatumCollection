
*Usage for spider client:

<code>
	static void Main(string[] args)
        {
            CreateSpiderHostBuilder(args).Build().Run();
        }
</code>
<br />
<code>
    static ISpiderHostBuilder CreateSpiderHostBuilder(string[] args) =>
        new SpiderHostBuilderFactory().CreateDefaultBuilder(args).UseStartUp<Startup>();
</code>

you can contruct your startup by inheritating from interface <code>ISatrtUp</code>,
implementing member <code>IServiceProvider ConfigureServices(IServiceCollection services);</code>

*Spider client configuration:<br />
detail configuration items please take a look at appsettings.json.
