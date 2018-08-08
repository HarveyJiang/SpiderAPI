using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
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
        protected IRepository<T> repository;
        protected readonly ILogger logger;
        protected Result result;
        public BaseController(IConfiguration configuration, ILoggerFactory loggerFactory, IRepository<T> _repository)
        {
            connectionString = configuration.GetSection("ConnectionString").GetSection("MySQL").Value;
            repository = _repository;
            logger = loggerFactory.CreateLogger<T>();
            result = new Result()
            {
                Succeed = false,
                MessageType = Result.MessageTypeEnum.error,
                Count = -1,
                Message = "fail",
            };
        }
        [NonAction]
        public override JsonResult Json(object data)
        {
            var jss = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateFormatString = "yyyy-MM-dd hh:mm:ss",
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            //jss.Converters.Add(new StringEnumConverter());
            return base.Json(data, jss);
        }

        //sort_by=desc(email) sort_by=desc(last_modified),asc(email)
        //X-Total-Count
        [HttpGet("search")]
        //[HttpGet("search?limit={number}&offset=20")]
        public async Task<JsonResult> GetListByPage([FromQuery] Condition<T> condition)
        {
            JsonResult r = await CommonAction(ActionType.GETLISTBYPAGE, condition);
            //dynamic d = r.Value;
            //HttpContext.Response.Headers.Add("X-Total-Count", d.Count);
            return r;
        }

        // GET api/values/5
        [HttpGet("{id:int}")]
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
        [HttpPut]
        public async Task<JsonResult> Put([FromBody]T model)
        {
            return await CommonAction(ActionType.UPDATE, model);
        }

        [HttpPatch("update")]
        public async Task<JsonResult> Patch(int id, [FromBody]JsonPatchDocument<T> modelPatch)
        {
            var r = await GetById(id);
            T instantce = r.Value as T;
            modelPatch.ApplyTo(instantce);
            return await CommonAction(ActionType.UPDATE, modelPatch);
        }


        // DELETE api/values/5
        [HttpDelete("{id:min(1)}")]
        public async Task<JsonResult> Delete(int id)
        {
            return await CommonAction(ActionType.DELETE, new T() { Id = id });
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
                    MessageType = Result.MessageTypeEnum.error,
                    Succeed = false,
                    Count = -1,
                    StackTrace = exception.StackTrace,
                };
                logger.LogError($"Message:{exception.Message}\r\nStackTrace:{exception.StackTrace}");
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
                        case ActionType.GETLISTBYPAGE:
                            //model is condition
                            return await repository.GetListByPage(model);
                        case ActionType.GETLISTBYQUERY:
                            //model is condition
                            return await repository.GetListByQuery(model);
                        default:
                            result.Message = $"暂不支持{actionType}。";
                            return result;
                    }
                });
            }
            else
            {
                result.Message = "参数错误。";
                logger.LogError($"error Eessage:{result.Message}");
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
        GETLISTBYPAGE = 5,
        GETLISTBYQUERY = 6,
    }
}


