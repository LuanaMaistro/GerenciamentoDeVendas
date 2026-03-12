using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public record RelatorioVendasResumoDTO(
        DateTime DataInicio,
        DateTime DataFim,
        int TotalPedidos,
        decimal ValorTotal,
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
        string? Localizacao,
        bool AbaixoDoMinimo
    );

    public record RelatorioEstoqueDTO(
        int TotalProdutos,
        int ProdutosAbaixoDoMinimo,
        IEnumerable<RelatorioEstoqueItemDTO> Itens
    );
}
