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
        EnderecoDTO? EnderecoPrincipal,
        IEnumerable<EnderecoDTO> EnderecosSecundarios,
        IEnumerable<ContatoDTO> Contatos
    );

    public record ClienteCreateDTO(
        string Nome,
        string Documento,
        EnderecoDTO? EnderecoPrincipal,
        IEnumerable<ContatoDTO>? Contatos
    );

    public record ClienteUpdateDTO(
        string Nome,
        EnderecoDTO? EnderecoPrincipal
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

    public record ContatoDTO(
        string Tipo,
        string Valor,
        bool Principal
    );
}
