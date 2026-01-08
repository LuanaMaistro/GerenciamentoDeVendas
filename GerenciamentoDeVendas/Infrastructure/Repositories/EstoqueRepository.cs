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
    public class EstoqueRepository : RepositoryBase<Estoque>, IEstoqueRepository
    {
        public EstoqueRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Estoque?> ObterPorProdutoIdAsync(Guid produtoId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.ProdutoId == produtoId);
        }

        public async Task<IEnumerable<Estoque>> ObterEstoqueBaixoAsync()
        {
            return await _dbSet
                .Where(e => e.Quantidade < e.QuantidadeMinima)
                .ToListAsync();
        }

        public async Task<IEnumerable<Estoque>> ObterPorLocalizacaoAsync(string localizacao)
        {
            return await _dbSet
                .Where(e => e.Localizacao == localizacao)
                .ToListAsync();
        }

        public async Task<bool> ProdutoTemEstoqueAsync(Guid produtoId)
        {
            return await _dbSet
                .AnyAsync(e => e.ProdutoId == produtoId);
        }
    }
}
