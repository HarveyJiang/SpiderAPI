using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SpiderAPI.Models;

namespace SpiderAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SpiderController : BaseController
    {

        Repository.Repository<SpiderBasic> repository;
        public SpiderController(IConfiguration configuration) : base(configuration)
        {
            repository = new Repository.Repository<SpiderBasic>(configuration);
        }

        // GET api/values
        [HttpGet]
        public async Task<JsonResult> Get([FromBody] Condition condition)
        {
            var r = await TryAction(async () =>
            {
                return await repository.GetList(condition);
            });

            return Json(r);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<JsonResult> Get(int id)
        {
            var r = await TryAction(async () =>
            {
                return await repository.Get(new SpiderBasic() { Id = id });
            });

            return Json(r);
        }

        // POST api/values
        [HttpPost]
        public async Task<JsonResult> Post([FromBody]SpiderBasic model)
        {
            var r = await TryAction(async () =>
            {
                return await repository.InsertAsync(model);
            });

            return Json(r);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<JsonResult> Put([FromBody]SpiderBasic model)
        {
            var r = await TryAction(async () =>
            {
                return await repository.UpdateAsync(model);
            });

            return Json(r);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<JsonResult> Delete(int id)
        {
            var r = await TryAction(async () =>
            {
                return await repository.DeleteAsync(new SpiderBasic() { Id = id });
            });

            return Json(r);
        }
    }
}
