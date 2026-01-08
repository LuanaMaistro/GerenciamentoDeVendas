using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Estoque
    {
        // Construtor para EF Core
        private Estoque() { }

        public Guid Id { get; private set; }
        public Guid ProdutoId { get; private set; }
        public int Quantidade { get; private set; }
        public int QuantidadeMinima { get; private set; }
        public string? Localizacao { get; private set; }
        public DateTime DataUltimaAtualizacao { get; private set; }

        public Estoque(Guid produtoId, int quantidadeInicial = 0, int quantidadeMinima = 0, string? localizacao = null)
        {
            if (produtoId == Guid.Empty)
                throw new ArgumentException("ProdutoId é obrigatório", nameof(produtoId));

            if (quantidadeInicial < 0)
                throw new ArgumentException("Quantidade inicial não pode ser negativa", nameof(quantidadeInicial));

            if (quantidadeMinima < 0)
                throw new ArgumentException("Quantidade mínima não pode ser negativa", nameof(quantidadeMinima));

            Id = Guid.NewGuid();
            ProdutoId = produtoId;
            Quantidade = quantidadeInicial;
            QuantidadeMinima = quantidadeMinima;
            Localizacao = localizacao?.Trim();
            DataUltimaAtualizacao = DateTime.Now;
        }

        public void AdicionarQuantidade(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

            Quantidade += quantidade;
            DataUltimaAtualizacao = DateTime.Now;
        }

        public void RemoverQuantidade(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

            if (quantidade > Quantidade)
                throw new InvalidOperationException("Quantidade insuficiente em estoque");

            Quantidade -= quantidade;
            DataUltimaAtualizacao = DateTime.Now;
        }

        public bool EstaAbaixoDoMinimo()
        {
            return Quantidade < QuantidadeMinima;
        }

        public bool TemEstoqueDisponivel(int quantidadeDesejada)
        {
            return Quantidade >= quantidadeDesejada;
        }

        public void AtualizarQuantidadeMinima(int novaQuantidadeMinima)
        {
            if (novaQuantidadeMinima < 0)
                throw new ArgumentException("Quantidade mínima não pode ser negativa", nameof(novaQuantidadeMinima));

            QuantidadeMinima = novaQuantidadeMinima;
            DataUltimaAtualizacao = DateTime.Now;
        }

        public void AtualizarLocalizacao(string? novaLocalizacao)
        {
            Localizacao = novaLocalizacao?.Trim();
            DataUltimaAtualizacao = DateTime.Now;
        }
    }
}
