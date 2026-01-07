using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        Task<Produto?> ObterPorCodigoAsync(string codigo);
        Task<IEnumerable<Produto>> ObterAtivosAsync();
        Task<IEnumerable<Produto>> ObterPorCategoriaAsync(string categoria);
        Task<IEnumerable<Produto>> BuscarPorNomeAsync(string nome);
        Task<bool> CodigoJaCadastradoAsync(string codigo);
    }
}
