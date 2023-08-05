using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IRepository<T> where T : class
    {
        Task<T> Create(T entity);
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(long id);
        Task Delete(long id);
        Task Update(T entity);
    }
}
