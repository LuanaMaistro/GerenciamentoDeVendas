using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IClienteService
    {
        Task<ClienteDTO?> ObterPorIdAsync(Guid id);
        Task<ClienteDTO?> ObterPorDocumentoAsync(string documento);
        Task<IEnumerable<ClienteDTO>> ObterTodosAsync();
        Task<IEnumerable<ClienteDTO>> ObterAtivosAsync();
        Task<IEnumerable<ClienteDTO>> BuscarPorNomeAsync(string nome);
        Task<ClienteDTO> CriarAsync(ClienteCreateDTO dto);
        Task<ClienteDTO> AtualizarAsync(Guid id, ClienteUpdateDTO dto);
        Task AtivarAsync(Guid id);
        Task InativarAsync(Guid id);
        Task AdicionarContatoAsync(Guid clienteId, ContatoDTO contato);
        Task AdicionarEnderecoSecundarioAsync(Guid clienteId, EnderecoDTO endereco);
        Task<bool> ExisteAsync(Guid id);
        Task<bool> DocumentoJaCadastradoAsync(string documento);
    }
}
