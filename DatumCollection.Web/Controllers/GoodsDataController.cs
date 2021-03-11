using System;
using System.Collections.Generic;
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
    public class GoodsDataController : ControllerBase
    {
        private IDataStorage _storage;

        public GoodsDataController(IDataStorage storage)
        {
            _storage = storage;
        }

        [HttpGet("[action]")]
        public IEnumerable<ElectronicCommerceWebsiteSpider> getGoodsData()
        {
            var datas = _storage.Query<ElectronicCommerceWebsiteSpider, SpiderSource, Channel>(
                (spider, item, channel) =>
                {
                    spider.SpiderSource = item;
                    item.Channel = channel;
                    return spider;
                }).GetAwaiter().GetResult();
            return datas.OrderByDescending(d => d.CreateTime);
        }
    }
}