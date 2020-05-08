using DatumCollection.Collectors;
using DatumCollection.Data;
using DatumCollection.WebDriver;
using DatumCollection.Utility.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatumCollection.SpiderScheduler
{
    /// <summary>
    /// 资源约束调度器
    /// </summary>
    public class ResourcesRestrictScheduler : ISpiderScheduler
    {
        internal readonly ConcurrentQueue<dynamic> RequestQueue;

        protected ConcurrentBag<WebDriverBase> drivers = new ConcurrentBag<WebDriverBase>();

        protected readonly SystemOptions _options;

        protected readonly ILogger<ResourcesRestrictScheduler> _logger;

        protected readonly IDataStorage _storage;

        protected readonly ICollector _collector;

        public ResourcesRestrictScheduler(
            SystemOptions options,
            ILogger<ResourcesRestrictScheduler> logger,
            IDataStorage storage,
            ICollector collector)
        {
            RequestQueue = new ConcurrentQueue<dynamic>();
            _options = options;
            _logger = logger;
            _storage = storage;
            _collector = collector;

            //Task.Factory.StartNew(
            //        () =>
            //        {
            //            while (drivers.Count < 10)
            //            {
            //                drivers.Add(new WebDriverBase(_options) { Logger = _logger });
            //            }
            //        });
        }

        public bool IsSatisfiedCondition =>
            FreeCpuPercent >= _options.FreeCpuLimitPercent
            && FreeMemoryPercent > _options.FreeMemoryLimitPercent;

        /// <summary>
        /// free cpu ratio
        /// </summary> 
        internal int FreeCpuPercent
        {
            get { return SpiderEnvironment.GetFreeCPU(); }
        }

        /// <summary>
        /// free memory ratio
        /// </summary>
        internal int FreeMemoryPercent
        {
            get { return (int)SpiderEnvironment.GetFreeMemory() * 100 / SpiderEnvironment.TotalMemory; }
        }

        public (bool, dynamic[]) Dequeue(int count = 1)
        {
            if (RequestQueue == null || RequestQueue.Count == 0)
            {
                return (false, null);
            }

            count = count > RequestQueue.Count ? RequestQueue.Count : count;
            dynamic[] result = new dynamic[count];
            for (int i = 0; i < count; i++)
            {
                RequestQueue.TryDequeue(out dynamic value);
                result[i] = value;
            }

            return (true, result);
        }

        public int Enqueue(IEnumerable<dynamic> sources)
        {
            int count = 0;
            foreach (var source in sources)
            {
                RequestQueue.Enqueue(source);
                count += 1;
            }

            return count;
        }

        public Task HandleScheduleTask(CollectContext context)
        {
            try
            {

                Parallel.ForEach(context.Sources,
                    (source) =>
                    {
                        while (true)
                        {
                            if (IsSatisfiedCondition)
                            {
                                var result = _collector.Collect(context);
                                //var driver = drivers.FirstOrDefault(d => !d.IsRunning);
                                //if (driver == null) { continue; }

                                //switch (context.CollectType)
                                //{
                                //    case CollectType.Shop:
                                //        {
                                //            IEnumerable<dynamic> goodsList = driver.CollectDataByShop(source);
                                //            Parallel.ForEach(goodsList,
                                //                (goods) =>
                                //                {
                                //                    Result result;
                                //                    lock (driver)
                                //                    {
                                //                        result = driver.ScrapeData(goods);
                                //                    }
                                //                    if (result.Success)
                                //                    {
                                //                        dynamic data = result.Data;
                                //                        data.ID = Guid.NewGuid().ToString();
                                //                        //处理下载图片
                                //                        IEnumerable<string> images = data.ImagePath;
                                //                        foreach (var image in images)
                                //                        {
                                //                            SqlContext insertGoodsImagesSql = new SqlContext("GoodsImages", operation: Operation.Insert);
                                //                            insertGoodsImagesSql.AddProperty("ID", Guid.NewGuid().ToString());
                                //                            insertGoodsImagesSql.AddProperty("FK_GoodsData_ID", data.ID);
                                //                            insertGoodsImagesSql.AddProperty("ImagePath", image);
                                //                            _storage.ExecuteAsync(insertGoodsImagesSql);
                                //                        }
                                //                        ((IDictionary<String, Object>)data).Remove("ImagePath");
                                //                        //门店抓取没有商品ID source.ID = null
                                //                        data.FK_Shop_ID = goods.FK_Shop_ID.Value;
                                //                        data.CollectType = CollectType.Shop;
                                //                        data.GoodsName = goods.SkuName.Value;
                                //                        data.FK_ScheduleRecord_ID = context.Schedule.ID;
                                //                        SqlContext insertSql = new SqlContext("GoodsData",
                                //                            data, operation: Operation.Insert);
                                //                        int count = _storage.Execute(insertSql);
                                //                        _logger.LogInformation($"{goods.SkuName.Value} 价格:{data.Price} 渠道:{goods.ChannelName.Value} 店铺:{goods.ShopName.Value}");
                                //                        _storage.ExecuteSql($"update ScheduleRecord set SuccessCount = SuccessCount + 1 where ID = '{context.Schedule.ID}'");
                                //                    }
                                //                    else
                                //                    {
                                //                        _logger.LogError($"scrape data failed.error:{result.ErrorMessage}");

                                //                        _storage.ExecuteSql($"update ScheduleRecord set FailedCount = FailedCount + 1 where ID = '{context.Schedule.ID}'");

                                //                        dynamic failData = new ExpandoObject();
                                //                        failData.FK_ScheduleRecord_ID = context.Schedule.ID;
                                //                        failData.CreateTime = DateTime.Now;
                                //                        failData.ID = Guid.NewGuid().ToString();
                                //                        failData.FK_Shop_ID = goods.FK_Shop_ID.Value;
                                //                        failData.GoodsName = goods.SkuName.Value;
                                //                        failData.FailureReason = result.ErrorMessage;

                                //                        SqlContext insertSql = new SqlContext("FailureRecord",
                                //                            failData, operation: Operation.Insert);
                                //                        int count = _storage.Execute(insertSql);
                                //                    }
                                //                });

                                //        }
                                //        break;
                                //    case CollectType.Goods:
                                //        {
                                //            Result result;
                                //            lock (driver)
                                //            {
                                //                result = driver.ScrapeData(source);
                                //            }
                                //            if (result.Success)
                                //            {
                                //                var data = result.Data;
                                //                data.ID = Guid.NewGuid().ToString();
                                //                //处理下载图片
                                //                IEnumerable<string> images = data.ImagePath;
                                //                foreach (var image in images)
                                //                {
                                //                    SqlContext insertGoodsImagesSql = new SqlContext("GoodsImages", operation: Operation.Insert);
                                //                    insertGoodsImagesSql.AddProperty("ID", Guid.NewGuid().ToString());
                                //                    insertGoodsImagesSql.AddProperty("FK_GoodsData_ID", data.ID);
                                //                    insertGoodsImagesSql.AddProperty("ImagePath", image);
                                //                    _storage.ExecuteAsync(insertGoodsImagesSql);
                                //                }
                                //                ((IDictionary<String, Object>)data).Remove("ImagePath");
                                //                data.FK_Goods_ID = source.ID.ToString();
                                //                data.CollectType = CollectType.Goods;
                                //                data.FK_ScheduleRecord_ID = context.Schedule.ID;
                                //                SqlContext insertSql = new SqlContext("GoodsData",
                                //                    data, operation: Operation.Insert);
                                //                int count = _storage.Execute(insertSql);
                                //                _logger.LogInformation($"{source.SkuName.Value} 价格:{data.Price} 渠道:{source.ChannelName.Value} 店铺:{source.ShopName.Value}");
                                //                _storage.ExecuteSql($"update ScheduleRecord set SuccessCount = SuccessCount + 1 where ID = '{context.Schedule.ID}'");
                                //            }
                                //            else
                                //            {
                                //                _logger.LogError($"scrape data failed.error:{result.ErrorMessage}");

                                //                _storage.ExecuteSql($"update ScheduleRecord set FailedCount = FailedCount + 1 where ID = '{context.Schedule.ID}'");

                                //                dynamic failData = new ExpandoObject();
                                //                failData.FK_ScheduleRecord_ID = context.Schedule.ID;
                                //                failData.CreateTime = DateTime.Now;
                                //                failData.ID = Guid.NewGuid().ToString();
                                //                failData.FK_Goods_ID = source.ID.ToString();
                                //                failData.FailureReason = result.ErrorMessage;

                                //                SqlContext insertSql = new SqlContext("FailureRecord",
                                //                    failData, operation: Operation.Insert);
                                //                _storage.Execute(insertSql);
                                //            }
                                //        }
                                //        break;
                                //    default:
                                //        break;
                                //}

                                break;
                            }
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                        }
                    });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            finally
            {
                dynamic updateSchedule = new ExpandoObject();
                updateSchedule.ID = context.Schedule.ID;
                updateSchedule.FinishTime = DateTime.Now;
                updateSchedule.ElapsedMinutes = (updateSchedule.FinishTime - context.Schedule.StartUpTime).TotalMinutes;
                SqlContext updateSql = new SqlContext("ScheduleRecord", updateSchedule, operation: Operation.Update);
                _storage.Execute(updateSql);
            }


            return Task.CompletedTask;
        }


    }
}
