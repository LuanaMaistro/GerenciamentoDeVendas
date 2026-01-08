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
    public class EstoqueService : IEstoqueService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EstoqueService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EstoqueDTO?> ObterPorIdAsync(Guid id)
        {
            var estoque = await _unitOfWork.Estoques.ObterPorIdAsync(id);
            if (estoque is null) return null;

            var produto = await _unitOfWork.Produtos.ObterPorIdAsync(estoque.ProdutoId);
            return MapToDTO(estoque, produto?.Nome);
        }

        public async Task<EstoqueDTO?> ObterPorProdutoIdAsync(Guid produtoId)
        {
            var estoque = await _unitOfWork.Estoques.ObterPorProdutoIdAsync(produtoId);
            if (estoque is null) return null;

            var produto = await _unitOfWork.Produtos.ObterPorIdAsync(produtoId);
            return MapToDTO(estoque, produto?.Nome);
        }

        public async Task<IEnumerable<EstoqueDTO>> ObterTodosAsync()
        {
            var estoques = await _unitOfWork.Estoques.ObterTodosAsync();
            var result = new List<EstoqueDTO>();

            foreach (var estoque in estoques)
            {
                var produto = await _unitOfWork.Produtos.ObterPorIdAsync(estoque.ProdutoId);
                result.Add(MapToDTO(estoque, produto?.Nome));
            }

            return result;
        }

        public async Task<IEnumerable<EstoqueDTO>> ObterEstoqueBaixoAsync()
        {
            var estoques = await _unitOfWork.Estoques.ObterEstoqueBaixoAsync();
            var result = new List<EstoqueDTO>();

            foreach (var estoque in estoques)
            {
                var produto = await _unitOfWork.Produtos.ObterPorIdAsync(estoque.ProdutoId);
                result.Add(MapToDTO(estoque, produto?.Nome));
            }

            return result;
        }

        public async Task<IEnumerable<EstoqueDTO>> ObterPorLocalizacaoAsync(string localizacao)
        {
            var estoques = await _unitOfWork.Estoques.ObterPorLocalizacaoAsync(localizacao);
            var result = new List<EstoqueDTO>();

            foreach (var estoque in estoques)
            {
                var produto = await _unitOfWork.Produtos.ObterPorIdAsync(estoque.ProdutoId);
                result.Add(MapToDTO(estoque, produto?.Nome));
            }

            return result;
        }

        public async Task<EstoqueDTO> CriarAsync(EstoqueCreateDTO dto)
        {
            var produto = await _unitOfWork.Produtos.ObterPorIdAsync(dto.ProdutoId)
                ?? throw new InvalidOperationException("Produto não encontrado");

            if (await _unitOfWork.Estoques.ProdutoTemEstoqueAsync(dto.ProdutoId))
                throw new InvalidOperationException("Produto já possui registro de estoque");

            var estoque = new Estoque(
                dto.ProdutoId,
                dto.QuantidadeInicial,
                dto.QuantidadeMinima,
                dto.Localizacao
            );

            await _unitOfWork.Estoques.AdicionarAsync(estoque);
            await _unitOfWork.CommitAsync();

            return MapToDTO(estoque, produto.Nome);
        }

        public async Task<EstoqueDTO> AtualizarAsync(Guid id, EstoqueUpdateDTO dto)
        {
            var estoque = await _unitOfWork.Estoques.ObterPorIdAsync(id)
                ?? throw new InvalidOperationException("Estoque não encontrado");

            estoque.AtualizarQuantidadeMinima(dto.QuantidadeMinima);
            estoque.AtualizarLocalizacao(dto.Localizacao);

            _unitOfWork.Estoques.Atualizar(estoque);
            await _unitOfWork.CommitAsync();

            var produto = await _unitOfWork.Produtos.ObterPorIdAsync(estoque.ProdutoId);
            return MapToDTO(estoque, produto?.Nome);
        }

        public async Task<EstoqueDTO> AdicionarQuantidadeAsync(EstoqueMovimentacaoDTO dto)
        {
            var estoque = await _unitOfWork.Estoques.ObterPorProdutoIdAsync(dto.ProdutoId)
                ?? throw new InvalidOperationException("Estoque não encontrado para este produto");

            estoque.AdicionarQuantidade(dto.Quantidade);

            _unitOfWork.Estoques.Atualizar(estoque);
            await _unitOfWork.CommitAsync();

            var produto = await _unitOfWork.Produtos.ObterPorIdAsync(dto.ProdutoId);
            return MapToDTO(estoque, produto?.Nome);
        }

        public async Task<EstoqueDTO> RemoverQuantidadeAsync(EstoqueMovimentacaoDTO dto)
        {
            var estoque = await _unitOfWork.Estoques.ObterPorProdutoIdAsync(dto.ProdutoId)
                ?? throw new InvalidOperationException("Estoque não encontrado para este produto");

            estoque.RemoverQuantidade(dto.Quantidade);

            _unitOfWork.Estoques.Atualizar(estoque);
            await _unitOfWork.CommitAsync();

            var produto = await _unitOfWork.Produtos.ObterPorIdAsync(dto.ProdutoId);
            return MapToDTO(estoque, produto?.Nome);
        }

        public async Task<bool> TemEstoqueDisponivelAsync(Guid produtoId, int quantidade)
        {
            var estoque = await _unitOfWork.Estoques.ObterPorProdutoIdAsync(produtoId);
            return estoque?.TemEstoqueDisponivel(quantidade) ?? false;
        }

        private static EstoqueDTO MapToDTO(Estoque estoque, string? produtoNome)
        {
            return new EstoqueDTO(
                estoque.Id,
                estoque.ProdutoId,
                produtoNome,
                estoque.Quantidade,
                estoque.QuantidadeMinima,
                estoque.Localizacao,
                estoque.EstaAbaixoDoMinimo(),
                estoque.DataUltimaAtualizacao
            );
        }
    }
}
