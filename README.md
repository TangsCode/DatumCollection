DatumCollection
==========
[![GitHub issues](https://img.shields.io/github/issues/TangsCode/DatumCollection)](https://github.com/TangsCode/DatumCollection/issues)
[![GitHub stars](https://img.shields.io/github/stars/TangsCode/DatumCollection)](https://github.com/TangsCode/DatumCollection/stargazers)
[![GitHub license](https://img.shields.io/github/license/TangsCode/DatumCollection)](https://github.com/TangsCode/DatumCollection/blob/master/LICENSE)

DatumCollection is a platform for web spider,this repository contains source code of spider client and spider host.

DatumCollection Core
--------------------
[![latest version](https://img.shields.io/nuget/v/DatumCollection.Core)](https://www.nuget.org/packages/DatumCollection.Core) 
[![downloads](https://img.shields.io/nuget/dt/DatumCollection.Core)](https://www.nuget.org/packages/DatumCollection.Core)

DatumCollection.Core is the core library for spider, including spider hosting, pipline middleware integration, dependency injection of services, etc.

### Installation

DatumCollection Core is available on [NuGet](https://www.nuget.org/packages/DatumCollection.Core). 

You can install from <b>NuGet Package Console</b>
~~~sh
Install-package DatumCollection.Core
~~~

or from <b>dotnet CLI</b>
~~~sh
dotnet add package DatumCollection.Core
~~~

and you can the `--version` option to specify a [version](https://www.nuget.org/packages/DatumCollection.Core) to install.

### Usage

First, create an empty dotnet core console application , then use the follwing snippet to replace the code in Program.cs.

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
