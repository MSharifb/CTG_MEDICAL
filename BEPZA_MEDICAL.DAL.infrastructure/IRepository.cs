using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Linq.Expressions;

namespace BEPZA_MEDICAL.DAL.Infrastructure
{
    public class SortExpression<TEntity, TType>
    {
        Expression<Func<TEntity, TType>> SortProperty;
    }
    public interface IRepository<T> : IDisposable where T : class
    {
        IQueryable<T> Fetch();
        IEnumerable<T> GetAll();
        IEnumerable<T> Get(
            Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            string includeProperties);
        T GetByID(object id);
        T GetByID(object id, string KeyName);

        int GetCount(string filterExpression);
        IQueryable<T> GetPaged(string filterExpression, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount);

        IEnumerable<T> Find(Func<T, bool> predicate);
        T Single(Func<T, bool> predicate);
        T First(Func<T, bool> predicate);
        void Add(T entity);
        void Delete(T entity);
        void Attach(T entity);
        void Update(T entityToUpdate);
        void Update(T entityToUpdate, string KeyName);
        void SaveChanges();
        void SaveChanges(SaveOptions options);
    }
}
