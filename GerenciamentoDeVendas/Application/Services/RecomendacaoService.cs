using Application.DTOs;
using Application.Interfaces.Services;
using Domain.Interfaces;
using Recombee.ApiClient;
using Recombee.ApiClient.ApiRequests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RecomendacaoService : IRecomendacaoService
    {
        private readonly RecombeeClient _client;
        private readonly IUnitOfWork _unitOfWork;

        // Garante que as propriedades de item sejam criadas apenas uma vez por processo
        private static bool _schemaInicializado;
        private static readonly SemaphoreSlim _schemaLock = new(1, 1);

        public RecomendacaoService(RecombeeClient client, IUnitOfWork unitOfWork)
        {
            _client = client;
            _unitOfWork = unitOfWork;
        }

        private async Task GarantirSchemaAsync()
        {
            if (_schemaInicializado) return;

            await _schemaLock.WaitAsync();
            try
            {
                if (_schemaInicializado) return;

                var propriedades = new[] { ("nome", "string"), ("preco", "double"), ("categoria", "string") };
                foreach (var (nome, tipo) in propriedades)
                {
                    try   { await _client.SendAsync(new AddItemProperty(nome, tipo)); }
                    catch { /* propriedade já existe — ok */ }
                }

                _schemaInicializado = true;
            }
            finally
            {
                _schemaLock.Release();
            }
        }

        public async Task SincronizarProdutoAsync(Guid produtoId, string nome, string? categoria, decimal preco)
        {
            await GarantirSchemaAsync();

            var values = new Dictionary<string, object>
            {
                ["nome"] = nome,
                ["preco"] = (double)preco
            };

            if (categoria is not null)
                values["categoria"] = categoria;

            await _client.SendAsync(new SetItemValues(
                produtoId.ToString(),
                values,
                cascadeCreate: true
            ));
        }

        public async Task RegistrarVisualizacaoAsync(Guid clienteId, Guid produtoId)
        {
            await _client.SendAsync(new AddDetailView(
                clienteId.ToString(),
                produtoId.ToString(),
                cascadeCreate: true
            ));
        }

        public async Task RegistrarCompraAsync(Guid clienteId, Guid produtoId, int quantidade)
        {
            await _client.SendAsync(new AddPurchase(
                clienteId.ToString(),
                produtoId.ToString(),
                cascadeCreate: true,
                amount: quantidade
            ));
        }

        public async Task<RecomendacaoResultadoDTO> ObterRecomendacoesAsync(Guid clienteId, int quantidade = 5)
        {
            var response = await _client.SendAsync(
                new RecommendItemsToUser(clienteId.ToString(), quantidade, cascadeCreate: true)
            );

            var itens = new List<RecomendacaoItemDTO>();

            foreach (var rec in response.Recomms)
            {
                if (!Guid.TryParse(rec.Id, out var produtoId))
                    continue;

                var produto = await _unitOfWork.Produtos.ObterPorIdAsync(produtoId);
                if (produto is null) continue;

                itens.Add(new RecomendacaoItemDTO(
                    produto.Id,
                    produto.Nome,
                    produto.Categoria,
                    produto.PrecoUnitario
                ));
            }

            return new RecomendacaoResultadoDTO(clienteId, itens);
        }

    }
}
