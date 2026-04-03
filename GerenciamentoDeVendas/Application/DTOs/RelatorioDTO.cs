using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public record TotalPedidosDTO(
        DateTime DataInicio,
        DateTime DataFim,
        int TotalPedidos
    );

    public record ValorTotalVendasDTO(
        DateTime DataInicio,
        DateTime DataFim,
        decimal ValorTotal
    );

    public record TicketMedioDTO(
        DateTime DataInicio,
        DateTime DataFim,
        decimal TicketMedio
    );

    public record ProdutoMaisVendidoDTO(
        Guid ProdutoId,
        string ProdutoNome,
        int QuantidadeVendida,
        decimal ValorTotal
    );

    public record ClienteCompradorDTO(
        Guid ClienteId,
        string ClienteNome,
        int TotalPedidos,
        decimal ValorTotal
    );

    public record RelatorioEstoqueItemDTO(
        Guid ProdutoId,
        string ProdutoNome,
        int Quantidade,
        int QuantidadeMinima,
        bool AbaixoDoMinimo
    );

    public record RelatorioEstoqueDTO(
        int TotalProdutos,
        int ProdutosAbaixoDoMinimo,
        IEnumerable<RelatorioEstoqueItemDTO> Itens
    );

    public record CategoriaMaisVendidaDTO(
        string Categoria,
        int QuantidadeVendida,
        decimal ValorTotal
    );
}
