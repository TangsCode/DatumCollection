using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using DatumCollection.Data;
using DatumCollection.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatumCollection.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpiderItemController : ControllerBase
    {
        private IDataStorage _storage;

        public SpiderItemController(IDataStorage storage)
        {
            _storage = storage;
        }

        [HttpGet("[action]")]
        public IEnumerable<SpiderSource> query()
        {
            var data = _storage.Query<SpiderSource, Channel>((source, channel) =>
            {
                source.Channel = channel;
                return source;
            }, source => !source.IsDelete).GetAwaiter().GetResult();
            return data;
        }

        [HttpGet("[action]")]
        public IEnumerable<object> getSelectOption()
        {
            //var sql = "select id,skuName from SpiderItem";
            //var context = new DataStorageContext() { SqlStatement = sql, Operation = Operation.Query,Parameters = new { IsDelete = 0} };

            var data = _storage.Query<SpiderSource>(source => !source.IsDelete).GetAwaiter().GetResult();
            var result = data.Select(s => {
                return new { key = s.ID, value = s.SkuName };
            });
            return result;
        }

        [HttpGet("[action]")]
        public IEnumerable<Channel> getChannels()
        {
            var data = _storage.Query<Channel>(c => !c.IsDelete).GetAwaiter().GetResult();
            return data;
        }

        [HttpPost("[action]")]
        public bool Add([FromBody]SpiderSource item)
        {
            item.ID = Guid.NewGuid();
            item.CreateTime = DateTime.Now;
            item.ModifyTime = DateTime.Now;
            var result = _storage.Insert(new[] { item }).GetAwaiter().GetResult();
            return true;
        }

        [HttpPost("[action]")]
        public bool Edit([FromBody]ExpandoObject item)
        {            
            //item.ModifyTime = DateTime.Now;
            var result = _storage.Update< SpiderSource>(item).GetAwaiter().GetResult();
            return true;
        }

        [HttpPost("[action]")]
        public bool Delete([FromBody]SpiderSource item)
        {
            var result = _storage.Delete(item).GetAwaiter().GetResult();
            return true;
        }

        [HttpGet("[action]")]
        public IEnumerable<dynamic> queryItemsPrice(string id)
        {
            var datas = _storage.Query<ElectronicCommerceWebsiteSpider, SpiderSource, Channel>(
                (spider, item, channel) =>
                {                    
                    spider.SpiderSource = item;
                    item.Channel = channel;
                    return spider;
                },s => s.FK_SpiderItem_ID == Guid.Parse(id) ).GetAwaiter().GetResult();
            return datas.OrderBy(s => s.CreateTime);
        }
    }
}