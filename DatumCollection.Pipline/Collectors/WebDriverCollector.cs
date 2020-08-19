using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatumCollection.Configuration;
using DatumCollection.Infrastructure.Abstraction;
using DatumCollection.Infrastructure.Web;
using DatumCollection.MessageQueue;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;

namespace DatumCollection.Pipline.Collector
{
    public class WebDriverCollector : ICollector
    {
        private readonly ILogger<WebDriverCollector> _logger;
        private readonly IMessageQueue _mq;
        private readonly SpiderClientConfiguration _config;

        private IWebDriver driver;
        private ConcurrentBag<IWebDriver> drivers;
        private DriverService _driverService;
        private DriverOptions _driverOptions;

        private Browser _browser;

        private string WebDriverExePath => _config.WebDriverExecutetableFilePath;
        private string ImagePath => _config.ImageDownloadPath;

        public WebDriverCollector(
            ILogger<WebDriverCollector> logger,
            IMessageQueue mq,
            SpiderClientConfiguration config)
        {
            _logger = logger;
            _mq = mq;
            _config = config;

            Initialize();
        }

        private void Initialize()
        {
            if (!Directory.Exists(ImagePath))
            {
                Directory.CreateDirectory(ImagePath);
            }
            drivers = new ConcurrentBag<IWebDriver>();
            _browser = (Browser)Enum.Parse(typeof(Browser), _config.Browser ?? Browser.Chrome.ToString(), true);
            switch (_browser)
            {
                case Browser.Chrome:
                    {
                        _driverService = ChromeDriverService.CreateDefaultService(WebDriverExePath);
                        //disable command prompt output
                        _driverService.HideCommandPromptWindow = true;
                        _driverService.SuppressInitialDiagnosticInformation = true;
                        var chromeOptions = new ChromeOptions();
                        #region proxy setting
                        //Proxy proxy = new Proxy();
                        //proxy.Kind = ProxyKind.Manual;
                        #endregion
                        chromeOptions.AddArguments(new[] { "disable-infobars", "headless", "silent", "log-level=3", "no-sandbox", "disable-dev-shm-usage" });
                        _driverOptions = chromeOptions;
                        //driver = new ChromeDriver(chromeService, chromeOptions, TimeSpan.FromSeconds(60));
                        //driver.Manage().Window.Size = new System.Drawing.Size(1296, 696);
                        Task.Factory.StartNew(() =>
                        {
                            while (drivers.Count < _config.WebDriverProcessCount)
                            {
                                var webdriver = new ChromeDriver((ChromeDriverService)_driverService, chromeOptions, TimeSpan.FromSeconds(_config.WebDriverTimeoutInSeconds));
                                drivers.Add(webdriver);
                                _logger.LogDebug("just added a new web driver, now drivers count {count}", drivers.Count);
                            }
                        });
                        break;
                    }
                case Browser.Firefox:
                    {
                        FirefoxDriverService firefoxService = FirefoxDriverService.CreateDefaultService(WebDriverExePath);
                        firefoxService.HideCommandPromptWindow = true;
                        firefoxService.SuppressInitialDiagnosticInformation = true;
                        FirefoxOptions firefoxOptions = new FirefoxOptions();
                        firefoxOptions.AddArguments(new[] { "disable-infobars", "headless", "silent", "log-level=3", "no-sandbox", "disable-dev-shm-usage" });
                        driver = new FirefoxDriver(firefoxService, firefoxOptions, TimeSpan.FromSeconds(60));
                        driver.Manage().Window.Size = new System.Drawing.Size(1296, 696);
                        break;
                    }
                case Browser.IE:
                    {
                        InternetExplorerDriverService ieService = InternetExplorerDriverService.CreateDefaultService(WebDriverExePath);
                        ieService.HideCommandPromptWindow = true;
                        ieService.SuppressInitialDiagnosticInformation = true;
                        driver = new InternetExplorerDriver(ieService);
                        break;
                    }
                case Browser.Edge:
                    {
                        EdgeDriverService edgeService = EdgeDriverService.CreateDefaultService(WebDriverExePath);
                        edgeService.HideCommandPromptWindow = true;
                        edgeService.SuppressInitialDiagnosticInformation = true;
                        driver = new EdgeDriver(edgeService);
                        break;
                    }
                case Browser.Safari:
                    {
                        SafariDriverService safariService = SafariDriverService.CreateDefaultService(WebDriverExePath);
                        safariService.HideCommandPromptWindow = true;
                        safariService.SuppressInitialDiagnosticInformation = true;
                        driver = new SafariDriver(safariService);
                        break;
                    }
            }
        }
        
        private Task<IWebDriver> GetWebDriverInstance()
        {
            IWebDriver driver = drivers.FirstOrDefault();
            while (driver == null)
            {
                Task.Delay(10000).Wait();
                _logger.LogInformation("wating one second to get web driver instance");
                driver = drivers.FirstOrDefault();
            }            

            return Task.FromResult(driver);            
        }

        public async Task<HttpResponse> CollectAsync(HttpRequest request)
        {
            HttpResponse response = new HttpResponse { Success = true };
            try
            {
                var webDriver = await GetWebDriverInstance();
                webDriver.Navigate().GoToUrl(request.Url);
                WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(_config.WebDriverTimeoutInSeconds));
                wait.Until(driver =>
                    ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                response.Content = webDriver.PageSource;
                response.ContentType = request.ContentType;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "collect error in {collector}", nameof(WebDriverCollector));
                response.Success = false;
                response.ErrorMsg = e.Message;                
            }

            return response;
        }

        public void Dispose()
        {
            foreach (var driver in drivers)
            {                
                driver.Quit();
                driver.Dispose();
            }

            _driverService?.Dispose();

            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                process.Kill();
            }
        }
    }
}
