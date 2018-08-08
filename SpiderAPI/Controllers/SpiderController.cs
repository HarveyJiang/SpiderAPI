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
    [Route("api/spider")]
    public class SpiderController : BaseController<Spider>
    {
        public SpiderController(IConfiguration configuration, ILoggerFactory loggerFactory, IRepository<Spider> _repository) :
            base(configuration, loggerFactory, _repository)
        {
            
        }
    }
}
