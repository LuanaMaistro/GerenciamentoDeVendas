using Application.DTOs;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RelatorioService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TotalPedidosDTO> ObterTotalPedidosAsync(DateTime dataInicio, DateTime dataFim)
        {
            var vendas = await _unitOfWork.Vendas.ObterPorPeriodoAsync(dataInicio, dataFim);
            var qtd = vendas.Count(v => v.Status == StatusVenda.Confirmada);
            return new TotalPedidosDTO(dataInicio, dataFim, qtd);
        }

        public async Task<ValorTotalVendasDTO> ObterValorTotalAsync(DateTime dataInicio, DateTime dataFim)
        {
            var vendas = await _unitOfWork.Vendas.ObterPorPeriodoAsync(dataInicio, dataFim);
            var total = vendas.Where(v => v.Status == StatusVenda.Confirmada).Sum(v => v.ValorTotal);
            return new ValorTotalVendasDTO(dataInicio, dataFim, total);
        }

        public async Task<TicketMedioDTO> ObterTicketMedioAsync(DateTime dataInicio, DateTime dataFim)
        {
            var vendas = await _unitOfWork.Vendas.ObterPorPeriodoAsync(dataInicio, dataFim);
            var confirmadas = vendas.Where(v => v.Status == StatusVenda.Confirmada).ToList();
            var ticketMedio = confirmadas.Count > 0 ? Math.Round(confirmadas.Sum(v => v.ValorTotal) / confirmadas.Count, 2) : 0;
            return new TicketMedioDTO(dataInicio, dataFim, ticketMedio);
        }

        public async Task<IEnumerable<ProdutoMaisVendidoDTO>> ObterProdutosMaisVendidosAsync(DateTime dataInicio, DateTime dataFim, int top = 10)
        {
            var vendas = await _unitOfWork.Vendas.ObterPorPeriodoAsync(dataInicio, dataFim);
            var confirmadas = vendas.Where(v => v.Status == StatusVenda.Confirmada);

            var resultado = confirmadas
                .SelectMany(v => v.Itens)
                .GroupBy(i => new { i.ProdutoId, i.ProdutoNome })
                .Select(g => new ProdutoMaisVendidoDTO(
                    g.Key.ProdutoId,
                    g.Key.ProdutoNome,
                    g.Sum(i => i.Quantidade),
                    g.Sum(i => i.Subtotal)
                ))
                .OrderByDescending(p => p.QuantidadeVendida)
                .Take(top);

            return resultado;
        }

        public async Task<IEnumerable<ClienteCompradorDTO>> ObterClientesQueMoreCompraramAsync(DateTime dataInicio, DateTime dataFim, int top = 10)
        {
            var vendas = await _unitOfWork.Vendas.ObterPorPeriodoAsync(dataInicio, dataFim);
            var confirmadas = vendas.Where(v => v.Status == StatusVenda.Confirmada).ToList();

            var clienteIds = confirmadas.Select(v => v.ClienteId).Distinct();
            var clientes = new Dictionary<Guid, string?>();

            foreach (var id in clienteIds)
            {
                var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(id);
                clientes[id] = cliente?.Nome;
            }

            var resultado = confirmadas
                .GroupBy(v => v.ClienteId)
                .Select(g => new ClienteCompradorDTO(
                    g.Key,
                    clientes.GetValueOrDefault(g.Key) ?? "Cliente não encontrado",
                    g.Count(),
                    g.Sum(v => v.ValorTotal)
                ))
                .OrderByDescending(c => c.ValorTotal)
                .Take(top);

            return resultado;
        }

        public async Task<IEnumerable<CategoriaMaisVendidaDTO>> ObterCategoriasMaisVendidasAsync(DateTime dataInicio, DateTime dataFim, int top = 10)
        {
            var vendas = await _unitOfWork.Vendas.ObterPorPeriodoAsync(dataInicio, dataFim);
            var confirmadas = vendas.Where(v => v.Status == StatusVenda.Confirmada).ToList();

            var produtoIds = confirmadas
                .SelectMany(v => v.Itens)
                .Select(i => i.ProdutoId)
                .Distinct()
                .ToHashSet();

            var produtos = await _unitOfWork.Produtos.ObterTodosAsync();
            var categoriaPorProduto = produtos
                .Where(p => produtoIds.Contains(p.Id))
                .ToDictionary(p => p.Id, p => p.Categoria ?? "Sem categoria");

            var resultado = confirmadas
                .SelectMany(v => v.Itens)
                .GroupBy(i => categoriaPorProduto.GetValueOrDefault(i.ProdutoId, "Sem categoria"))
                .Select(g => new CategoriaMaisVendidaDTO(
                    g.Key,
                    g.Sum(i => i.Quantidade),
                    g.Sum(i => i.Subtotal)
                ))
                .OrderByDescending(c => c.QuantidadeVendida)
                .Take(top);

            return resultado;
        }

        public async Task<RelatorioEstoqueDTO> ObterRelatorioEstoqueAsync()
        {
            var estoques = await _unitOfWork.Estoques.ObterTodosAsync();
            var estoqueList = estoques.ToList();

            var itens = new List<RelatorioEstoqueItemDTO>();

            foreach (var estoque in estoqueList)
            {
                var produto = await _unitOfWork.Produtos.ObterPorIdAsync(estoque.ProdutoId);
                itens.Add(new RelatorioEstoqueItemDTO(
                    estoque.ProdutoId,
                    produto?.Nome ?? "Produto não encontrado",
                    estoque.Quantidade,
                    estoque.QuantidadeMinima,
                    estoque.EstaAbaixoDoMinimo()
                ));
            }

            itens = itens.OrderBy(i => i.ProdutoNome).ToList();

            return new RelatorioEstoqueDTO(
                itens.Count,
                itens.Count(i => i.AbaixoDoMinimo),
                itens
            );
        }
    }
}
