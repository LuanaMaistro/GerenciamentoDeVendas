using System;

namespace Application.DTOs
{
    public record EstoqueDTO(
        Guid Id,
        Guid ProdutoId,
        string? ProdutoNome,
        int Quantidade,
        int QuantidadeMinima,
        string? Localizacao,
        bool AbaixoDoMinimo,
        DateTime DataUltimaAtualizacao
    );

    public record EstoqueCreateDTO(
        Guid ProdutoId,
        int QuantidadeInicial,
        int QuantidadeMinima,
        string? Localizacao
    );

    public record EstoqueMovimentacaoDTO(
        Guid ProdutoId,
        int Quantidade,
        string? Observacao
    );

    public record EstoqueUpdateDTO(
        int QuantidadeMinima,
        string? Localizacao
    );
}
