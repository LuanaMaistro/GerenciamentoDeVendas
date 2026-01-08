using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IVendaService
    {
        Task<VendaDTO?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<VendaDTO>> ObterTodosAsync();
        Task<IEnumerable<VendaDTO>> ObterPorClienteIdAsync(Guid clienteId);
        Task<IEnumerable<VendaDTO>> ObterPorStatusAsync(string status);
        Task<IEnumerable<VendaDTO>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<VendaDTO> CriarAsync(VendaCreateDTO dto);
        Task<VendaDTO> AdicionarItemAsync(Guid vendaId, ItemVendaCreateDTO item);
        Task<VendaDTO> RemoverItemAsync(Guid vendaId, Guid itemId);
        Task<VendaDTO> AtualizarQuantidadeItemAsync(Guid vendaId, Guid itemId, int novaQuantidade);
        Task<VendaDTO> ConfirmarAsync(Guid vendaId, VendaConfirmarDTO dto);
        Task<VendaDTO> CancelarAsync(Guid vendaId);
        Task<decimal> ObterTotalVendasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    }
}
