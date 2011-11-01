using System;
using System.Linq;
using System.Linq.Expressions;

namespace SocketService.Repository
{
    public interface IRepository<T>
    {
        T Find(long id);
        T[] GetAll();
        IQueryable<T> Query(Expression<Func<T, bool>> filter);

        void Add(T value);
        void Delete(T value);
        void Update(T value);

    }
}
