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
    public class VendaRepository : RepositoryBase<Venda>, IVendaRepository
    {
        public VendaRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<Venda?> ObterPorIdAsync(Guid id)
        {
            return await _dbSet
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public override async Task<IEnumerable<Venda>> ObterTodosAsync()
        {
            return await _dbSet
                .Include(v => v.Itens)
                .ToListAsync();
        }

        public async Task<IEnumerable<Venda>> ObterPorClienteIdAsync(Guid clienteId)
        {
            return await _dbSet
                .Include(v => v.Itens)
                .Where(v => v.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Venda>> ObterPorStatusAsync(StatusVenda status)
        {
            return await _dbSet
                .Include(v => v.Itens)
                .Where(v => v.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Venda>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _dbSet
                .Include(v => v.Itens)
                .Where(v => v.DataVenda >= dataInicio && v.DataVenda <= dataFim)
                .ToListAsync();
        }

        public async Task<IEnumerable<Venda>> ObterVendasComItensAsync(Guid vendaId)
        {
            return await _dbSet
                .Include(v => v.Itens)
                .Where(v => v.Id == vendaId)
                .ToListAsync();
        }

        public async Task<decimal> ObterTotalVendasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            var vendas = await _dbSet
                .Include(v => v.Itens)
                .Where(v => v.DataVenda >= dataInicio &&
                            v.DataVenda <= dataFim &&
                            v.Status == StatusVenda.Confirmada)
                .ToListAsync();

            return vendas.Sum(v => v.ValorTotal);
        }
    }
}
