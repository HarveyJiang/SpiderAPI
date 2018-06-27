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
        public async Task<JsonResult> Get()
        {
            using (var con = new SqlConnection(connectionString))
            {
                Result result = new Result();
                con.Open();
                int t = await con.InsertAsync(new SpiderStartUrls()
                {
                    Params = "111",
                    Url = "g.cn"
                });
                result.Message = t.ToString();
                return Json(result);
            }



        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
