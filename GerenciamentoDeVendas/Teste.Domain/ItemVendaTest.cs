using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain
{
    public class ItemVendaTest
    {
        [Fact]
        public void ItemVenda_QuandoValido_CriaInstancia()
        {
            // Arrange
            var vendaId = Guid.NewGuid();
            var produtoId = Guid.NewGuid();

            // Act
            var item = new ItemVenda(vendaId, produtoId, "Notebook Dell", 2, 1500m);

            // Assert
            Assert.NotNull(item);
            Assert.NotEqual(Guid.Empty, item.Id);
            Assert.Equal(vendaId, item.VendaId);
            Assert.Equal(produtoId, item.ProdutoId);
            Assert.Equal("Notebook Dell", item.ProdutoNome);
            Assert.Equal(2, item.Quantidade);
            Assert.Equal(1500m, item.PrecoUnitario);
        }

        [Fact]
        public void ItemVenda_VendaIdVazio_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new ItemVenda(Guid.Empty, Guid.NewGuid(), "Produto", 1, 100m));
        }

        [Fact]
        public void ItemVenda_ProdutoIdVazio_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new ItemVenda(Guid.NewGuid(), Guid.Empty, "Produto", 1, 100m));
        }

        [Fact]
        public void ItemVenda_ProdutoNomeVazio_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new ItemVenda(Guid.NewGuid(), Guid.NewGuid(), "", 1, 100m));
        }

        [Fact]
        public void ItemVenda_QuantidadeZero_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new ItemVenda(Guid.NewGuid(), Guid.NewGuid(), "Produto", 0, 100m));
        }

        [Fact]
        public void ItemVenda_QuantidadeNegativa_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new ItemVenda(Guid.NewGuid(), Guid.NewGuid(), "Produto", -1, 100m));
        }

        [Fact]
        public void ItemVenda_PrecoNegativo_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new ItemVenda(Guid.NewGuid(), Guid.NewGuid(), "Produto", 1, -100m));
        }

        [Fact]
        public void ItemVenda_Subtotal_CalculaCorretamente()
        {
            // Arrange
            var item = new ItemVenda(Guid.NewGuid(), Guid.NewGuid(), "Notebook", 3, 1500m);

            // Act & Assert
            Assert.Equal(4500m, item.Subtotal);
        }

        [Fact]
        public void ItemVenda_AtualizarQuantidade_AtualizaCorretamente()
        {
            // Arrange
            var item = new ItemVenda(Guid.NewGuid(), Guid.NewGuid(), "Notebook", 2, 1500m);

            // Act
            item.AtualizarQuantidade(5);

            // Assert
            Assert.Equal(5, item.Quantidade);
            Assert.Equal(7500m, item.Subtotal);
        }

        [Fact]
        public void ItemVenda_AtualizarQuantidadeZero_LancaExcecao()
        {
            // Arrange
            var item = new ItemVenda(Guid.NewGuid(), Guid.NewGuid(), "Notebook", 2, 1500m);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => item.AtualizarQuantidade(0));
        }

        [Fact]
        public void ItemVenda_AtualizarQuantidadeNegativa_LancaExcecao()
        {
            // Arrange
            var item = new ItemVenda(Guid.NewGuid(), Guid.NewGuid(), "Notebook", 2, 1500m);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => item.AtualizarQuantidade(-3));
        }

        [Theory]
        [InlineData(1, 100, 100)]
        [InlineData(2, 50, 100)]
        [InlineData(10, 25.50, 255)]
        [InlineData(3, 33.33, 99.99)]
        public void ItemVenda_Subtotal_CalculaParaDiferentesValores(int quantidade, decimal preco, decimal subtotalEsperado)
        {
            // Arrange
            var item = new ItemVenda(Guid.NewGuid(), Guid.NewGuid(), "Produto", quantidade, preco);

            // Act & Assert
            Assert.Equal(subtotalEsperado, item.Subtotal);
        }
    }
}
