using Domain.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IClienteRepository Clientes { get; }
        IProdutoRepository Produtos { get; }
        IEstoqueRepository Estoques { get; }
        IVendaRepository Vendas { get; }

        Task<int> CommitAsync();
        Task RollbackAsync();
    }
}
