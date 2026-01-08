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
    public class ProdutoService : IProdutoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProdutoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProdutoDTO?> ObterPorIdAsync(Guid id)
        {
            var produto = await _unitOfWork.Produtos.ObterPorIdAsync(id);
            return produto is null ? null : MapToDTO(produto);
        }

        public async Task<ProdutoDTO?> ObterPorCodigoAsync(string codigo)
        {
            var produto = await _unitOfWork.Produtos.ObterPorCodigoAsync(codigo);
            return produto is null ? null : MapToDTO(produto);
        }

        public async Task<IEnumerable<ProdutoDTO>> ObterTodosAsync()
        {
            var produtos = await _unitOfWork.Produtos.ObterTodosAsync();
            return produtos.Select(MapToDTO);
        }

        public async Task<IEnumerable<ProdutoDTO>> ObterAtivosAsync()
        {
            var produtos = await _unitOfWork.Produtos.ObterAtivosAsync();
            return produtos.Select(MapToDTO);
        }

        public async Task<IEnumerable<ProdutoDTO>> ObterPorCategoriaAsync(string categoria)
        {
            var produtos = await _unitOfWork.Produtos.ObterPorCategoriaAsync(categoria);
            return produtos.Select(MapToDTO);
        }

        public async Task<IEnumerable<ProdutoDTO>> BuscarPorNomeAsync(string nome)
        {
            var produtos = await _unitOfWork.Produtos.BuscarPorNomeAsync(nome);
            return produtos.Select(MapToDTO);
        }

        public async Task<ProdutoDTO> CriarAsync(ProdutoCreateDTO dto)
        {
            if (await _unitOfWork.Produtos.CodigoJaCadastradoAsync(dto.Codigo))
                throw new InvalidOperationException("Código já cadastrado");

            var produto = new Produto(
                dto.Codigo,
                dto.Nome,
                dto.PrecoUnitario,
                dto.Descricao,
                dto.Categoria
            );

            await _unitOfWork.Produtos.AdicionarAsync(produto);
            await _unitOfWork.CommitAsync();

            return MapToDTO(produto);
        }

        public async Task<ProdutoDTO> AtualizarAsync(Guid id, ProdutoUpdateDTO dto)
        {
            var produto = await _unitOfWork.Produtos.ObterPorIdAsync(id)
                ?? throw new InvalidOperationException("Produto não encontrado");

            produto.AtualizarNome(dto.Nome);
            produto.AtualizarDescricao(dto.Descricao);
            produto.AtualizarPreco(dto.PrecoUnitario);
            produto.AtualizarCategoria(dto.Categoria);

            _unitOfWork.Produtos.Atualizar(produto);
            await _unitOfWork.CommitAsync();

            return MapToDTO(produto);
        }

        public async Task AtivarAsync(Guid id)
        {
            var produto = await _unitOfWork.Produtos.ObterPorIdAsync(id)
                ?? throw new InvalidOperationException("Produto não encontrado");

            produto.Ativar();
            _unitOfWork.Produtos.Atualizar(produto);
            await _unitOfWork.CommitAsync();
        }

        public async Task InativarAsync(Guid id)
        {
            var produto = await _unitOfWork.Produtos.ObterPorIdAsync(id)
                ?? throw new InvalidOperationException("Produto não encontrado");

            produto.Inativar();
            _unitOfWork.Produtos.Atualizar(produto);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> ExisteAsync(Guid id)
        {
            return await _unitOfWork.Produtos.ExisteAsync(id);
        }

        public async Task<bool> CodigoJaCadastradoAsync(string codigo)
        {
            return await _unitOfWork.Produtos.CodigoJaCadastradoAsync(codigo);
        }

        private static ProdutoDTO MapToDTO(Produto produto)
        {
            return new ProdutoDTO(
                produto.Id,
                produto.Codigo,
                produto.Nome,
                produto.Descricao,
                produto.PrecoUnitario,
                produto.Categoria,
                produto.Ativo,
                produto.DataCadastro
            );
        }
    }
}
