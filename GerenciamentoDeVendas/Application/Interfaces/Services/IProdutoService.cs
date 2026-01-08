using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IProdutoService
    {
        Task<ProdutoDTO?> ObterPorIdAsync(Guid id);
        Task<ProdutoDTO?> ObterPorCodigoAsync(string codigo);
        Task<IEnumerable<ProdutoDTO>> ObterTodosAsync();
        Task<IEnumerable<ProdutoDTO>> ObterAtivosAsync();
        Task<IEnumerable<ProdutoDTO>> ObterPorCategoriaAsync(string categoria);
        Task<IEnumerable<ProdutoDTO>> BuscarPorNomeAsync(string nome);
        Task<ProdutoDTO> CriarAsync(ProdutoCreateDTO dto);
        Task<ProdutoDTO> AtualizarAsync(Guid id, ProdutoUpdateDTO dto);
        Task AtivarAsync(Guid id);
        Task InativarAsync(Guid id);
        Task<bool> ExisteAsync(Guid id);
        Task<bool> CodigoJaCadastradoAsync(string codigo);
    }
}
