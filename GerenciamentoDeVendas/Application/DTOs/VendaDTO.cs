using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public record VendaDTO(
        Guid Id,
        Guid ClienteId,
        string? ClienteNome,
        DateTime DataVenda,
        decimal ValorTotal,
        string Status,
        string? FormaPagamento,
        string? Observacao,
        IEnumerable<ItemVendaDTO> Itens
    );

    public record VendaCreateDTO(
        Guid ClienteId,
        string? Observacao,
        IEnumerable<ItemVendaCreateDTO> Itens
    );

    public record ItemVendaDTO(
        Guid Id,
        Guid ProdutoId,
        string ProdutoNome,
        int Quantidade,
        decimal PrecoUnitario,
        decimal Subtotal
    );

    public record ItemVendaCreateDTO(
        Guid ProdutoId,
        int Quantidade
    );

    public record VendaConfirmarDTO(
        string FormaPagamento
    );
}
