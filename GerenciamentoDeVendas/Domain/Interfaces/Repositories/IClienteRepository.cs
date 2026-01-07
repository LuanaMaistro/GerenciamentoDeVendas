using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        Task<Cliente?> ObterPorDocumentoAsync(Documento documento);
        Task<IEnumerable<Cliente>> ObterAtivosAsync();
        Task<IEnumerable<Cliente>> ObterInativosAsync();
        Task<IEnumerable<Cliente>> BuscarPorNomeAsync(string nome);
        Task<bool> DocumentoJaCadastradoAsync(Documento documento);
    }
}
