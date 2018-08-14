
using SpiderAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpiderAPI
{
    public interface IRepository<T> where T : class, new()
    {
        Task<Result> InsertAsync(T model);
        Task<Result> DeleteAsync(T model);
        Task<Result> DeleteAllAsync(T model);
        Task<Result> UpdateAsync(T model);
        Task<Result> GetListByPage(Condition<T> condition, Func<T, bool> query=null);
        Task<Result> GetListByQuery(Condition<T> condition, Func<T, bool> query);
        Task<Result> GetAsync(T model);

        //IEnumerator<T> GetList(Condition condition);

        //T Get(int id);

        //int Update(T product);

        //int Delete(int id);

        //IEnumerator<T> Find(Condition condition);
    }
}
