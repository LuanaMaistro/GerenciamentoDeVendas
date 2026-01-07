using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IEstoqueRepository : IRepository<Estoque>
    {
        Task<Estoque?> ObterPorProdutoIdAsync(Guid produtoId);
        Task<IEnumerable<Estoque>> ObterEstoqueBaixoAsync();
        Task<IEnumerable<Estoque>> ObterPorLocalizacaoAsync(string localizacao);
        Task<bool> ProdutoTemEstoqueAsync(Guid produtoId);
    }
}
