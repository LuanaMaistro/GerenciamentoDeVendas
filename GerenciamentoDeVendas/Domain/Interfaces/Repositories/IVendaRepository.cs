using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IVendaRepository : IRepository<Venda>
    {
        Task<IEnumerable<Venda>> ObterPorClienteIdAsync(Guid clienteId);
        Task<IEnumerable<Venda>> ObterPorStatusAsync(StatusVenda status);
        Task<IEnumerable<Venda>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<Venda>> ObterVendasComItensAsync(Guid vendaId);
        Task<decimal> ObterTotalVendasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    }
}
