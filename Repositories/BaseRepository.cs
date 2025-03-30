using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Data;
using System.Reflection;

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
        public T GetById(Expression<Func<T, bool>> predicate)
        {
            return  Table.FirstOrDefault(predicate);
        }
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
    }
}
