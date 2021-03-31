using DatumCollection.Configuration;
using DatumCollection.Data;
using DatumCollection.Data.Entities;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.Infrastructure.Web;
using DatumCollection.MessageQueue;
using DatumCollection.Utility.Helper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatumCollection.HostedServices.Schedule
{
    public class SpiderScheduleHostedService<T> : IHostedService where T : ISpider, new()
    {
        private readonly ILogger<SpiderScheduleHostedService<T>> _logger;
        private readonly IMessageQueue _mq;
        private readonly IDataStorage _storage;
        private readonly SpiderClientConfiguration _config;

        public SpiderScheduleHostedService(
            ILogger<SpiderScheduleHostedService<T>> logger,
            IMessageQueue mq,
            IDataStorage storage,
            SpiderClientConfiguration config)
        {
            _logger = logger;
            _mq = mq;
            _storage = storage;
            _config = config;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{service} is stopping", nameof(SpiderScheduleHostedService<T>));
            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{service} is starting", nameof(SpiderScheduleHostedService<T>));

            cancellationToken.Register(() =>
            {
                _logger.LogInformation("{service} is cancelling", nameof(SpiderScheduleHostedService<T>));
            });

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("{service} is heartbeating...", nameof(SpiderScheduleHostedService<T>));
                var schedules = await _storage.Query<SpiderScheduleSetting>((schedule) => schedule.OnSchedule() && !schedule.IsDelete);
                if (schedules != null && schedules.Count() > 0)
                {
                    _logger.LogInformation("schedules retriving, count {count}", schedules.Count());
                    Parallel.ForEach(schedules, async schedule =>
                    {
                        var scheduleItems = await _storage.Query<SpiderScheduleItems, SpiderItem<T>, Channel>(
                            (scheduleitem, item, channel) =>
                            {                                
                                item.Channel = channel;
                                scheduleitem.SpiderItem = item;                                
                                return scheduleitem;
                            },si => si.FK_SpiderSchedule_ID == schedule.ID
                            );
                        var items = scheduleItems.Select(si => (SpiderItem<T>)si.SpiderItem);
                        if (items.Any())
                        {
                            var spiderContext = new SpiderContext(schedule.ID);
                            var spiderFields = schedule.GetType().GetProperties().Where(p => p.Name.ToString().StartsWith("get") && (bool)p.GetValue(schedule) == true).Select(p => p.Name.Replace("get", ""));
                            foreach (var item in items)
                            {
                                var atom = new SpiderAtom
                                {
                                    Request = new HttpRequest
                                    {
                                        Url = item.Url,
                                        Method = item.Method ?? "Get",
                                        ContentType = (ContentType)Enum.Parse(typeof(ContentType), item.ContentType ?? ContentType.Html.ToString(), true),
                                        Encoding = Encoding.GetEncoding(item.Encoding ?? "utf-8")
                                    },
                                    SpiderItem = item,
                                    Model = new T(),
                                    SpiderFields = spiderFields
                                };
                                var prop = typeof(T).GetProperties()?.FirstOrDefault(p => p.Name == "FK_SpiderItem_ID");
                                prop?.SetValue(atom.Model, ((SpiderSource)atom.SpiderItem).ID);
                                var spiderTaskProp = typeof(T).GetProperties()?.FirstOrDefault(p => p.Name == "FK_SpiderTask_ID");
                                spiderTaskProp?.SetValue(atom.Model, spiderContext.Task.Id);
                                spiderContext.SpiderAtoms.Add(atom);
                            }

                            await _mq.PublishAsync(MessageType.SpiderRequest.ToString(), new Message
                            {
                                Data = spiderContext,
                                PublishTime = (long)DateTimeHelper.GetCurrentUnixTimeNumber()
                            });
                        }
                    });
                }
                await Task.Delay(TimeSpan.FromSeconds(_config.ScheduleQueryFrequency), cancellationToken);
            }

        }
    }
}
