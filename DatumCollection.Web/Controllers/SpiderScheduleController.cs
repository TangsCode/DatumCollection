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
    public class SpiderScheduleController : ControllerBase
    {
        private IDataStorage _storage;
        public SpiderScheduleController(IDataStorage storage)
        {
            _storage = storage;
        }

        [HttpGet("[action]")]
        public IEnumerable<dynamic> Query()
        {
            var data = _storage.RecursiveQuery<SpiderScheduleSetting>().GetAwaiter().GetResult();
            //var data = _storage.RecursiveQuery(typeof(SpiderScheduleSetting), depth: 2).GetAwaiter().GetResult();
            return data;
        }

        [HttpGet("[action]/{id}")]
        public dynamic Get(string id)
        {
            var data = _storage.RecursiveQuery<SpiderScheduleSetting>(param: new { ID = id }, depth: 1).GetAwaiter().GetResult();
            var result = data.FirstOrDefault();
            return result;
        }

        [HttpPost("[action]")]
        public bool Add([FromBody]SpiderScheduleSetting item)
        {
            item.ID = Guid.NewGuid();
            item.CreateTime = DateTime.Now;
            item.ModifyTime = DateTime.Now;
            var result = _storage.Insert(new[] { item }).GetAwaiter().GetResult();
            return true;
        }

        [HttpPost("[action]")]
        public DbExecutionResult Edit([FromBody]ExpandoObject item)
        {
            //item.ModifyTime = DateTime.Now;
            var result = _storage.Update<SpiderScheduleSetting>(item).GetAwaiter().GetResult();
            return result;
        }

        [HttpPost("[action]")]
        public bool Delete([FromBody]SpiderScheduleSetting item)
        {
            var result = _storage.Delete(item).GetAwaiter().GetResult();
            return true;
        }

        [HttpGet("[action]/{id}")]
        public dynamic loadSpiderItems(string id)
        {
            var data = _storage.Query<SpiderScheduleItems>(i => i.FK_SpiderSchedule_ID == Guid.Parse(id)).GetAwaiter().GetResult();
            var ids = data.Select(d => d.FK_SpiderItem_ID).ToArray();
            return ids;
        }
    }
}