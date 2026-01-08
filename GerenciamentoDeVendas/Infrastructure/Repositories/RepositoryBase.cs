using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public RepositoryBase(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> ObterPorIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> ObterTodosAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task AdicionarAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Atualizar(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Remover(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task<bool> ExisteAsync(Guid id)
        {
            return await _dbSet.FindAsync(id) is not null;
        }
    }
}
