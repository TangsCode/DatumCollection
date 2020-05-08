using DatumCollection.Console;
using DatumCollection.Data;
using DatumCollection.EventBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Quartz
{
    /// <summary>
    /// 采集任务Job
    /// </summary>
    class ScheduleObserverJob : IJob
    {
        private ILogger<ScheduleObserverJob> _logger;

        private IDataStorage _storage;

        private IEventBus _mq;

        private SystemOptions _options;        

        public ScheduleObserverJob(
            ILogger<ScheduleObserverJob> logger,
            IDataStorage storage,
            IEventBus mq,
            SystemOptions options)
        {
            _logger = logger;
            _storage = storage;
            _mq = mq;
            _options = options;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"{this.ToString()} is running");
            try
            {
                var configList = _storage.Query<dynamic>(new SqlContext("ScheduleConfig")
                {
                    WhereClause = @"cast(getdate() as time)  between StartTime and EndTime 
                                    and datediff(minute, StartTime, cast(getdate() as time)) % IntervalMinutes = 0 
                                    and IsDisabled = 0 and IsDelete = 0"
                });
                if (configList.Count() > 0)
                {
                    int count = configList.Count();
                    _logger.LogInformation($"{count} tasks are going to run");

                    Parallel.ForEach(configList,
                        (c) => {
                            var sqlContext = new SqlContext("Goods", aliasName: "G")
                                .AddJoinTable(new JoinTable("Shop", "S", "on S.ID = G.FK_Shop_ID"))
                                .AddJoinTable(new JoinTable("Channel", "C", "on C.ID = S.FK_Channel_ID"))
                                .AddJoinTable(new JoinTable("ScheduleGoods", "SG", "on SG.FK_Goods_ID = G.ID"))
                                .AddJoinTable(new JoinTable("ScheduleConfig", "CON", "on con.ID = SG.FK_ScheduleConfig_ID"));
                            sqlContext.AddQueryField("ScheduleGoods", new string[] { "FK_ScheduleConfig_ID" })
                                .AddQueryField("ScheduleConfig", new string[] { "IsTakeScreenshot" })
                                .AddQueryField("Goods", new string[] { "ID", "SystemId", "Model", "SkuName", "Spec",
                                "GoodsUrl", "DataInterface", "UseInterface" })
                                .AddQueryField("Shop", new string[] { "ShopName" })
                                .AddQueryField("Channel", new string[] { "PriceXPath", "ScreenshotXPath", "ImageUrlXPath", "TaxFeeXPath", "PostageXPath",
                                "PreferentialXPath","CouponXPath","CloseXPath","ContentType","ChannelEnum","ChannelName" });
                            sqlContext.UpdateQueryFieldAliasName("ChannelEnum", "Channel");
                            sqlContext.WhereClause = "CON.IsDisabled = 0 and SG.IsDelete = 0 and G.IsDelete = 0 and G.IsOffSale = 0 and CON.ID = @FK_ScheduleConfig_ID";
                            sqlContext.AddProperty("@FK_ScheduleConfig_ID", c.ID);

                            var dataSource = _storage.Query<dynamic>(sqlContext);
                            if (dataSource != null && dataSource.Count() > 0)
                            {
                                _mq.PublishAsync(_options.TopicScheduleObserver, new Event
                                {
                                    Type = MessageType.GoodsSpider.ToString(),
                                    Data = JsonConvert.SerializeObject(dataSource)
                                });
                            }

                            //按门店配置
                            var shopSql = new SqlContext("Shop", aliasName: "S")
                            .AddJoinTable(new JoinTable("Channel", "C", "on C.ID = S.FK_Channel_ID"))
                            .AddJoinTable(new JoinTable("ScheduleShop", "SS", "on SS.FK_Shop_ID = S.ID"))
                            .AddJoinTable(new JoinTable("ScheduleConfig", "CON", "on con.ID = SS.FK_ScheduleConfig_ID"));
                            shopSql.AddQueryField("ScheduleShop", new string[] { "FK_ScheduleConfig_ID", "FK_Shop_ID" })
                            .AddQueryField("ScheduleConfig", new string[] { "IsTakeScreenshot" })
                            .AddQueryField("Shop", new string[] { "ShopName", "NavXPath", "ShopUrl" })
                            .AddQueryField("Channel", new string[] { "AllGoodsXPath", "PriceXPath", "ScreenshotXPath", "ImageUrlXPath", "TaxFeeXPath", "PostageXPath",
                                "PreferentialXPath","CouponXPath","CloseXPath","ContentType","ChannelEnum","ChannelName", "GoodsXPath", "PaginationXPath",
                                "ShopUrlRegex", "ShopUrlRegexReplace"});
                            shopSql.UpdateQueryFieldAliasName("ChannelEnum", "Channel");
                            shopSql.WhereClause = "CON.IsDisabled = 0 and SS.IsDelete = 0 and  CON.ID = @FK_ScheduleConfig_ID";
                            shopSql.AddProperty("@FK_ScheduleConfig_ID", c.ID);
                            var shops = _storage.Query<dynamic>(shopSql);
                            if (shops != null && shops.Count() > 0)
                            {
                                _mq.PublishAsync(_options.TopicScheduleObserver, new Event()
                                {
                                    Type = MessageType.ShopSpider.ToString(),
                                    Data = JsonConvert.SerializeObject(shops)
                                });
                            }
                        });

                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
           
            return Task.CompletedTask;
        }
    }
}
