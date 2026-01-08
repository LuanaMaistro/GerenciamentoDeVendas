using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Produto
    {
        // Construtor para EF Core
        private Produto()
        {
            Codigo = string.Empty;
            Nome = string.Empty;
        }

        public Guid Id { get; private set; }
        public string Codigo { get; private set; }
        public string Nome { get; private set; }
        public string? Descricao { get; private set; }
        public decimal PrecoUnitario { get; private set; }
        public string? Categoria { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime DataCadastro { get; private set; }

        public Produto(string codigo, string nome, decimal precoUnitario, string? descricao = null, string? categoria = null)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("Código é obrigatório", nameof(codigo));

            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório", nameof(nome));

            if (precoUnitario < 0)
                throw new ArgumentException("Preço unitário não pode ser negativo", nameof(precoUnitario));

            Id = Guid.NewGuid();
            Codigo = codigo.Trim();
            Nome = nome.Trim();
            Descricao = descricao?.Trim();
            PrecoUnitario = precoUnitario;
            Categoria = categoria?.Trim();
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

        public void AtualizarPreco(decimal novoPreco)
        {
            if (novoPreco < 0)
                throw new ArgumentException("Preço não pode ser negativo", nameof(novoPreco));

            PrecoUnitario = novoPreco;
        }

        public void AtualizarNome(string novoNome)
        {
            if (string.IsNullOrWhiteSpace(novoNome))
                throw new ArgumentException("Nome é obrigatório", nameof(novoNome));

            Nome = novoNome.Trim();
        }

        public void AtualizarDescricao(string? novaDescricao)
        {
            Descricao = novaDescricao?.Trim();
        }

        public void AtualizarCategoria(string? novaCategoria)
        {
            Categoria = novaCategoria?.Trim();
        }
    }
}
