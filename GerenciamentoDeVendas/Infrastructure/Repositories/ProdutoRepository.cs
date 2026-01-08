using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ProdutoRepository : RepositoryBase<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Produto?> ObterPorCodigoAsync(string codigo)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Codigo == codigo);
        }

        public async Task<IEnumerable<Produto>> ObterAtivosAsync()
        {
            return await _dbSet
                .Where(p => p.Ativo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Produto>> ObterPorCategoriaAsync(string categoria)
        {
            return await _dbSet
                .Where(p => p.Categoria == categoria)
                .ToListAsync();
        }

        public async Task<IEnumerable<Produto>> BuscarPorNomeAsync(string nome)
        {
            return await _dbSet
                .Where(p => p.Nome.Contains(nome))
                .ToListAsync();
        }

        public async Task<bool> CodigoJaCadastradoAsync(string codigo)
        {
            return await _dbSet
                .AnyAsync(p => p.Codigo == codigo);
        }
    }
}
