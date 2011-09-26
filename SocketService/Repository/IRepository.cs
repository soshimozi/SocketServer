using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace SocketService.Repository
{
    public interface IRepository<T>
    {
        T Find(int id);
        T[] GetAll();
        IQueryable<T> Query(Expression<Func<T, bool>> filter);

        void Add(T value);
        void Delete(T value);
        void Update(T value);

    }
}
