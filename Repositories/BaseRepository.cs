using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using System.Reflection;
using Dorak.Models;

namespace Repositories
{
    public class BaseRepository<T> where T : class
    {
        protected readonly DorakContext dbContext;
        protected DbSet<T> Table;
        public BaseRepository(DorakContext _dbContext)
        {
            dbContext = _dbContext;
            Table = dbContext.Set<T>();
        }
      
        public void Add(T entity)
        {
            Table.Add(entity);
        }
        public void Edit(T entity)
        {
            Table.Update(entity);
        }
        public void Delete(T entity)
        {
            Table.Remove(entity);
        }
        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
        public async Task<T> GetByIdAsync(Expression<Func<T, bool>> predicate)
        {
            return await Table.FirstOrDefaultAsync(predicate);
        }
        //public async Task<IEnumerable<T>> GetAll()
        //{
        //    return await Table.ToListAsync();
        //}
        public IQueryable<T> GetAll()
        {
            return Table.AsQueryable();
        }
        public IQueryable<T> FilterBy(Expression<Func<T, bool>> filtereq, string Order_ColName, bool isAscending)
        {
            var Query = GetAll();
            if (filtereq != null)
            {
                Query = Query.Where(filtereq);
            }

            // order by
            var entityType = typeof(T);
            var property = entityType.GetProperty(Order_ColName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                throw new ArgumentException($"Property '{Order_ColName}' does not exist on type '{entityType.Name}'.");
            }

            var parameter = Expression.Parameter(entityType, "item");
            var propertyAccess = Expression.Property(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);


            string methodName = isAscending ? "OrderBy" : "OrderByDescending";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { entityType, property.PropertyType },
                Query.Expression,
                Expression.Quote(orderByExpression)
            );

            return Query.Provider.CreateQuery<T>(resultExpression);
        }
        //Pagination
        public IQueryable<T> GetList(Expression<Func<T, bool>> filter = null)
        {
            if (filter == null)
                return Table.AsQueryable();
            return Table.Where(filter);
        }
        public IQueryable<T> Get(Expression<Func<T, bool>> filter = null, int pageSize = 4, int pageNumber = 1)
        {
            IQueryable<T> query = Table.AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            if (pageSize < 0)
                pageSize = 4;

            if (pageNumber < 0)
                pageNumber = 1;


            int count = query.Count();

            if (count < pageSize)
            {
                pageSize = count;
                pageNumber = 1;
            }

            int ToSkip = (pageNumber - 1) * pageSize;

            query = query.Skip(ToSkip).Take(pageSize);

            return query;
        }
    }
}
