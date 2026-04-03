using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IRelatorioService
    {
        Task<TotalPedidosDTO> ObterTotalPedidosAsync(DateTime dataInicio, DateTime dataFim);
        Task<ValorTotalVendasDTO> ObterValorTotalAsync(DateTime dataInicio, DateTime dataFim);
        Task<TicketMedioDTO> ObterTicketMedioAsync(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<ProdutoMaisVendidoDTO>> ObterProdutosMaisVendidosAsync(DateTime dataInicio, DateTime dataFim, int top = 10);
        Task<IEnumerable<ClienteCompradorDTO>> ObterClientesQueMoreCompraramAsync(DateTime dataInicio, DateTime dataFim, int top = 10);
        Task<RelatorioEstoqueDTO> ObterRelatorioEstoqueAsync();
        Task<IEnumerable<CategoriaMaisVendidaDTO>> ObterCategoriasMaisVendidasAsync(DateTime dataInicio, DateTime dataFim, int top = 10);
    }
}
