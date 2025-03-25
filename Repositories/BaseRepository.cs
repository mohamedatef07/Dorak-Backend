using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Table.ToListAsync();
        }

        //public async Task<T> GetById(int Id)
        //{
        //    return await Table.FindAsync(Id);
        //}

    }
}
