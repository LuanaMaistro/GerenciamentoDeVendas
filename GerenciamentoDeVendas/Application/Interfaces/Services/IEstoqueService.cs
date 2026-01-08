using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IEstoqueService
    {
        Task<EstoqueDTO?> ObterPorIdAsync(Guid id);
        Task<EstoqueDTO?> ObterPorProdutoIdAsync(Guid produtoId);
        Task<IEnumerable<EstoqueDTO>> ObterTodosAsync();
        Task<IEnumerable<EstoqueDTO>> ObterEstoqueBaixoAsync();
        Task<IEnumerable<EstoqueDTO>> ObterPorLocalizacaoAsync(string localizacao);
        Task<EstoqueDTO> CriarAsync(EstoqueCreateDTO dto);
        Task<EstoqueDTO> AtualizarAsync(Guid id, EstoqueUpdateDTO dto);
        Task<EstoqueDTO> AdicionarQuantidadeAsync(EstoqueMovimentacaoDTO dto);
        Task<EstoqueDTO> RemoverQuantidadeAsync(EstoqueMovimentacaoDTO dto);
        Task<bool> TemEstoqueDisponivelAsync(Guid produtoId, int quantidade);
    }
}
