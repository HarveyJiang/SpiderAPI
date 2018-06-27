
using SpiderAPI.Models;
using System.Collections.Generic;

namespace SpiderAPI.Repository
{
    public interface IRepository<T> where T : class, new()
    {
        int Add(T model);

        IEnumerator<T> GetList(Condition condition);

        T Get(int id);

        int Update(T product);

        int Delete(int id);

        IEnumerator<T> Find(Condition condition);
    }
}
