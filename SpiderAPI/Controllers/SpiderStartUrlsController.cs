using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpiderAPI.Models;


namespace SpiderAPI.Controllers
{

    [Route("api/spider_starturl")]
    public class SpiderStartUrlsController : BaseController<SpiderStartUrls>
    {

        public SpiderStartUrlsController(IConfiguration configuration, ILoggerFactory loggerFactory, IRepository<SpiderStartUrls> _repository) :
            base(configuration, loggerFactory, _repository)
        {


        }

        [HttpGet("spider/{spiderId:int}/starturls")]
        public async Task<JsonResult> GetUrlsBySpiderId(int spiderId, [FromQuery] Condition<SpiderStartUrls> condition)
        {
            condition.Query = m => m.SpiderId == spiderId; 
            return await CommonAction(ActionType.GETLISTBYQUERY, condition);
        }
    }
}
