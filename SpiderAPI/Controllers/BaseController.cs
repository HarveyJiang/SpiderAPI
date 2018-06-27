using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using NLog;
using SpiderAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SpiderAPI.Controllers
{

    public class BaseController : Controller
    {
        public string connectionString;
        private readonly ILogger _logger;
        public BaseController(IConfiguration config)
        {
            connectionString = config.GetSection("ConnectionString").GetSection("MySQL").Value;
        }
        [NonAction]
        public IDbConnection GetConnection()
        {
            var conn = new MySqlConnection(connectionString);
            return conn;
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
                //PostException(exception);

                Result result = new Result()
                {
                    Message = exception.Message,
                    Succeed = false,
                    Count = -1,
                    StackTrace = exception.StackTrace,
                };
                //_logger.Error(exception.StackTrace);
                return result;
            }
        }


    }
}
