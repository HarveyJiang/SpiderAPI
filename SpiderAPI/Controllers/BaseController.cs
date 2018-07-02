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

    public class BaseController<T> : Controller where T : BaseModel, new()
    {
        protected string connectionString;
        protected Repository.Repository<T> repository;
        protected readonly ILogger logger = LogManager.GetCurrentClassLogger();
        protected Result result;
        public BaseController(IConfiguration configuration)
        {
            connectionString = configuration.GetSection("ConnectionString").GetSection("MySQL").Value;
            repository = new Repository.Repository<T>(configuration);
            result = new Result()
            {
                Succeed = false,
                Count = -1,
                Message = "fail",
            };
        }

        // GET api/values
        [HttpPost]
        public async Task<JsonResult> GetListByPage([FromBody] Condition condition)
        {

            return await CommonAction(ActionType.GETBYPAGE, condition);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<JsonResult> GetById(int id)
        {
            return await CommonAction(ActionType.GETBYID, new T() { Id = id });
        }

        // POST api/values
        [HttpPost]
        public async Task<JsonResult> Post([FromBody]T model)
        {
            return await CommonAction(ActionType.INSERT, model);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<JsonResult> Put([FromBody]T model)
        {
            return await CommonAction(ActionType.UPDATE, model);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<JsonResult> Delete(int id)
        {
            return await CommonAction(ActionType.GETBYID, new T() { Id = id });
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
        [NonAction]
        public async Task<JsonResult> CommonAction(ActionType actionType, dynamic model)
        {
            if (ModelState.IsValid)
            {
                result = await TryAction(async () =>
                {
                    switch (actionType)
                    {
                        case ActionType.INSERT:
                            return await repository.InsertAsync(model);
                        case ActionType.DELETE:
                            return await repository.DeleteAsync(model);
                        case ActionType.UPDATE:
                            return await repository.UpdateAsync(model);
                        case ActionType.GETBYID:
                            return await repository.GetAsync(model);
                        case ActionType.GETBYPAGE:
                            return await repository.GetListByPage(model);
                        default:
                            result.Message = $"暂不支持{actionType}。";
                            return result;
                    }
                });
            }
            else
            {
                result.Message = "参数错误。";
            }
            return Json(result);
        }
    }

    public enum ActionType
    {
        INSERT = 1,
        DELETE = 2,
        UPDATE = 3,
        GETBYID = 4,
        GETBYPAGE = 5,
    }
}


