using DatumCollection.Configuration;
using DatumCollection.Data;
using DatumCollection.MessageQueue;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatumCollection.HostedServices.Schedule
{
    public class SpiderScheduleHostedService : IHostedService
    {
        private readonly ILogger<SpiderScheduleHostedService> _logger;
        private readonly IMessageQueue _mq;
        private readonly IDataStorage _storage;
        private readonly SpiderClientConfiguration _config;

        public SpiderScheduleHostedService(
            ILogger<SpiderScheduleHostedService> logger,
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

            _logger.LogInformation("{service} is stopping", nameof(SpiderScheduleHostedService));
            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{service} is starting", nameof(SpiderScheduleHostedService));

            cancellationToken.Register(() =>
            {
                _logger.LogInformation("{service} is stopping", nameof(SpiderScheduleHostedService));
            });

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("{service} is executing background task", nameof(SpiderScheduleHostedService));
                var schedules = await _storage.Query<SpiderScheduleSetting>();
                foreach (var schedule in schedules)
                {
                    if (schedule.OnSchedule())
                    {
                        //await _mq.PublishAysnc("", new Message
                        //{
                        //    Data = schedule,
                        //    PublishTime = DateTime.Now
                        //});
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(_config.ScheduleQueryFrequency), cancellationToken);
            }

        }
    }
}
