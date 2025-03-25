using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data;

namespace Repositories
{
    public class BaseRepository<T> where T : class
    {
        protected readonly DorakContext DbContext;
        protected DbSet<T> Table;

        public BaseRepository(DorakContext dbContext)
        {
            DbContext = dbContext;
            Table = DbContext.Set<T>();
        }

        // add 
        public async Task AddAsync(T entity)
        {
            await Table.AddAsync(entity);
            await DbContext.SaveChangesAsync();
        }

        // update
        public async Task EditAsync(T entity)
        {
            Table.Update(entity);
            await DbContext.SaveChangesAsync();
        }

        // delete
        public async Task DeleteAsync(T entity)
        {
            Table.Remove(entity);
            await DbContext.SaveChangesAsync();
        }

        // read
        public async Task<T> GetByIdAsync(Expression<Func<T, bool>> predicate)
        {
            return await Table.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await Table.ToListAsync();
        }

    }
}
