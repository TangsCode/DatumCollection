using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
            _browser = (Browser)Enum.Parse(typeof(Browser), _config.Browser ?? Browser.Chrome.ToString());
            switch (_browser)
            {
                case Browser.Chrome:
                    {
                        ChromeDriverService chromeService = ChromeDriverService.CreateDefaultService(WebDriverExePath);
                        //disable command prompt output
                        chromeService.HideCommandPromptWindow = true;
                        chromeService.SuppressInitialDiagnosticInformation = true;
                        ChromeOptions chromeOptions = new ChromeOptions();
                        #region proxy setting
                        //Proxy proxy = new Proxy();
                        //proxy.Kind = ProxyKind.Manual;
                        #endregion
                        chromeOptions.AddArguments(new[] { "disable-infobars", "headless", "silent", "log-level=3", "no-sandbox", "disable-dev-shm-usage" });
                        driver = new ChromeDriver(chromeService, chromeOptions, TimeSpan.FromSeconds(60));
                        driver.Manage().Window.Size = new System.Drawing.Size(1296, 696);
                        for (int i = 0; i < _config.WebDriverProcessCount; i++)
                        {
                            var webdriver = new ChromeDriver(chromeService, chromeOptions, TimeSpan.FromSeconds(_config.WebDriverTimeoutInSeconds));
                            drivers.Add(webdriver);
                        }
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
        
        public Task<HttpResponse> CollectAsync(HttpRequest request)
        {
            HttpResponse response = new HttpResponse { Success = true };
            try
            {
                var webDriver = drivers.FirstOrDefault();
                webDriver.Navigate().GoToUrl(request.Url);
                WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(_config.WebDriverTimeoutInSeconds));
                wait.Until(driver =>
                    ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                response.Content = webDriver.PageSource;
                response.ContentType = ContentType.Html;                
            }
            catch (Exception e)
            {
                _logger.LogError(e, "collect error in {collector}", nameof(WebDriverCollector));
                response.Success = false;
                response.ErrorMsg = e.Message;                
            }

            return Task.FromResult(response);
        }
        
    }
}
