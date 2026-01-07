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

        public Endereco? EnderecoPrincipal { get; private set; }
        private readonly List<Endereco> _enderecosSecundarios = new();
        public IReadOnlyCollection<Endereco> EnderecosSecundarios => _enderecosSecundarios.AsReadOnly();

        private readonly List<Contato> _contatos = new();
        public IReadOnlyCollection<Contato> Contatos => _contatos.AsReadOnly();

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

        public void SetEnderecoPrincipal(Endereco endereco)
        {
            EnderecoPrincipal = endereco ?? throw new ArgumentNullException(nameof(endereco));
        }

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

        public void AdicionarContato(Contato contato)
        {
            if (contato is null)
                throw new ArgumentNullException(nameof(contato));

            if (contato.Principal)
            {
                foreach (var c in _contatos.Where(c => c.Tipo == contato.Tipo && c.Principal))
                {
                    _contatos.Remove(c);
                    _contatos.Add(new Contato(c.Tipo, c.Valor, false));
                }
            }

            _contatos.Add(contato);
        }

        public void RemoverContato(Contato contato)
        {
            _contatos.Remove(contato);
        }

        public Contato? ObterContatoPrincipal(TipoContato tipo)
        {
            return _contatos.FirstOrDefault(c => c.Tipo == tipo && c.Principal)
                ?? _contatos.FirstOrDefault(c => c.Tipo == tipo);
        }
    }
}
