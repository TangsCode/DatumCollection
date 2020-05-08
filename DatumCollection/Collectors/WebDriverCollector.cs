using DatumCollection.Common;
using DatumCollection.Data;
using DatumCollection.EventBus;
using DatumCollection.Utility.Extensions;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Edge;
using DatumCollection.Utility.Helper;
using System.Dynamic;
using System.Collections.Concurrent;
using System.Linq;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading;
using DatumCollection.WebDriver;
using DatumCollection.ImageRecognition;

namespace DatumCollection.Collectors
{
    public class WebDriverCollector : ICollector
    {
        protected readonly ILogger<WebDriverCollector> _logger;
        protected readonly IEventBus _mq;
        protected readonly IDataStorage _storage;
        protected readonly SystemOptions _options;
        protected readonly IRecognizer _recognizer;

        //webdriver instance
        protected IWebDriver driver;

        //webdriver collection(thread-safe)
        protected ConcurrentBag<WebDriverBase> drivers = new ConcurrentBag<WebDriverBase>();

        //driver executable file path
        protected string DriverPath =>
            _options.WebDriverPath.NotNull() ?
            _options.WebDriverPath : Path.Join(SpiderEnvironment.BaseDirectory, "SeleniumDrivers");
        protected string ScreenshotPath =>
            _options.ScreenshotPath.NotNull()
            ? Path.Join(_options.ScreenshotPath, DateTimeHelper.TodayString)
            : Path.Join(SpiderEnvironment.BaseDirectory, "screenshots", DateTimeHelper.TodayString);

        //webdriver timeout
        protected int WebdriverTimeout =>
            _options.WebDriverTimeoutSeconds;

        public WebDriverCollector(
            ILogger<WebDriverCollector> logger,
            IEventBus mq,
            IDataStorage storage,
            SystemOptions options,
            IRecognizer recognizer
            )
        {
            _logger = logger;
            _mq = mq;
            _storage = storage;
            _options = options;
            _recognizer = recognizer;

            if (!Directory.Exists(ScreenshotPath))
            {
                Directory.CreateDirectory(ScreenshotPath);
            }
            var browser = Enum.Parse(typeof(Browser), _options.Browser ?? Browser.Chrome.ToString());
            switch (browser)
            {
                case Browser.Chrome:
                    {
                        ChromeDriverService chromeService = ChromeDriverService.CreateDefaultService(DriverPath);
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
                        break;
                    }
                case Browser.Firefox:
                    {
                        FirefoxDriverService firefoxService = FirefoxDriverService.CreateDefaultService(DriverPath);
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
                        InternetExplorerDriverService ieService = InternetExplorerDriverService.CreateDefaultService(DriverPath);
                        ieService.HideCommandPromptWindow = true;
                        ieService.SuppressInitialDiagnosticInformation = true;
                        driver = new InternetExplorerDriver(ieService);
                        break;
                    }
                case Browser.Edge:
                    {
                        EdgeDriverService edgeService = EdgeDriverService.CreateDefaultService(DriverPath);
                        edgeService.HideCommandPromptWindow = true;
                        edgeService.SuppressInitialDiagnosticInformation = true;
                        driver = new EdgeDriver(edgeService);
                        break;
                    }
            }
        }

        public CollectResult Collect(CollectContext context)
        {
            CollectResult collectResult = new CollectResult(context.Sources.Count());
            try
            {
                _logger.LogInformation($"开始采集数据.....\r\n采集类型{context.CollectType.ToString()}");
                switch (context.CollectType)
                {
                    case CollectType.Goods:
                        {
                            
                        }
                        break;
                    case CollectType.Shop:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"采集数据发生错误。错误详情：{e.ToString()}");
            }

            return collectResult;
        }

        public Task<CollectResult> CollectAsync(CollectContext context)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            driver?.Quit();
            driver?.Dispose();
        }
    }
}
