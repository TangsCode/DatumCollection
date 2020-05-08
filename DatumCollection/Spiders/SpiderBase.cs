using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using HtmlAgilityPack;
using System.Net;
using DatumCollection.Utility.Extensions;
using DatumCollection.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using DatumCollection.Utility.Helper;
using System.Diagnostics;
using System.Collections.Concurrent;
using DatumCollection.WebDriver;
using DatumCollection.EventBus;
using DatumCollection.Logger;
using System.Threading;
using DatumCollection.SpiderScheduler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using DatumCollection.Quartz;

namespace DatumCollection.Spiders
{
    /// <summary>
    /// 爬虫基类
    /// </summary>
    public class SpiderBase : AbstractSpider, IDisposable
    {
        public SpiderBase(SpiderParameters parameters) : base(parameters)
        {
        }

        protected virtual void Initialize()
        {            
            
        }

        public override async Task RunAsync()
        {            
            try
            {
                //MessageQueue.Subscribe($"{Options.TopicResponseHandler}{Id}",
                //    async message => await HandleMessageAsync(message)); 
                #region Quartz Job
                //IJobDetail job = JobBuilder.Create<ScheduleObserverJob>()
                //    .WithIdentity("schedule", "spider")
                //    .Build();
                //ITrigger trigger = TriggerBuilder.Create()
                //    .WithIdentity("trigger", "spider")
                //    .StartNow()
                //    .WithSimpleSchedule(s =>
                //    s.WithIntervalInSeconds(60)
                //    .RepeatForever())
                //    .Build();
                //await _scheduler.ScheduleJob(job, trigger);
                #endregion
                bool @break = false;
                while (!@break)
                {
                    try
                    {
                        await WaitForExit();
                    }
                    catch (Exception)
                    {
                        @break = true;
                    }

                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            finally
            {
                this.Dispose();
            }
            
        }
        

        private Task WaitForExit()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            var backgroundServices = _services.GetRequiredService<IEnumerable<IHostedService>>();
            foreach (IHostedService service in backgroundServices)
            {
                service.StopAsync(CancellationToken.None).ConfigureAwait(true).GetAwaiter().GetResult();
            }
        }

    }
}
