using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using SpiderAPI.Models;
using Dapper.Contrib.Extensions;

namespace SpiderAPI.Repository
{
    public class Repository<T> where T : BaseModel
    {
        public string connectionString;
        public Repository(IConfiguration configuration)
        {
            connectionString = configuration.GetSection("ConnectionString").GetSection("MySQL").Value;
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
            using (var con = new SqlConnection(connectionString))
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
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                result.Succeed = await con.DeleteAsync(model);
                return result;
            }
        }

        public async Task<Result> DeleteAllAsync(T model)
        {
            Result result = new Result();
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                result.Succeed = await con.DeleteAllAsync<T>();
                return result;
            }
        }

        public async Task<Result> UpdateAsync(T model)
        {
            Result result = new Result();
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                result.Succeed = await con.UpdateAsync(model);
                return result;
            }
        }

        public async Task<Result> GetList(Condition condition)
        {

            List<T> products = new List<T>();
            Result result = new Result();
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                var x = await con.GetAllAsync<T>();
                result.Data = x.ToList();
                return result;
            }
        }

        public async Task<Result> Get(T model)
        {
            Result result = new Result();
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                //await con.Get(model);
                return result;
            }
        }


    }
}
