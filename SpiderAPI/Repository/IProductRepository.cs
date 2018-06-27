
using System.Collections.Generic;

namespace RazorPagesExample.Repository
{
    public interface IRepository<T> where T : class, new()
    {
        int Add(T model);

        IEnumerator<T> GetList();

        T Get(int id);

        int Update(T product);

        int Delete(int id);

        
    }
}
