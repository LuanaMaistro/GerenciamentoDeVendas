using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain
{
    public class VendaTest
    {
        [Fact]
        public void Venda_QuandoValida_CriaInstancia()
        {
            // Arrange
            var clienteId = Guid.NewGuid();

            // Act
            var venda = new Venda(clienteId);

            // Assert
            Assert.NotNull(venda);
            Assert.NotEqual(Guid.Empty, venda.Id);
            Assert.Equal(clienteId, venda.ClienteId);
            Assert.Equal(StatusVenda.Pendente, venda.Status);
            Assert.Empty(venda.Itens);
            Assert.Equal(0, venda.ValorTotal);
        }

        [Fact]
        public void Venda_ClienteIdVazio_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Venda(Guid.Empty));
        }

        [Fact]
        public void Venda_AdicionarItem_AdicionaCorretamente()
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());
            var produtoId = Guid.NewGuid();

            // Act
            venda.AdicionarItem(produtoId, "Notebook", 2, 1500m);

            // Assert
            Assert.Single(venda.Itens);
            Assert.Equal(3000m, venda.ValorTotal);
        }

        [Fact]
        public void Venda_AdicionarItemMesmoProduto_IncrementaQuantidade()
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());
            var produtoId = Guid.NewGuid();

            // Act
            venda.AdicionarItem(produtoId, "Notebook", 2, 1500m);
            venda.AdicionarItem(produtoId, "Notebook", 3, 1500m);

            // Assert
            Assert.Single(venda.Itens);
            Assert.Equal(5, venda.Itens[0].Quantidade);
            Assert.Equal(7500m, venda.ValorTotal);
        }

        [Fact]
        public void Venda_AdicionarItemVendaConfirmada_LancaExcecao()
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());
            venda.AdicionarItem(Guid.NewGuid(), "Notebook", 1, 1500m);
            venda.Confirmar(FormaPagamento.Pix);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                venda.AdicionarItem(Guid.NewGuid(), "Mouse", 1, 50m));
        }

        [Fact]
        public void Venda_RemoverItem_RemoveCorretamente()
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());
            venda.AdicionarItem(Guid.NewGuid(), "Notebook", 2, 1500m);
            var itemId = venda.Itens[0].Id;

            // Act
            venda.RemoverItem(itemId);

            // Assert
            Assert.Empty(venda.Itens);
            Assert.Equal(0, venda.ValorTotal);
        }

        [Fact]
        public void Venda_RemoverItemInexistente_LancaExcecao()
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => venda.RemoverItem(Guid.NewGuid()));
        }

        [Fact]
        public void Venda_AtualizarQuantidadeItem_AtualizaCorretamente()
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());
            venda.AdicionarItem(Guid.NewGuid(), "Notebook", 2, 1500m);
            var itemId = venda.Itens[0].Id;

            // Act
            venda.AtualizarQuantidadeItem(itemId, 5);

            // Assert
            Assert.Equal(5, venda.Itens[0].Quantidade);
            Assert.Equal(7500m, venda.ValorTotal);
        }

        [Fact]
        public void Venda_Confirmar_MudaStatusParaConfirmada()
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());
            venda.AdicionarItem(Guid.NewGuid(), "Notebook", 1, 1500m);

            // Act
            venda.Confirmar(FormaPagamento.CartaoCredito);

            // Assert
            Assert.Equal(StatusVenda.Confirmada, venda.Status);
            Assert.Equal(FormaPagamento.CartaoCredito, venda.FormaPagamento);
        }

        [Fact]
        public void Venda_ConfirmarSemItens_LancaExcecao()
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => venda.Confirmar(FormaPagamento.Pix));
        }

        [Fact]
        public void Venda_ConfirmarVendaJaConfirmada_LancaExcecao()
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());
            venda.AdicionarItem(Guid.NewGuid(), "Notebook", 1, 1500m);
            venda.Confirmar(FormaPagamento.Pix);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => venda.Confirmar(FormaPagamento.Dinheiro));
        }

        [Fact]
        public void Venda_Cancelar_MudaStatusParaCancelada()
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());
            venda.AdicionarItem(Guid.NewGuid(), "Notebook", 1, 1500m);

            // Act
            venda.Cancelar();

            // Assert
            Assert.Equal(StatusVenda.Cancelada, venda.Status);
        }

        [Fact]
        public void Venda_CancelarVendaJaCancelada_LancaExcecao()
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());
            venda.Cancelar();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => venda.Cancelar());
        }

        [Fact]
        public void Venda_CancelarVendaConfirmada_Cancela()
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());
            venda.AdicionarItem(Guid.NewGuid(), "Notebook", 1, 1500m);
            venda.Confirmar(FormaPagamento.Pix);

            // Act
            venda.Cancelar();

            // Assert
            Assert.Equal(StatusVenda.Cancelada, venda.Status);
        }

        [Fact]
        public void Venda_PodeSerEditada_RetornaTrueQuandoPendente()
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());

            // Act & Assert
            Assert.True(venda.PodeSerEditada());
        }

        [Fact]
        public void Venda_PodeSerEditada_RetornaFalseQuandoConfirmada()
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());
            venda.AdicionarItem(Guid.NewGuid(), "Notebook", 1, 1500m);
            venda.Confirmar(FormaPagamento.Pix);

            // Act & Assert
            Assert.False(venda.PodeSerEditada());
        }

        [Fact]
        public void Venda_ValorTotal_CalculaCorretamente()
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());

            // Act
            venda.AdicionarItem(Guid.NewGuid(), "Notebook", 2, 1500m);     // 3000
            venda.AdicionarItem(Guid.NewGuid(), "Mouse", 3, 50m);          // 150
            venda.AdicionarItem(Guid.NewGuid(), "Teclado", 1, 200m);       // 200

            // Assert
            Assert.Equal(3350m, venda.ValorTotal);
        }

        [Theory]
        [InlineData(FormaPagamento.Dinheiro)]
        [InlineData(FormaPagamento.CartaoCredito)]
        [InlineData(FormaPagamento.CartaoDebito)]
        [InlineData(FormaPagamento.Pix)]
        [InlineData(FormaPagamento.Boleto)]
        [InlineData(FormaPagamento.Transferencia)]
        public void Venda_Confirmar_AceitaTodasFormasPagamento(FormaPagamento formaPagamento)
        {
            // Arrange
            var venda = new Venda(Guid.NewGuid());
            venda.AdicionarItem(Guid.NewGuid(), "Produto", 1, 100m);

            // Act
            venda.Confirmar(formaPagamento);

            // Assert
            Assert.Equal(formaPagamento, venda.FormaPagamento);
        }
    }
}
