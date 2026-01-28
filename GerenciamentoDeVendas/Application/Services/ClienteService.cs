using Application.DTOs;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClienteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ClienteDTO?> ObterPorIdAsync(Guid id)
        {
            var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(id);
            return cliente is null ? null : MapToDTO(cliente);
        }

        public async Task<ClienteDTO?> ObterPorDocumentoAsync(string documento)
        {
            var doc = new Documento(documento);
            var cliente = await _unitOfWork.Clientes.ObterPorDocumentoAsync(doc);
            return cliente is null ? null : MapToDTO(cliente);
        }

        public async Task<IEnumerable<ClienteDTO>> ObterTodosAsync()
        {
            var clientes = await _unitOfWork.Clientes.ObterTodosAsync();
            return clientes.Select(MapToDTO);
        }

        public async Task<IEnumerable<ClienteDTO>> ObterAtivosAsync()
        {
            var clientes = await _unitOfWork.Clientes.ObterAtivosAsync();
            return clientes.Select(MapToDTO);
        }

        public async Task<IEnumerable<ClienteDTO>> BuscarPorNomeAsync(string nome)
        {
            var clientes = await _unitOfWork.Clientes.BuscarPorNomeAsync(nome);
            return clientes.Select(MapToDTO);
        }

        public async Task<ClienteDTO> CriarAsync(ClienteCreateDTO dto)
        {
            var documento = new Documento(dto.Documento);

            if (await _unitOfWork.Clientes.DocumentoJaCadastradoAsync(documento))
                throw new InvalidOperationException("Documento já cadastrado");

            var cliente = new Cliente(dto.Nome, documento);

            if (dto.ContatoPrincipal is not null)
            {
                var contato = MapToContato(dto.ContatoPrincipal);
                cliente.SetContatoPrincipal(contato);
            }

            if (dto.ContatosSecundarios is not null)
            {
                foreach (var contatoDto in dto.ContatosSecundarios)
                {
                    var contato = MapToContato(contatoDto);
                    cliente.AdicionarContatoSecundario(contato);
                }
            }

            if (dto.EnderecoPrincipal is not null)
            {
                var endereco = MapToEndereco(dto.EnderecoPrincipal);
                cliente.SetEnderecoPrincipal(endereco);
            }

            if (dto.EnderecosSecundarios is not null)
            {
                foreach (var enderecoDto in dto.EnderecosSecundarios)
                {
                    var endereco = MapToEndereco(enderecoDto);
                    cliente.AdicionarEnderecoSecundario(endereco);
                }
            }

            await _unitOfWork.Clientes.AdicionarAsync(cliente);
            await _unitOfWork.CommitAsync();

            return MapToDTO(cliente);
        }

        public async Task<ClienteDTO> AtualizarAsync(Guid id, ClienteUpdateDTO dto)
        {
            var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(id)
                ?? throw new InvalidOperationException("Cliente não encontrado");

            cliente.AtualizarNome(dto.Nome);

            if (dto.ContatoPrincipal is not null)
            {
                var contato = MapToContato(dto.ContatoPrincipal);
                cliente.SetContatoPrincipal(contato);
            }

            if (dto.EnderecoPrincipal is not null)
            {
                var endereco = MapToEndereco(dto.EnderecoPrincipal);
                cliente.SetEnderecoPrincipal(endereco);
            }

            _unitOfWork.Clientes.Atualizar(cliente);
            await _unitOfWork.CommitAsync();

            return MapToDTO(cliente);
        }

        public async Task AtivarAsync(Guid id)
        {
            var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(id)
                ?? throw new InvalidOperationException("Cliente não encontrado");

            cliente.Ativar();
            _unitOfWork.Clientes.Atualizar(cliente);
            await _unitOfWork.CommitAsync();
        }

        public async Task InativarAsync(Guid id)
        {
            var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(id)
                ?? throw new InvalidOperationException("Cliente não encontrado");

            cliente.Inativar();
            _unitOfWork.Clientes.Atualizar(cliente);
            await _unitOfWork.CommitAsync();
        }

        public async Task AdicionarContatoSecundarioAsync(Guid clienteId, ContatoDTO contatoDto)
        {
            var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(clienteId)
                ?? throw new InvalidOperationException("Cliente não encontrado");

            var contato = MapToContato(contatoDto);
            cliente.AdicionarContatoSecundario(contato);

            _unitOfWork.Clientes.Atualizar(cliente);
            await _unitOfWork.CommitAsync();
        }

        public async Task AdicionarEnderecoSecundarioAsync(Guid clienteId, EnderecoDTO enderecoDto)
        {
            var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(clienteId)
                ?? throw new InvalidOperationException("Cliente não encontrado");

            var endereco = MapToEndereco(enderecoDto);
            cliente.AdicionarEnderecoSecundario(endereco);

            _unitOfWork.Clientes.Atualizar(cliente);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> ExisteAsync(Guid id)
        {
            return await _unitOfWork.Clientes.ExisteAsync(id);
        }

        public async Task<bool> DocumentoJaCadastradoAsync(string documento)
        {
            var doc = new Documento(documento);
            return await _unitOfWork.Clientes.DocumentoJaCadastradoAsync(doc);
        }

        private static ClienteDTO MapToDTO(Cliente cliente)
        {
            return new ClienteDTO(
                cliente.Id,
                cliente.Nome,
                cliente.Documento.GetFormatado(),
                cliente.Documento.Tipo.ToString(),
                cliente.Ativo,
                cliente.DataCadastro,
                cliente.ContatoPrincipal is null ? null : MapToContatoDTO(cliente.ContatoPrincipal),
                cliente.ContatosSecundarios.Select(MapToContatoDTO),
                cliente.EnderecoPrincipal is null ? null : MapToEnderecoDTO(cliente.EnderecoPrincipal),
                cliente.EnderecosSecundarios.Select(MapToEnderecoDTO)                                                                      
            );
        }

        private static ContatoDTO MapToContatoDTO(Contato contato)
        {
            return new ContatoDTO(
                contato.GetTelefoneFormatado(),
                contato.GetCelularFormatado(),
                contato.Email
            );
        }

        private static EnderecoDTO MapToEnderecoDTO(Endereco endereco)
        {
            return new EnderecoDTO(
                endereco.GetCEPFormatado(),
                endereco.Logradouro,
                endereco.Numero,
                endereco.Complemento,
                endereco.Bairro,
                endereco.Cidade,
                endereco.UF
            );
        }

        private static Contato MapToContato(ContatoDTO dto)
        {
            return new Contato(dto.Telefone, dto.Celular, dto.Email);
        }

        private static Endereco MapToEndereco(EnderecoDTO dto)
        {
            return new Endereco(
                dto.CEP,
                dto.Logradouro,
                dto.Numero,
                dto.Complemento,
                dto.Bairro,
                dto.Cidade,
                dto.UF
            );
        }
    }
}
