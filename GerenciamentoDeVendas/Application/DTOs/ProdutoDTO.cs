using System;

namespace Application.DTOs
{
    public record ProdutoDTO(
        Guid Id,
        string Codigo,
        string Nome,
        string? Descricao,
        decimal PrecoUnitario,
        string? Categoria,
        bool Ativo,
        DateTime DataCadastro,
        int Quantidade,
        int QuantidadeMinima
    );

    public record ProdutoCreateDTO(
        string Codigo,
        string Nome,
        decimal PrecoUnitario,
        string? Descricao,
        string? Categoria,
        int Quantidade,
        int QuantidadeMinima
    );

    public record ProdutoUpdateDTO(
        string Nome,
        string? Descricao,
        decimal PrecoUnitario,
        string? Categoria,
        int QuantidadeMinima
    );

    public record ProdutoEstoqueMovimentacaoDTO(int Quantidade);
}
