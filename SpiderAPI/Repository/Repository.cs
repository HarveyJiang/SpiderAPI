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
            logger = loggerFactory.CreateLogger(this.GetType().FullName);
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
                //_logger.Error(exception.StackTrace);
                return result;
            }
        }

        public async Task<Result> InsertAsync(T model)
        {
            Result result = new Result();
            using (var con = this.GetConnection())
            {
                con.Open();
                int t = await con.InsertAsync(model);
                result.Message = t.ToString();
                return result;
            }
        }

        public async Task<Result> DeleteAsync(T model)
        {
            Result result = new Result();
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
            Result result = new Result();
            using (var con = this.GetConnection())
            {
                con.Open();
                result.Succeed = await con.UpdateAsync(model);
                return result;
            }
        }

        public async Task<Result> GetListByPage(Condition condition)
        {

            List<T> products = new List<T>();
            Result result = new Result();

            int pageSize = condition.Pagination["PageSize"] ?? 10;
            pageSize = pageSize > 40 ? 10 : pageSize;
            int pageIndex = (condition.Pagination["PageIndex"] ?? 1) - 1;

            Func<T, bool> whereByFunc = null;

            foreach (var item in condition.FieldAndKeyWord)
            {
                var p = typeof(T).GetProperty(item.Key);
                whereByFunc = (t) => { return p.GetValue(t)?.ToString().Contains(item.Value) ?? false; };
                break;
            }

            Func<T, string> orderByFunc = null;
            var order = "asc";
            foreach (var item in condition?.Sort)
            {
                if (item.Value == "desc")
                {
                    order = "desc";
                }
                orderByFunc = (t) => { return typeof(T).GetProperty(item.Key)?.GetValue(t)?.ToString(); };
                break;
            }



            using (var con = this.GetConnection())
            {
                con.Open();
                var x = await con.GetAllAsync<T>();
                if (whereByFunc != null)
                {
                    x = x.Where(whereByFunc);
                }
                x = order == "asc" ? x.OrderBy(orderByFunc) : x.OrderByDescending(orderByFunc);
                result.Data = x;
                result.Count = x?.Count<T>();
                return result;
            }
        }

        public async Task<Result> GetAsync(T model)
        {
            Result result = new Result();
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

    }
}
