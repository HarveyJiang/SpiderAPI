using System;
using System.Collections.Generic;
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
        public SpiderController(IConfiguration config) : base(config)
        {

        }

        // GET api/values
        [HttpGet]
        public JsonResult Get()
        {
            Result result = TryAction(() =>
             {
                 using (var con = this.GetConnection())
                 {
                     con.Open();
                     con.Insert(new SpiderStartUrls { Params = "124", Url = "baidul" });
                     //var query = "InSERT INTO crawler_setting.spider_start_urls(Params, Url) VALUES(@Params, @Url);";
                     //var count = con.Execute(query, new SpiderStartUrls() { Params = "124", Url = "baidul" });
                     Result r = new Result();
                     return r;
                 }
             });
            return Json(result);

        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public JsonResult Post([FromBody]string value)
        {
            return Json(base.connectionString);
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
