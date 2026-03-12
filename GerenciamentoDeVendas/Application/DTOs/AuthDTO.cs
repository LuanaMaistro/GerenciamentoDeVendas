using System;

namespace Application.DTOs
{
    public record LoginDTO(
        string Email,
        string Senha
    );

    public record TokenDTO(
        string Token,
        DateTime Expiracao,
        string NomeUsuario,
        string Email,
        string Role
    );

    public record UsuarioCreateDTO(
        string Nome,
        string Email,
        string Senha,
        string Role
    );

    public record UsuarioDTO(
        Guid Id,
        string Nome,
        string Email,
        string Role,
        DateTime DataCadastro
    );
}
