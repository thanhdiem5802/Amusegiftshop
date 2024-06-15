using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DATA.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbCoffeeDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly ILogger<Repository<T>> _logger;
        public Repository(DbCoffeeDbContext context,  ILogger<Repository<T>> logger)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _logger = logger;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all records of type {EntityType}", typeof(T).Name);
                var result = await _dbSet.ToListAsync();
                _logger.LogInformation("Fetched {Count} records of type {EntityType}", result.Count, typeof(T).Name);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching records of type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> InsertAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
