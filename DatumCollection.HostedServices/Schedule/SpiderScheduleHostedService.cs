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
    public class SpiderScheduleHostedService<T> : IHostedService where T : ISpider
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
                        var scheduleItems = await _storage.Query<SpiderScheduleItems<T>, SpiderItem<T>, Channel >(
                            (scheduleitem, item, channel ) =>
                            {
                                item.Channel = channel;
                                scheduleitem.SpiderItem = item;
                                scheduleitem.SpiderScheduleSetting = schedule;
                                return scheduleitem;
                            }
                            );
                        var items = scheduleItems.Select(si => si.SpiderItem);
                        if (items.Any())
                        {
                            var spiderContext = new SpiderContext();
                            foreach (var item in items)
                            {
                                var atom = new SpiderAtom
                                {
                                    SpiderItem = item,
                                    Request = new HttpRequest
                                    {
                                        Url = item.Url,
                                        Method = item.Method ?? "Get",
                                        ContentType = (ContentType)Enum.Parse(typeof(ContentType), item.ContentType ?? ContentType.Html.ToString(), true),
                                        Encoding = Encoding.GetEncoding(item.Encoding ?? "utf-8")
                                    }
                                };
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
