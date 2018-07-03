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
    [Route("api/[controller]/[action]")]
    public class SpiderStartUrlsController : BaseController<SpiderStartUrls>
    {
        public SpiderStartUrlsController(IConfiguration configuration, ILoggerFactory loggerFactory, IRepository<SpiderStartUrls> _repository) :
            base(configuration, loggerFactory, _repository)
        {

        }
    }
}
