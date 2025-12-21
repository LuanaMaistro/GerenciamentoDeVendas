using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ItemVenda
    {
        public Guid Id { get; private set; }
        public Guid VendaId { get; private set; }
        public Guid ProdutoId { get; private set; }
        public string ProdutoNome { get; private set; }
        public int Quantidade { get; private set; }
        public decimal PrecoUnitario { get; private set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;

        public ItemVenda(Guid vendaId, Guid produtoId, string produtoNome, int quantidade, decimal precoUnitario)
        {
            if (vendaId == Guid.Empty)
                throw new ArgumentException("VendaId é obrigatório", nameof(vendaId));

            if (produtoId == Guid.Empty)
                throw new ArgumentException("ProdutoId é obrigatório", nameof(produtoId));

            if (string.IsNullOrWhiteSpace(produtoNome))
                throw new ArgumentException("Nome do produto é obrigatório", nameof(produtoNome));

            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

            if (precoUnitario < 0)
                throw new ArgumentException("Preço unitário não pode ser negativo", nameof(precoUnitario));

            Id = Guid.NewGuid();
            VendaId = vendaId;
            ProdutoId = produtoId;
            ProdutoNome = produtoNome.Trim();
            Quantidade = quantidade;
            PrecoUnitario = precoUnitario;
        }

        public void AtualizarQuantidade(int novaQuantidade)
        {
            if (novaQuantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero", nameof(novaQuantidade));

            Quantidade = novaQuantidade;
        }

        internal void AssociarVenda(Guid vendaId)
        {
            if (vendaId == Guid.Empty)
                throw new ArgumentException("VendaId é obrigatório", nameof(vendaId));

            VendaId = vendaId;
        }
    }
}
