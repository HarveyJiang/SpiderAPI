using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using SpiderAPI.Models;
using Dapper.Contrib.Extensions;
using System.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Extensions.Logging;

namespace SpiderAPI
{
    public class Repository<T> : IRepository<T> where T : BaseModel, new()
    {
        public string connectionString;
        protected readonly ILogger logger;
        public Repository(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<T>();
            connectionString = configuration.GetSection("ConnectionString").GetSection("MySQL").Value;

        }

        public IDbConnection GetConnection()
        {
            var conn = new MySqlConnection(connectionString);
            return conn;
        }


        public Result TryAction(Func<Result> pFunc)
        {
            try
            {
                return pFunc();
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
                logger.LogError(exception, "TryAction Error.");
                return result;
            }
        }

        public async Task<Result> InsertAsync(T model)
        {
            Result result = new Result() { Message = "添加成功。" };
            using (var con = this.GetConnection())
            {
                con.Open();
                int t = await con.InsertAsync(model);
                result.Data = t.ToString();
                return result;
            }
        }

        public async Task<Result> DeleteAsync(T model)
        {
            Result result = new Result() { Message = "删除成功。" };
            using (var con = this.GetConnection())
            {
                con.Open();
                result.Succeed = await con.DeleteAsync(model);
                return result;
            }
        }

        public async Task<Result> DeleteAllAsync(T model)
        {
            Result result = new Result();
            using (var con = this.GetConnection())
            {
                con.Open();
                result.Succeed = await con.DeleteAllAsync<T>();
                return result;
            }
        }

        public async Task<Result> UpdateAsync(T model)
        {
            Result result = new Result() { Message = "更新成功。" };
            using (var con = this.GetConnection())
            {
                con.Open();
                result.Succeed = await con.UpdateAsync(model);
                return result;
            }
        }

        public async Task<Result> GetListByPage(Condition<T> condition)
        {
            List<T> products = new List<T>();
            Result result = new Result() { Message = "" };
            int limit = condition.Limit > 40 ? 10 : condition.Limit;
            int offset = condition.Offset;
            List<Func<T, bool>> wheres = new List<Func<T, bool>>();

            if (condition.Query != null)
            {
                wheres.Add(condition.Query);
            }
            else if (!string.IsNullOrEmpty(condition.Key))
            {
                var fields = condition?.Fields?.Split(',') ?? new string[] { };
                foreach (var field in fields)
                {
                    var p = typeof(T).GetProperty(field);
                    if (p != null)
                    {
                        wheres.Add((t) =>
                        {
                            return p.GetValue(t)?.ToString()?.Contains(condition.Key) ?? false;
                        });
                    }
                    else
                    {
                        logger.LogWarning(string.Format("field '{0}' not found, search is invalid", field));
                    }
                }
            }
            List<Dictionary<string, Func<T, string>>> orders = new List<Dictionary<string, Func<T, string>>>();
            var sorts = condition?.Sorts?.Split(',') ?? new string[] { "Id" };
            foreach (var sort in sorts)
            {
                var d = new Dictionary<string, Func<T, string>>();
                var sf = sort.TrimStart('+', '-');
                d.Add(sort.StartsWith("+") ? "asc" : "desc",
                    (t) =>
                    {
                        var p = typeof(T).GetProperty(sf);
                        if (p == null)
                        {
                            logger.LogWarning(string.Format("field:'{0}' not found,sort is invalid.", sf));
                            return null;
                        }
                        return typeof(T).GetProperty(sf)?.GetValue(t)?.ToString();
                    });
                orders.Add(d);
            }



            using (var con = this.GetConnection())
            {
                con.Open();
                var x = await con.GetAllAsync<T>();
                foreach (var w in wheres)
                {
                    x = x.Where(w);
                }
                foreach (var o in orders)
                {
                    foreach (var item in o)
                    {
                        x = item.Key == "asc" ? x.OrderBy(item.Value) : x.OrderByDescending(item.Value);
                    }
                }
                //x = x.OrderByDescending(m => m.Id);
                result.Count = x.Count();
                x = x.Skip(offset).Take(limit);
                result.Data = x.ToList();
                return result;
            }
        }

        public async Task<Result> GetAsync(T model)
        {
            Result result = new Result() { Message = "" };
            using (var con = this.GetConnection())
            {
                con.Open();
                var x = await con.GetAsync<T>(model.Id);
                //if (x != null)
                //{
                //result.Data = new List<dynamic>() { x };
                //}
                result.Data = x;
                return result;
            }
        }

        public Task<Result> GetListByQuery(Condition<T> condition)
        {
            return GetListByPage(condition);
        }
    }


}
