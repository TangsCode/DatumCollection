using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DatumCollection.Configuration;
using DatumCollection.Data.Entities;
using DatumCollection.Infrastructure.Abstraction;
using DatumCollection.Infrastructure.Selectors;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.Infrastructure.Web;
using DatumCollection.MessageQueue;
using DatumCollection.Utility.Helper;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;

namespace DatumCollection.Pipline.Collectors
{
    public class WebDriverCollector : ICollector
    {
        private readonly ILogger<WebDriverCollector> _logger;
        private readonly IMessageQueue _mq;
        private readonly SpiderClientConfiguration _config;

        private IWebDriver driver;
        private ConcurrentBag<ManagedWebDriver> _drivers;
        private DriverService _driverService;
        private DriverOptions _driverOptions;

        private Browser _browser;
        private bool _initialized;
        private int _lock;
        private SemaphoreSlim _pool;

        private readonly HttpClient _client; 

        #region performance optimization
        private ConcurrentDictionary<Type, PropertyInfo[]> _props = new ConcurrentDictionary<Type, PropertyInfo[]>();
        //private ConcurrentDictionary<Type, SelectorAttribute> _target = new ConcurrentDictionary<Type, SelectorAttribute>();
        //private ConcurrentDictionary<Type, IEnumerable<SelectorAttribute>> _selectors = new ConcurrentDictionary<Type, IEnumerable<SelectorAttribute>>();
        #endregion

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

            _client = new HttpClient();
            _pool = new SemaphoreSlim(_config.WebDriverProcessCount, _config.WebDriverProcessCount);            
        }

