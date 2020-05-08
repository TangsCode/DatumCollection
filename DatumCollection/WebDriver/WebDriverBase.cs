using DatumCollection.Common;
using DatumCollection.ImageRecognition;
using DatumCollection.Utility.Extensions;
using DatumCollection.Utility.Helper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.WebDriver
{

    public class WebDriverBase : IDisposable
    {
        #region Field

        //webdriver instace
        protected IWebDriver driver;

        //webdriver service
        protected ChromeDriverService chromeDriverService;

        //webdriver options
        protected ChromeOptions driverOptions = new ChromeOptions();

        protected bool IsInitialized = false;

        public bool IsRunning = false;

        public IWebDriver Instance { get { return driver; } }
        #endregion

        #region Property
        SystemOptions Options { get; set; }

        //screenshot file stored folder
        protected string ScreenshotPath =>
            Options.ScreenshotPath.NotNull()
            ? Path.Join(Options.ScreenshotPath, DateTimeHelper.TodayString)
            : Path.Join(SpiderEnvironment.BaseDirectory, "screenshots", DateTimeHelper.TodayString);

        //driver executable file path
        protected string DriverPath =>
            Options.WebDriverPath.NotNull() && File.Exists(Path.Join(Options.WebDriverPath,"chromedriver.exe")) ?
            Options.WebDriverPath : Path.Join(SpiderEnvironment.BaseDirectory, "SeleniumDrivers");

        //webdriver timeout
        protected int WebdriverTimeout =>
            Options.WebDriverTimeoutSeconds;

        public dynamic Config { get; set; }

        public ILogger Logger { get; set; }
        #endregion

        public WebDriverBase(SystemOptions options)
        {
            Options = options;
        }

        ~WebDriverBase()
        {
            this.Dispose();
        }

        internal void Initialize()
        {            
            //driver setting            
            chromeDriverService = ChromeDriverService.CreateDefaultService(DriverPath);
            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            driverOptions.AddArguments(new[] { "disable-infobars", "headless", "silent", "log-level=3", "no-sandbox", "disable-dev-shm-usage" });
            driver = new ChromeDriver(chromeDriverService, driverOptions, TimeSpan.FromSeconds(60));
            //driver = new ChromeDriver(DriverPath, driverOptions);
            driver.Manage().Window.Size = new System.Drawing.Size(1296, 696);            
            IsInitialized = true;
        }

        public IEnumerable<dynamic> CollectDataByShop(dynamic source)
        {
            IsRunning = true;
            List<dynamic> ret = new List<dynamic>();

            try
            {
                if (!IsInitialized) { Initialize(); }
                //int currentPage = 1;
                //find the real link where all goods are shown
                string url = source.ShopUrl.Value;
                driver.Navigate().GoToUrl(url);
                if (!string.IsNullOrEmpty(source.AllGoodsXPath?.Value))
                {
                    IWebElement element = driver.FindElement(By.XPath(source.AllGoodsXPath.Value));
                    //ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.TagName("area"));
                    url = element.GetAttribute("href");
                }
                else
                {
                    string regex = source.ShopUrlRegex.Value;
                    string replace = source.ShopUrlRegexReplace.Value;
                    url = url.RegexReplace(regex, replace);
                }                
                driver.Navigate().GoToUrl(url);
                //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WebdriverTimeout));
                //IEnumerable<IWebElement> elements = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(source.GoodsXPath.Value)));
                IEnumerable<IWebElement> goodsElements = driver.FindElements(By.XPath(source.GoodsXPath.Value));
                foreach (var element in goodsElements)
                {
                    dynamic goods = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(source));
                    goods.GoodsUrl = element.GetAttribute("href");
                    goods.SkuName = element.GetAttribute("text");
                    ret.Add(goods);
                }

                //IWebElement pageElement = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(source.PaginationXPath.Value)));

            }
            catch (Exception e)
            {
                Logger.LogError($"门店抓取失败，错误：{e.ToString()}");
            }
            finally
            {
                IsRunning = false;
            }

            return ret;
        }

        public Result ScrapeData(dynamic goods)
        {
            Result result = new Result { Success = false };

            try
            {
                IsRunning = true;                
                if (!IsInitialized) { Initialize(); }
                //creates screenshot folder
                if (ScreenshotPath.NotNull() && !Directory.Exists(ScreenshotPath))
                {
                    Directory.CreateDirectory(ScreenshotPath);
                }
                IWebElement element = null;
                dynamic data = new ExpandoObject();
                data.CreateTime = DateTime.Now;

                var url = goods.GoodsUrl.Value;
                driver.Navigate().GoToUrl(url);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WebdriverTimeout));
                element = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(goods.PriceXPath.Value)));
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                if (goods.CloseXPath != null && !string.IsNullOrEmpty(goods.CloseXPath.Value))
                {
                    IWebElement closeElement = driver.FindElement(By.XPath(goods.CloseXPath.Value));
                    closeElement.Click();
                }                
                //price if detected
                decimal.TryParse(RegexHelper.Decimal.Match(element.Text).Value, out decimal price);
                data.Price = price;
                //Tax fee if detected
                if (goods.TaxFeeXPath != null && !string.IsNullOrEmpty(goods.TaxFeeXPath.Value))
                {
                    ReadOnlyCollection<IWebElement> taxElements = driver.FindElements(By.XPath(goods.TaxFeeXPath.Value));
                    if (taxElements != null && taxElements.Count > 0)
                    {
                        var taxVal = RegexHelper.Decimal.Match(taxElements[0].Text).Value;
                        decimal.TryParse(taxVal, out decimal tax);
                        data.TaxFee = tax;
                    }
                }
                //postage if detected
                if (goods.PostageXPath != null && !string.IsNullOrEmpty(goods.PostageXPath.Value))
                {
                    ReadOnlyCollection<IWebElement> postageElements = driver.FindElements(By.XPath(goods.PostageXPath.Value));
                    if (postageElements != null && postageElements.Count > 0)
                    {
                        data.PostageDesc = postageElements[0].GetAttribute("innerText");          
                    }
                }
                //preferential if detected
                if (goods.PreferentialXPath != null && !string.IsNullOrEmpty(goods.PreferentialXPath.Value))
                {
                    ReadOnlyCollection<IWebElement> prefElements = driver.FindElements(By.XPath(goods.PreferentialXPath.Value));
                    if (prefElements != null && prefElements.Count > 0)
                    {
                        data.PreferentialInfo = string.Join(';', prefElements.Where(e => e.Text.NotNull()).Select(e => e.Text).ToArray());
                    }
                }
                //cupon if detected
                if (goods.CouponXPath != null && !string.IsNullOrEmpty(goods.CouponXPath.Value))
                {
                    ReadOnlyCollection<IWebElement> couponElements = driver.FindElements(By.XPath(goods.CouponXPath.Value));
                    if (couponElements != null && couponElements.Count > 0)
                    {
                        data.CouponInfo = string.Join(';', couponElements.Where(e => e.Text.NotNull()).Select(e => e.Text).ToArray());
                    }
                }
                //download images of goods                 
                if(goods.ImageUrlXPath != null && !string.IsNullOrEmpty(goods.ImageUrlXPath.Value))
                {
                    ReadOnlyCollection<IWebElement> imageElements = driver.FindElements(By.XPath(goods.ImageUrlXPath.Value));
                    if(imageElements != null && imageElements.Count > 0)
                    {
                        //图像识别
                        //foreach (var image in imageElements)
                        //{
                        //    string src = image.GetAttribute("src");
                        //    //京东规则
                        //    src = src.RegexReplace("/n[0-9]{1}/", "/n0/");
                        //    src = src.RegexReplace("(/s[0-9]{2,3}x{1}[0-9]{2,3}_jfs/)", "/800x800_jfs/");
                        //    //阿里规则
                        //    src = src.RegexReplace("_[0-9]{2,3}x[0-9]{2,3}", "_800x800");
                        //    Logger.LogInformation(string.Format("src:{0}", new string[] { src }));
                        //    string text = RecognizerService.Recognize(src);
                        //    data.ImageText = text;
                        //}
                        using (WebClient client = new WebClient())
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
                                Logger.LogInformation(string.Format("src:{0}", new string[] { src }));
                                string fileName = $"{goods.FK_ScheduleConfig_ID.Value}_{goods.SystemId}_{DateTime.Now.ToString("HHmmssfff")}{src.Match(".(jpg|jpeg|png|gif)")}";
                                string filePath = Path.Join(ScreenshotPath, fileName);
                                client.DownloadFile(new Uri(src.Match("(https|http)://(\\S+?)\\.(jpg|jpeg|png|gif)")), filePath);
                                images.Add(string.Join('/', filePath.Split('\\').TakeLast(3)));
                            }
                            data.ImagePath = images;
                        }
                        
                    }
                }
                //take screenshot 
                if (goods.IsTakeScreenshot.Value ?? false)
                {
                    //scroll to the scope where price is visible 
                    ReadOnlyCollection<IWebElement> screenshotElements = driver.FindElements(By.XPath(goods.ScreenshotXPath.Value));
                    if (screenshotElements != null && screenshotElements.Count > 0)
                    {
                        var scrollscript = $"arguments[0].scrollIntoView();";
                        js.ExecuteScript(scrollscript, screenshotElements[0]);
                    }
                    ITakesScreenshot screenshotDriver = driver as ITakesScreenshot;
                    Screenshot screenshot = screenshotDriver.GetScreenshot();
                    string fileName = $"{goods.FK_ScheduleConfig_ID.Value}_{goods.SystemId}_{DateTime.Now.ToString("HHmmssfff")}.png";
                    string filePath = Path.Join(ScreenshotPath, fileName);
                    screenshot.SaveAsFile(filePath, ScreenshotImageFormat.Png);
                    data.ScreenshotPath = string.Join('/', filePath.Split('\\').TakeLast(3));
                }
                if (data.Price == null | data.Price == 0)
                {
                    result.ErrorMessage = $"fail to scrape data on web page.{data}";
                    return result;
                }
                result.Data = data;
                result.Success = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.ToString();                
            }
            finally
            {
                IsRunning = false;
            }
            return result;
        }

        public Task<Result> ScrapeDataAsync(dynamic goods)
        {
            var result = new Result { Success = false };

            try
            {
                IsRunning = true;
                if (!IsInitialized) { Initialize(); }

                IWebElement element = null;
                dynamic data = new ExpandoObject();
                data.CreateTime = DateTime.Now;

                var url = goods.GoodsUrl;                
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                driver.Navigate().GoToUrl(url);

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WebdriverTimeout));
                element = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(goods.PriceXPath)));
                
                if (goods.CloseXPath != null && !string.IsNullOrEmpty(goods.CloseXPath))
                {
                    IWebElement closeElement = driver.FindElement(By.XPath(goods.CloseXPath));
                    closeElement.Click();
                }
                //price if detected
                decimal.TryParse(RegexHelper.Decimal.Match(element.Text).Value, out decimal price);
                data.Price = price;
                //Tax fee if detected
                if (goods.TaxFeeXPath != null && !string.IsNullOrEmpty(goods.TaxFeeXPath))
                {
                    ReadOnlyCollection<IWebElement> taxElements = driver.FindElements(By.XPath(goods.TaxFeeXPath));
                    if (taxElements != null && taxElements.Count > 0)
                    {
                        var taxVal = RegexHelper.Decimal.Match(taxElements[0].Text).Value;
                        decimal.TryParse(taxVal, out decimal tax);
                        data.TaxFee = tax;
                    }
                }
                //postage if detected
                if (goods.PostageXPath != null && !string.IsNullOrEmpty(goods.PostageXPath))
                {
                    ReadOnlyCollection<IWebElement> postageElements = driver.FindElements(By.XPath(goods.PostageXPath));
                    if (postageElements != null && postageElements.Count > 0)
                    {
                        data.PostageDesc = postageElements[0].GetAttribute("innerText");
                    }
                }
                //preferential if detected
                if (goods.PreferentialXPath != null && !string.IsNullOrEmpty(goods.PreferentialXPath))
                {
                    ReadOnlyCollection<IWebElement> prefElements = driver.FindElements(By.XPath(goods.PreferentialXPath));
                    if (prefElements != null && prefElements.Count > 0)
                    {
                        data.PreferentialInfo = string.Join(';', prefElements.Where(e => e.Text.NotNull()).Select(e => e.Text).ToArray());
                    }
                }
                //cupon if detected
                if (goods.CouponXPath != null && !string.IsNullOrEmpty(goods.CouponXPath))
                {
                    ReadOnlyCollection<IWebElement> couponElements = driver.FindElements(By.XPath(goods.CouponXPath));
                    if (couponElements != null && couponElements.Count > 0)
                    {
                        data.CouponInfo = string.Join(';', couponElements.Where(e => e.Text.NotNull()).Select(e => e.Text).ToArray());
                    }
                }

                //take screenshot 
                if (Config.IsTakeScreenshot ?? false)
                {
                    //scroll to the scope where price is visible 
                    ReadOnlyCollection<IWebElement> screenshotElements = driver.FindElements(By.XPath(goods.ScreenshotXPath));
                    if (screenshotElements != null && screenshotElements.Count > 0)
                    {
                        var scrollscript = $"arguments[0].scrollIntoView();";
                        js.ExecuteScript(scrollscript, screenshotElements[0]);
                    }
                    ITakesScreenshot screenshotDriver = driver as ITakesScreenshot;
                    Screenshot screenshot = screenshotDriver.GetScreenshot();
                    string fileName = $"{Config.HashCode}_{goods.SystemId}_{DateTime.Now.ToString("HHmmssfff")}.png";
                    string filePath = Path.Join(ScreenshotPath, fileName);
                    screenshot.SaveAsFile(filePath, ScreenshotImageFormat.Png);
                    data.ScreenshotPath = string.Join('/', filePath.Split('\\').TakeLast(3));
                }
                if (data.Price == null | data.Price == 0)
                {
                    result.ErrorMessage = $"fail to scrape data on web page.price:{data.Price}";
                    return Task.FromResult(result);
                }
                result.Data = data;
                result.Success = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.ToString();
            }
            finally
            {
                IsRunning = false;
            }

            return Task.FromResult(result);
        }
        public void Dispose()
        {
            driver?.Close();
            driver?.Dispose();
            chromeDriverService?.Dispose();
        }
    }
}
