
### Usage for spider client:

~~~cs
    static void Main(string[] args)
        {
            CreateSpiderHostBuilder(args).Build().Run();
        }
        
    static ISpiderHostBuilder CreateSpiderHostBuilder(string[] args) =>
        new SpiderHostBuilderFactory().CreateDefaultBuilder(args).UseStartUp<Startup>();
~~~

You can construct your own <code>Startup</code> by inheritating from interface <code>IStartup</code>,
implementing member <code>IServiceProvider ConfigureServices(IServiceCollection services)</code> and inject dependencies you require.

### Spider client configuration
Detail configuration items please take a look at appsettings.json.
