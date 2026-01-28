using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public record ClienteDTO(
        Guid Id,
        string Nome,
        string Documento,
        string TipoDocumento,
        bool Ativo,
        DateTime DataCadastro,
        ContatoDTO? ContatoPrincipal,
        IEnumerable<ContatoDTO> ContatosSecundarios,
        EnderecoDTO? EnderecoPrincipal,
        IEnumerable<EnderecoDTO> EnderecosSecundarios
    );

    public record ClienteCreateDTO(
        string Nome,
        string Documento,
        ContatoDTO? ContatoPrincipal,
        IEnumerable<ContatoDTO>? ContatosSecundarios,
        EnderecoDTO? EnderecoPrincipal,
        IEnumerable<EnderecoDTO>? EnderecosSecundarios
    );

    public record ClienteUpdateDTO(
        string Nome,
        ContatoDTO? ContatoPrincipal,
        EnderecoDTO? EnderecoPrincipal
    );

    public record ContatoDTO(
        string? Telefone,
        string? Celular,
        string? Email
    );

    public record EnderecoDTO(
        string CEP,
        string Logradouro,
        string Numero,
        string? Complemento,
        string Bairro,
        string Cidade,
        string UF
    );
}
