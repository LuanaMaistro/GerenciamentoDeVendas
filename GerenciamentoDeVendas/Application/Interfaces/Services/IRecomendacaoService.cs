using Application.DTOs;
using System;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IRecomendacaoService
    {
        /// <summary>
        /// Sincroniza os dados do produto no Recombee (usado ao criar ou atualizar produto).
        /// </summary>
        Task SincronizarProdutoAsync(Guid produtoId, string nome, string? categoria, decimal preco);

        /// <summary>
        /// Registra que um cliente visualizou um produto.
        /// </summary>
        Task RegistrarVisualizacaoAsync(Guid clienteId, Guid produtoId);

        /// <summary>
        /// Registra a compra de um produto por um cliente (chamado ao confirmar venda).
        /// </summary>
        Task RegistrarCompraAsync(Guid clienteId, Guid produtoId, int quantidade);

        /// <summary>
        /// Retorna os N produtos recomendados para um cliente com base no histórico.
        /// </summary>
        Task<RecomendacaoResultadoDTO> ObterRecomendacoesAsync(Guid clienteId, int quantidade = 5);
    }
}
