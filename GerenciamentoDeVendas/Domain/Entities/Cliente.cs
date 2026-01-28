using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Cliente
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public Documento Documento { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime DataCadastro { get; private set; }

        // Contato Principal (inline na tabela Clientes)
        public Contato? ContatoPrincipal { get; private set; }

        // Contatos Secundários (tabela separada)
        private readonly List<Contato> _contatosSecundarios = new();
        public IReadOnlyCollection<Contato> ContatosSecundarios => _contatosSecundarios.AsReadOnly();

        // Endereço Principal (inline na tabela Clientes)
        public Endereco? EnderecoPrincipal { get; private set; }

        // Endereços Secundários (tabela separada)
        private readonly List<Endereco> _enderecosSecundarios = new();
        public IReadOnlyCollection<Endereco> EnderecosSecundarios => _enderecosSecundarios.AsReadOnly();

        // Construtor para EF Core
        private Cliente()
        {
            Nome = string.Empty;
            Documento = null!;
        }

        public Cliente(string nome, Documento documento)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório", nameof(nome));

            if (documento is null)
                throw new ArgumentNullException(nameof(documento), "Documento é obrigatório");

            Id = Guid.NewGuid();
            Nome = nome.Trim();
            Documento = documento;
            Ativo = true;
            DataCadastro = DateTime.Now;
        }

        public void Ativar()
        {
            Ativo = true;
        }

        public void Inativar()
        {
            Ativo = false;
        }

        public void AtualizarNome(string novoNome)
        {
            if (string.IsNullOrWhiteSpace(novoNome))
                throw new ArgumentException("Nome é obrigatório", nameof(novoNome));

            Nome = novoNome.Trim();
        }

        // Contato Principal
        public void SetContatoPrincipal(Contato contato)
        {
            ContatoPrincipal = contato ?? throw new ArgumentNullException(nameof(contato));
        }

        // Contatos Secundários
        public void AdicionarContatoSecundario(Contato contato)
        {
            if (contato is null)
                throw new ArgumentNullException(nameof(contato));

            _contatosSecundarios.Add(contato);
        }

        public void RemoverContatoSecundario(Contato contato)
        {
            _contatosSecundarios.Remove(contato);
        }

        // Endereço Principal
        public void SetEnderecoPrincipal(Endereco endereco)
        {
            EnderecoPrincipal = endereco ?? throw new ArgumentNullException(nameof(endereco));
        }

        // Endereços Secundários
        public void AdicionarEnderecoSecundario(Endereco endereco)
        {
            if (endereco is null)
                throw new ArgumentNullException(nameof(endereco));

            _enderecosSecundarios.Add(endereco);
        }

        public void RemoverEnderecoSecundario(Endereco endereco)
        {
            _enderecosSecundarios.Remove(endereco);
        }
    }
}
