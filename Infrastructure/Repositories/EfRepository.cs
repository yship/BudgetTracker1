using ApplicationCore.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EfRepository<T> : IAsyncRepository<T> where T : class
    {
        protected readonly BudgetTrackerDbContext _dbContext;

        public EfRepository(BudgetTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> ListAllWithIncludesAsync(Expression<Func<T, bool>> @where,
            params Expression<Func<T, object>>[] includes)
        {
            var query = _dbContext.Set<T>().AsQueryable();

            if (includes != null)
                foreach (Expression<Func<T, object>> navigationProperty in includes)
                    query = query.Include(navigationProperty);

            return await query.Where(@where).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbContext.Set<T>().Where(filter).ToListAsync();
        }

        public virtual async Task<int> GetCountAsync(Expression<Func<T, bool>> filter = null)
        {
            if (filter != null)
                return await _dbContext.Set<T>().Where(filter).CountAsync();
            return await _dbContext.Set<T>().CountAsync();
        }

        public virtual async Task<bool> GetExistsAsync(Expression<Func<T, bool>> filter = null)
        {
            if (filter == null) return false;
            return await _dbContext.Set<T>().Where(filter).AnyAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
        /*
        public virtual async Task<PaginatedList<T>> GetPagedData(int page = 1, int pageSize = 25,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderedQuery
                = null, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes)
        {
            var pagedList =
                await PaginatedList<T>.GetPaged(_dbContext.Set<T>(), page, pageSize, orderedQuery, filter, includes);
            return pagedList;
        }
        */
    }
}
