using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum StatusVenda
    {
        Pendente,
        Confirmada,
        Cancelada
    }

    public enum FormaPagamento
    {
        Dinheiro,
        CartaoCredito,
        CartaoDebito,
        Pix,
        Boleto,
        Transferencia
    }

    public class Venda
    {
        public Guid Id { get; private set; }
        public Guid ClienteId { get; private set; }
        public DateTime DataVenda { get; private set; }
        public List<ItemVenda> Itens { get; private set; }
        public decimal ValorTotal => Itens.Sum(i => i.Subtotal);
        public StatusVenda Status { get; private set; }
        public FormaPagamento? FormaPagamento { get; private set; }
        public string? Observacao { get; private set; }

        public Venda(Guid clienteId, string? observacao = null)
        {
            if (clienteId == Guid.Empty)
                throw new ArgumentException("ClienteId é obrigatório", nameof(clienteId));

            Id = Guid.NewGuid();
            ClienteId = clienteId;
            DataVenda = DateTime.Now;
            Itens = new List<ItemVenda>();
            Status = StatusVenda.Pendente;
            Observacao = observacao?.Trim();
        }

        public void AdicionarItem(Guid produtoId, string produtoNome, int quantidade, decimal precoUnitario)
        {
            if (Status != StatusVenda.Pendente)
                throw new InvalidOperationException("Não é possível adicionar itens a uma venda que não está pendente");

            var itemExistente = Itens.FirstOrDefault(i => i.ProdutoId == produtoId);

            if (itemExistente != null)
            {
                itemExistente.AtualizarQuantidade(itemExistente.Quantidade + quantidade);
            }
            else
            {
                var novoItem = new ItemVenda(Id, produtoId, produtoNome, quantidade, precoUnitario);
                Itens.Add(novoItem);
            }
        }

        public void RemoverItem(Guid itemId)
        {
            if (Status != StatusVenda.Pendente)
                throw new InvalidOperationException("Não é possível remover itens de uma venda que não está pendente");

            var item = Itens.FirstOrDefault(i => i.Id == itemId);

            if (item == null)
                throw new InvalidOperationException("Item não encontrado na venda");

            Itens.Remove(item);
        }

        public void AtualizarQuantidadeItem(Guid itemId, int novaQuantidade)
        {
            if (Status != StatusVenda.Pendente)
                throw new InvalidOperationException("Não é possível alterar itens de uma venda que não está pendente");

            var item = Itens.FirstOrDefault(i => i.Id == itemId);

            if (item == null)
                throw new InvalidOperationException("Item não encontrado na venda");

            item.AtualizarQuantidade(novaQuantidade);
        }

        public void Confirmar(FormaPagamento formaPagamento)
        {
            if (Status != StatusVenda.Pendente)
                throw new InvalidOperationException("Somente vendas pendentes podem ser confirmadas");

            if (!Itens.Any())
                throw new InvalidOperationException("Não é possível confirmar uma venda sem itens");

            FormaPagamento = formaPagamento;
            Status = StatusVenda.Confirmada;
        }

        public void Cancelar()
        {
            if (Status == StatusVenda.Cancelada)
                throw new InvalidOperationException("Venda já está cancelada");

            Status = StatusVenda.Cancelada;
        }

        public void AtualizarObservacao(string? novaObservacao)
        {
            Observacao = novaObservacao?.Trim();
        }

        public bool PodeSerEditada()
        {
            return Status == StatusVenda.Pendente;
        }
    }
}
