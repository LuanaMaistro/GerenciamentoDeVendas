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
    public class VendaService : IVendaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VendaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<VendaDTO?> ObterPorIdAsync(Guid id)
        {
            var venda = await _unitOfWork.Vendas.ObterPorIdAsync(id);
            if (venda is null) return null;

            var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(venda.ClienteId);
            return MapToDTO(venda, cliente?.Nome);
        }

        public async Task<IEnumerable<VendaDTO>> ObterTodosAsync()
        {
            var vendas = await _unitOfWork.Vendas.ObterTodosAsync();
            return await MapVendasToDTOsAsync(vendas);
        }

        public async Task<IEnumerable<VendaDTO>> ObterPorClienteIdAsync(Guid clienteId)
        {
            var vendas = await _unitOfWork.Vendas.ObterPorClienteIdAsync(clienteId);
            return await MapVendasToDTOsAsync(vendas);
        }

        public async Task<IEnumerable<VendaDTO>> ObterPorStatusAsync(string status)
        {
            var statusEnum = Enum.Parse<StatusVenda>(status, ignoreCase: true);
            var vendas = await _unitOfWork.Vendas.ObterPorStatusAsync(statusEnum);
            return await MapVendasToDTOsAsync(vendas);
        }

        public async Task<IEnumerable<VendaDTO>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            var vendas = await _unitOfWork.Vendas.ObterPorPeriodoAsync(dataInicio, dataFim);
            return await MapVendasToDTOsAsync(vendas);
        }

        public async Task<VendaDTO> CriarAsync(VendaCreateDTO dto)
        {
            var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(dto.ClienteId)
                ?? throw new InvalidOperationException("Cliente não encontrado");

            var venda = new Venda(dto.ClienteId, dto.Observacao);

            foreach (var itemDto in dto.Itens)
            {
                var produto = await _unitOfWork.Produtos.ObterPorIdAsync(itemDto.ProdutoId)
                    ?? throw new InvalidOperationException($"Produto {itemDto.ProdutoId} não encontrado");

                if (!produto.Ativo)
                    throw new InvalidOperationException($"Produto {produto.Nome} está inativo");

                venda.AdicionarItem(produto.Id, produto.Nome, itemDto.Quantidade, produto.PrecoUnitario);
            }

            await _unitOfWork.Vendas.AdicionarAsync(venda);
            await _unitOfWork.CommitAsync();

            return MapToDTO(venda, cliente.Nome);
        }

        public async Task<VendaDTO> AdicionarItemAsync(Guid vendaId, ItemVendaCreateDTO itemDto)
        {
            var venda = await _unitOfWork.Vendas.ObterPorIdAsync(vendaId)
                ?? throw new InvalidOperationException("Venda não encontrada");

            var produto = await _unitOfWork.Produtos.ObterPorIdAsync(itemDto.ProdutoId)
                ?? throw new InvalidOperationException("Produto não encontrado");

            if (!produto.Ativo)
                throw new InvalidOperationException($"Produto {produto.Nome} está inativo");

            venda.AdicionarItem(produto.Id, produto.Nome, itemDto.Quantidade, produto.PrecoUnitario);

            _unitOfWork.Vendas.Atualizar(venda);
            await _unitOfWork.CommitAsync();

            var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(venda.ClienteId);
            return MapToDTO(venda, cliente?.Nome);
        }

        public async Task<VendaDTO> RemoverItemAsync(Guid vendaId, Guid itemId)
        {
            var venda = await _unitOfWork.Vendas.ObterPorIdAsync(vendaId)
                ?? throw new InvalidOperationException("Venda não encontrada");

            venda.RemoverItem(itemId);

            _unitOfWork.Vendas.Atualizar(venda);
            await _unitOfWork.CommitAsync();

            var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(venda.ClienteId);
            return MapToDTO(venda, cliente?.Nome);
        }

        public async Task<VendaDTO> AtualizarQuantidadeItemAsync(Guid vendaId, Guid itemId, int novaQuantidade)
        {
            var venda = await _unitOfWork.Vendas.ObterPorIdAsync(vendaId)
                ?? throw new InvalidOperationException("Venda não encontrada");

            venda.AtualizarQuantidadeItem(itemId, novaQuantidade);

            _unitOfWork.Vendas.Atualizar(venda);
            await _unitOfWork.CommitAsync();

            var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(venda.ClienteId);
            return MapToDTO(venda, cliente?.Nome);
        }

        public async Task<VendaDTO> ConfirmarAsync(Guid vendaId, VendaConfirmarDTO dto)
        {
            var venda = await _unitOfWork.Vendas.ObterPorIdAsync(vendaId)
                ?? throw new InvalidOperationException("Venda não encontrada");

            var formaPagamento = Enum.Parse<FormaPagamento>(dto.FormaPagamento, ignoreCase: true);

            // Validar e baixar estoque
            foreach (var item in venda.Itens)
            {
                var estoque = await _unitOfWork.Estoques.ObterPorProdutoIdAsync(item.ProdutoId);

                if (estoque is null)
                    throw new InvalidOperationException($"Estoque não encontrado para o produto {item.ProdutoNome}");

                if (!estoque.TemEstoqueDisponivel(item.Quantidade))
                    throw new InvalidOperationException($"Estoque insuficiente para o produto {item.ProdutoNome}");

                estoque.RemoverQuantidade(item.Quantidade);
                _unitOfWork.Estoques.Atualizar(estoque);
            }

            venda.Confirmar(formaPagamento);

            _unitOfWork.Vendas.Atualizar(venda);
            await _unitOfWork.CommitAsync();

            var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(venda.ClienteId);
            return MapToDTO(venda, cliente?.Nome);
        }

        public async Task<VendaDTO> CancelarAsync(Guid vendaId)
        {
            var venda = await _unitOfWork.Vendas.ObterPorIdAsync(vendaId)
                ?? throw new InvalidOperationException("Venda não encontrada");

            // Se a venda estava confirmada, devolver ao estoque
            if (venda.Status == StatusVenda.Confirmada)
            {
                foreach (var item in venda.Itens)
                {
                    var estoque = await _unitOfWork.Estoques.ObterPorProdutoIdAsync(item.ProdutoId);
                    if (estoque is not null)
                    {
                        estoque.AdicionarQuantidade(item.Quantidade);
                        _unitOfWork.Estoques.Atualizar(estoque);
                    }
                }
            }

            venda.Cancelar();

            _unitOfWork.Vendas.Atualizar(venda);
            await _unitOfWork.CommitAsync();

            var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(venda.ClienteId);
            return MapToDTO(venda, cliente?.Nome);
        }

        public async Task<decimal> ObterTotalVendasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _unitOfWork.Vendas.ObterTotalVendasPorPeriodoAsync(dataInicio, dataFim);
        }

        private async Task<IEnumerable<VendaDTO>> MapVendasToDTOsAsync(IEnumerable<Venda> vendas)
        {
            var result = new List<VendaDTO>();

            foreach (var venda in vendas)
            {
                var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(venda.ClienteId);
                result.Add(MapToDTO(venda, cliente?.Nome));
            }

            return result;
        }

        private static VendaDTO MapToDTO(Venda venda, string? clienteNome)
        {
            return new VendaDTO(
                venda.Id,
                venda.ClienteId,
                clienteNome,
                venda.DataVenda,
                venda.ValorTotal,
                venda.Status.ToString(),
                venda.FormaPagamento?.ToString(),
                venda.Observacao,
                venda.Itens.Select(MapToItemDTO)
            );
        }

        private static ItemVendaDTO MapToItemDTO(ItemVenda item)
        {
            return new ItemVendaDTO(
                item.Id,
                item.ProdutoId,
                item.ProdutoNome,
                item.Quantidade,
                item.PrecoUnitario,
                item.Subtotal
            );
        }
    }
}
