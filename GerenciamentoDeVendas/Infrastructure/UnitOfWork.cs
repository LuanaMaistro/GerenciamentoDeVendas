using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using System;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        private IClienteRepository? _clientes;
        private IProdutoRepository? _produtos;
        private IEstoqueRepository? _estoques;
        private IVendaRepository? _vendas;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IClienteRepository Clientes =>
            _clientes ??= new ClienteRepository(_context);

        public IProdutoRepository Produtos =>
            _produtos ??= new ProdutoRepository(_context);

        public IEstoqueRepository Estoques =>
            _estoques ??= new EstoqueRepository(_context);

        public IVendaRepository Vendas =>
            _vendas ??= new VendaRepository(_context);

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task RollbackAsync()
        {
            await _context.DisposeAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
