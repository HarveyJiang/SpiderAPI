using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using NLog;
using SpiderAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SpiderAPI.Controllers
{

    public class BaseController : Controller
    {
        public string connectionString;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        public BaseController(IConfiguration config)
        {
            connectionString = config.GetSection("ConnectionString").GetSection("MySQL").Value;
        }
        
        [NonAction]
        public async Task<Result> TryAction(Func<Task<Result>> pFunc)
        {
            try
            {
                return await pFunc();
            }
            catch (Exception exception)
            {
                Result result = new Result()
                {
                    Message = exception.Message,
                    Succeed = false,
                    Count = -1,
                    StackTrace = exception.StackTrace,
                };
                logger.Error($"Message:{exception.Message}\r\nStackTrace:{exception.StackTrace}");
                return result;
            }
        }


    }
}
