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
        DateTime DataCadastro
    );

    public record ProdutoCreateDTO(
        string Codigo,
        string Nome,
        decimal PrecoUnitario,
        string? Descricao,
        string? Categoria
    );

    public record ProdutoUpdateDTO(
        string Nome,
        string? Descricao,
        decimal PrecoUnitario,
        string? Categoria
    );
}
