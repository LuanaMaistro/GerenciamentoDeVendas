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
            // var doc = new Documento(documento);
            // var cliente = await _unitOfWork.Clientes.ObterPorDocumentoAsync(doc);
            // ------
            // eu, eu, digo, não usaria a variável "doc"
            // não, não é errado fazer isso, mas também não é um crime
            // se for manter a variável eu só não usaria o nome "doc", abreviações de uma forma geral devem ser evitadas
            // se um nome de variável tá reduntante, talvez a variável não precise mesmo existir
            var cliente = await _unitOfWork.Clientes.ObterPorDocumentoAsync(new Documento(documento));
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
            // aqui a gente já tinha conversado, mas eu vou fazer só para você ver o que eu tava pensando
            // as vezes a gente tem visões diferentes

            // vou remover essa linha
            // var documento = new Documento(dto.Documento);


            var cliente = new Cliente(dto.Nome, dto.Documento);

            // movi essas duas linhas para debaixo da criação do cliente
            // usando o documento que eu criei dentro do cliente
            if (await _unitOfWork.Clientes.DocumentoJaCadastradoAsync(cliente.Documento))
                throw new InvalidOperationException("Documento já cadastrado");

            if (dto.EnderecoPrincipal is not null)
                //mesma coisa que eu disse no ObterPorDocumentoAsync, eu removeria essa variável
                cliente.SetEnderecoPrincipal(MapToEndereco(dto.EnderecoPrincipal));


            if (dto.Contatos is not null)
            {
                // aqui não tem nada de ERRADO
                // mas nas linguagens de programação modernas a gente tem um recurso mais interessante para melhor clareza de laços de repetição: pipelines
                // pipelines são aplicaveis toda vez que você usar um laço para "transformar" uma lista de objetos para uma lista de outros objetos
                // esse é o caso aqui
                // foreach (var contatoDto in dto.Contatos)
                // {
                //     var contato = MapToContato(contatoDto); -> transformação de ContatoDTO para Contato
                //     cliente.AdicionarContato(contato); -> ação para cada item
                // }
                // ---
                // Nesse caso a gente tem duas opções, as duas funcionam igualmente, mas uma altera um pouco a interface de Cliente
                // e a outra não

                // SEM ALTERAR Cliente
                dto.Contatos.Select(MapToContato)
                    .ToList()
                    .ForEach(cliente.AdicionarContato);
                // teoricamente, fica mais simples de ler o que tá sendo feito
                // 1. transformo todos os dtos contatos em um Contato
                // 2. transformo em uma lista (esse passo é necessário porque o select transforma em IEnumerable)
                // 3. para cara item da lista, ou seja, para cada contato, o cliente adiciona o contato
                //
                // agora, se quiser, tem como reduzir isso tudo para uma única linha, alterando a interface de Cliente

                // ALTERANDO Cliente
                cliente.AdicionarContatos(dto.Contatos.Select(MapToContato));

                // aqui eu só adicionei um método para adicionar uma lista de contatos todo de uma vez
                // mas isso não é necessário, alterar a interface de uma Classe de Domínio nunca deve ser feito sem antes perguntar: "o domínio pede essa alteração?"
                // nesse caso: faz sentido eu querer adicionar varios contatos de uma vez em um cliente em mais lugares?
                // nunca se cria um método só para "ficar mais simples para quem chama".
                // mas como aqui você é o deus supremo do domínio, você pode decidir como prefere :)

                // Se você quiser também pode criar um método de extensão Map e simplificar mais ainda.
                // vou criar nesse arquivo mesmo só para você ver como fica
                // mas o ideal é criar em um arquivo separado
                // se quiser ver a implementação do Map, tá lá embaixo no arquivo

                // as mesmas duas opções, mas com método de extensão
                // SEM ALTERAR a interface de Cliente

                dto.Contatos.Map(ToContato)
                    .ForEach(cliente.AdicionarContato);

                // ALTERANDO a interface de Cliente
                cliente.AdicionarContatos(dto.Contatos.Map(ToContato));

                // quando a gente usa o nosso próprio método de extensão, conseguimos deixar o código mais sêmântico ainda
                // note que até conseguimos formar uma frase em português quase perfeita:
                // "os dtos de contatos do dto, mapear para contato, para cada um adicionar contato no cliente"

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

            if (dto.EnderecoPrincipal is not null)
            {
                var endereco = MapToEndereco(dto.EnderecoPrincipal);
                cliente.SetEnderecoPrincipal(endereco);
            }

            _unitOfWork.Clientes.Atualizar(cliente);

            // existe um problema classico com CommitAsync: e se o desenvolvedor esquecer de chamar?
            // "então é melhor eu chamar o CommitAsync/SaveChanges dentro do repository?"
            // É.... não. Além de não resolver o problema, você também perde a vantagem do UnitOfWork de agrupar várias operações em uma única transação
            // Para o caso desse projeto, eu recomendaria aplicar o padrão Proxy.
            // Porém isso é um pouco mais complexo para explicar escrevendo, se tiver interesse eu falo contigo via disc qualquer dia.
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

        public async Task AdicionarContatoAsync(Guid clienteId, ContatoDTO contatoDto)
        {
            var cliente = await _unitOfWork.Clientes.ObterPorIdAsync(clienteId)
                ?? throw new InvalidOperationException("Cliente não encontrado");

            var contato = MapToContato(contatoDto);
            cliente.AdicionarContato(contato);

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
                cliente.EnderecoPrincipal is null ? null : MapToEnderecoDTO(cliente.EnderecoPrincipal),
                cliente.EnderecosSecundarios.Select(MapToEnderecoDTO),
                cliente.Contatos.Select(MapToContatoDTO)
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

        private static ContatoDTO MapToContatoDTO(Contato contato)
        {
            return new ContatoDTO(
                contato.Tipo.ToString(),
                contato.GetFormatado(),
                contato.Principal
            );
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

        private static Contato ToContato(ContatoDTO dto)
        {
            return MapToContato(dto);
        }

        private static Contato MapToContato(ContatoDTO dto)
        {
            var tipo = Enum.Parse<TipoContato>(dto.Tipo, ignoreCase: true);
            return new Contato(tipo, dto.Valor, dto.Principal);
        }
    }

    public static class SistemasVendasExtension {
        public static List<T> Map<TSource, T>(this IEnumerable<TSource> source, Func<TSource, T> mapFunc)
        {
            return source.Select(mapFunc).ToList();
        }
    }
}