        private async Task InitializeAsync()
        {
            if(Interlocked.Exchange(ref _lock, 1) == 0)
            {
                if (!Directory.Exists(ImagePath))
                {
                    Directory.CreateDirectory(ImagePath);
                }
                _drivers = new ConcurrentBag<ManagedWebDriver>();
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
                            if (_config.IsUseProxy)
                            {
                                Proxy proxy = new Proxy();
                                proxy.Kind = ProxyKind.Manual;
                                ProxyProtocol protocol = (ProxyProtocol)Enum.Parse(typeof(ProxyProtocol), _config.ProxyProtocol, true);
                                switch (protocol)
                                {
                                    case ProxyProtocol.Http:
                                        proxy.HttpProxy = _config.ProxyAddress;
                                        break;
                                    case ProxyProtocol.SSL:
                                        proxy.SslProxy = _config.ProxyAddress;
                                        break;
                                    case ProxyProtocol.Ftp:
                                        proxy.FtpProxy = _config.ProxyAddress;
                                        break;
                                    case ProxyProtocol.Socks:
                                        proxy.SocksProxy = _config.ProxyAddress;
                                        break;
                                }
                                chromeOptions.Proxy = proxy;
                            }
                            #endregion
                            chromeOptions.AddArguments(new[] { "disable-infobars", "headless", "silent", "log-level=3", "no-sandbox", "disable-dev-shm-usage" });
                            _driverOptions = chromeOptions;                                             
                            await Task.Factory.StartNew(() =>
                            {
                                while (_drivers.Count < _config.WebDriverProcessCount)
                                {
                                    var webdriver = new ChromeDriver((ChromeDriverService)_driverService, chromeOptions, TimeSpan.FromSeconds(_config.WebDriverTimeoutInSeconds));
                                    webdriver.Manage().Window.Size = new System.Drawing.Size(1296, 696);
                                    _drivers.Add(new ManagedWebDriver(webdriver));
                                    _logger.LogInformation("just added a new web driver, now drivers count {count}", _drivers.Count);
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
                _initialized = true;
                Interlocked.Exchange(ref _lock, 0);
            }  
        }
        
        private async Task<ManagedWebDriver> GetWebDriverInstance()
        {
            await _pool.WaitAsync();
            ManagedWebDriver driver = null;
            try
            {
                while (driver == null)
                {
                    driver = _drivers.FirstOrDefault(md => !md.Used);
                    Thread.SpinWait(1);
                }
                
                driver.Used = true;
            }
            catch (Exception e)
            {
                
            }
            finally
            {
                _pool.Release();
            }                        
            
            return driver;                                   
        }

        public async Task CollectAsync(SpiderAtom atom)
        {
            if (!_initialized)
            {
                await InitializeAsync();
            }
            atom.Response = new HttpResponse { Success = true };
            ManagedWebDriver md = null;
            try
            {
                md = await GetWebDriverInstance();                
                var webDriver = md.WebDriver;                
                webDriver.Navigate().GoToUrl(atom.Request.Url);
                webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(_config.WebDriverTimeoutInSeconds));            
                wait.IgnoreExceptionTypes(new Type[] { typeof(NoSuchElementException) });
                var target = atom.SpiderItem.GetTargetSelector().Result;
                wait.Until(webdriver => webdriver.FindElement(By.XPath(target.Path)).Displayed);                
                #region it is unnecessary to extract data in extractor after extraxting data in collector 
                var js = (IJavaScriptExecutor)webDriver;
                //close popup window
                if (((SpiderSource)atom.SpiderItem).Channel.ChannelCode.ToLower() == "tmall")
                {
                    var els = webDriver.FindElements(By.XPath
                        ("//div[@class='baxia-dialog-close']|//div[@id='sufei-dialog-close']"));
                    if (els != null && els.Any())
                    {
                        foreach (var el in els)
                        {
                            js.ExecuteScript("arguments[0].click()", el);
                        }                        
                    }
                }
                var type = atom.Model.GetType();
                var props = _props.GetOrAdd(type, type.GetProperties());
                var selectors = atom.SpiderItem.GetAllSelectors().Result;
                selectors = selectors.Where(selector => !string.IsNullOrEmpty(selector.Path)
                        && props.Any(p => p.Name.ToLower() == selector.Key.ToLower())
                        && atom.SpiderFields.Contains(selector.Key));
                foreach (var selector in selectors)
                {
                    object o = null;
                    //screensshot
                    if (selector.Key == "Screenshot")
                    {                        
                        DatumCollection.Utility.Helper.IO.ensureLocalDirectory(_config.ImageDownloadPath);
                        //scroll to the scope where price is visible 
                        IWebElement screenshotElement = webDriver.FindElement(By.XPath(selector.Path));
                        if (screenshotElement != null)
                        {
                            var scrollscript = $"arguments[0].scrollIntoView();";
                            js.ExecuteScript(scrollscript, screenshotElement);
                        }
                        ITakesScreenshot screenshotDriver = webDriver as ITakesScreenshot;
                        Screenshot screenshot = screenshotDriver.GetScreenshot();
                        string fileName = $"screenshot_{((SpiderSource)atom.SpiderItem).ID}_{DateTime.Now.ToString("HHmmssfff")}.png";
                        string filePath = Path.Combine(_config.ImageDownloadPath, fileName);
                        screenshot.SaveAsFile(filePath, ScreenshotImageFormat.Png);
                        o = fileName;
                    }
                    if (selector.Key == "ImageFile")
                    {
                        DatumCollection.Utility.Helper.IO.ensureLocalDirectory(_config.ImageDownloadPath);
                        ReadOnlyCollection<IWebElement> imageElements = webDriver.FindElements(By.XPath(selector.Path));
                        if (imageElements != null && imageElements.Count > 0)
                        {
                            try
                            {
                                List<string> images = new List<string>();
                                foreach (var img in imageElements)
                                {
                                    string src = img.GetAttribute("src");
                                    //京东规则
                                    src = src.RegexReplace("/n[0-9]{1}/", "/n0/");
                                    src = src.RegexReplace("(/s[0-9]{2,3}x{1}[0-9]{2,3}_jfs/)", "/800x800_jfs/");
                                    //阿里规则
                                    src = src.RegexReplace("_[0-9]{2,3}x[0-9]{2,3}", "_800x800");
                                    _logger.LogInformation(string.Format("src:{0}", new string[] { src }));
                                    string fileName = $"image_{((SpiderSource)atom.SpiderItem).ID}_{DateTime.Now.ToString("HHmmssfff")}{src.Match(".(jpg|jpeg|png|gif)")}";
                                    string filePath = Path.Combine(_config.ImageDownloadPath, fileName);
                                    var res = await _client.GetAsync(src.Match("(https|http)://(\\S+?)\\.(jpg|jpeg|png|gif)"));
                                    using (var memStream = res.Content.ReadAsStreamAsync().Result)
                                    {
                                        using (var fileStream = File.Create(filePath))
                                        {
                                            memStream.CopyTo(fileStream);
                                        }
                                    }
                                    images.Add(fileName);
                                }
                                o = string.Join(";", images);
                            }
                            catch (Exception e)
                            {

                                throw;
                            }
                            //using (WebClient client = new WebClient())
                            //{
                            //    List<string> images = new List<string>();
                            //    foreach (var img in imageElements)
                            //    {
                            //        string src = img.GetAttribute("src");
                            //        //京东规则
                            //        src = src.RegexReplace("/n[0-9]{1}/", "/n0/");
                            //        src = src.RegexReplace("(/s[0-9]{2,3}x{1}[0-9]{2,3}_jfs/)", "/800x800_jfs/");
                            //        //阿里规则
                            //        src = src.RegexReplace("_[0-9]{2,3}x[0-9]{2,3}", "_800x800");
                            //        _logger.LogInformation(string.Format("src:{0}", new string[] { src }));
                            //        string fileName = $"image_{((SpiderSource)atom.SpiderItem).ID}_{DateTime.Now.ToString("HHmmssfff")}{src.Match(".(jpg|jpeg|png|gif)")}";
                            //        string filePath = Path.Combine(_config.ImageDownloadPath, fileName);
                            //        client.DownloadFile(new Uri(src.Match("(https|http)://(\\S+?)\\.(jpg|jpeg|png|gif)")), filePath);
                            //        images.Add(fileName);
                            //    }
                            //    o = string.Join(";", images);                                
                            //}

                        }
                    }
                    if(o == null)
                    {
                        switch (selector.Type)
                        {
                            case SelectorType.XPath:
                                {
                                    var elements = webDriver.FindElements(By.XPath(selector.Path));
                                    o = elements?.FirstOrDefault()?.Text;
                                }
                                break;
                            case SelectorType.Html:
                                {
                                    var elements = webDriver.FindElements(By.XPath(selector.Path));
                                    o = elements?.FirstOrDefault()?.Text;
                                }
                                break;
                            case SelectorType.Json:
                                break;
                            default:
                                break;
                        }
                    }
                    var prop = props.FirstOrDefault(p => p.Name.ToLower() == selector.Key.ToLower());
                    if (o != null && !string.IsNullOrEmpty(o.ToString()))
                    {                        
                        prop?.SetValue(atom.Model, Convert.ChangeType(o, prop.PropertyType));
                    }
                }                                

                #endregion
            }
            catch (Exception e)
            {
                _logger.LogError(e, "collect error in {collector}", nameof(WebDriverCollector));
                atom.SpiderStatus = SpiderStatus.CollectError;
                atom.Response.Success = false;
                atom.Response.ErrorMsg = e.Message;
                //await _mq.PublishAsync(_config.TopicStatisticsFail, new Message
                //{
                //    MessageType = ErrorMessageType.CollectorError.ToString(),
                //    Data = atom
                //});
            }
            finally
            {
                if (md != null)
                {
                    md.Used = false;
                }                
            }
        }

        public void Dispose()
        {
            foreach (var driver in _drivers)
            {
                driver.WebDriver.Quit();
                driver.WebDriver.Dispose();
            }

            _driverService?.Dispose();
        }
    }

    public class ManagedWebDriver
    {
        public ManagedWebDriver(IWebDriver driver)
        {
            WebDriver = driver;
            Used = false;
        }

        public IWebDriver WebDriver { get; }

        public bool Used { get; set; }
    }
}
