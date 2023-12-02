using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApp.Models;

namespace TestApp.Data.Repositories
{
    public abstract class BaseRepository<T>
        where T : BaseEntity
    {
        private readonly DbContext _context;

        public BaseRepository(DbContext context)
        {
            _context = context;
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await Table().ToListAsync();
        }

        public virtual async Task<T> GetAsync(long id)
        {
            return await Table().FirstOrDefaultAsync(x => x.Id == id);
        }

        protected virtual async Task<List<T>> GetByNames(IEnumerable<string> names, Func<T, string> propertyName)
        {
            return await Table().Where(x => names.Select(x => x.ToLower()).Contains(propertyName(x).ToLower())).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await Table().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await Table().AddRangeAsync(entities);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            Table().UpdateRange(entities);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        protected DbSet<T> Table()
        {
            return _context.Set<T>();
        }
    }
}
