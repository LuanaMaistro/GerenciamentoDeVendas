using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain
{
    public class EstoqueTest
    {
        [Fact]
        public void Estoque_QuandoValido_CriaInstancia()
        {
            // Arrange
            var produtoId = Guid.NewGuid();

            // Act
            var estoque = new Estoque(produtoId, 100, 10, "Prateleira A1");

            // Assert
            Assert.NotNull(estoque);
            Assert.NotEqual(Guid.Empty, estoque.Id);
            Assert.Equal(produtoId, estoque.ProdutoId);
            Assert.Equal(100, estoque.Quantidade);
            Assert.Equal(10, estoque.QuantidadeMinima);
            Assert.Equal("Prateleira A1", estoque.Localizacao);
        }

        [Fact]
        public void Estoque_ProdutoIdVazio_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Estoque(Guid.Empty));
        }

        [Fact]
        public void Estoque_QuantidadeInicialNegativa_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Estoque(Guid.NewGuid(), -10));
        }

        [Fact]
        public void Estoque_QuantidadeMinimaNegativa_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Estoque(Guid.NewGuid(), 10, -5));
        }

        [Fact]
        public void Estoque_AdicionarQuantidade_AumentaEstoque()
        {
            // Arrange
            var estoque = new Estoque(Guid.NewGuid(), 50);

            // Act
            estoque.AdicionarQuantidade(30);

            // Assert
            Assert.Equal(80, estoque.Quantidade);
        }

        [Fact]
        public void Estoque_AdicionarQuantidadeZero_LancaExcecao()
        {
            // Arrange
            var estoque = new Estoque(Guid.NewGuid(), 50);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => estoque.AdicionarQuantidade(0));
        }

        [Fact]
        public void Estoque_AdicionarQuantidadeNegativa_LancaExcecao()
        {
            // Arrange
            var estoque = new Estoque(Guid.NewGuid(), 50);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => estoque.AdicionarQuantidade(-10));
        }

        [Fact]
        public void Estoque_RemoverQuantidade_DiminuiEstoque()
        {
            // Arrange
            var estoque = new Estoque(Guid.NewGuid(), 50);

            // Act
            estoque.RemoverQuantidade(20);

            // Assert
            Assert.Equal(30, estoque.Quantidade);
        }

        [Fact]
        public void Estoque_RemoverQuantidadeZero_LancaExcecao()
        {
            // Arrange
            var estoque = new Estoque(Guid.NewGuid(), 50);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => estoque.RemoverQuantidade(0));
        }

        [Fact]
        public void Estoque_RemoverQuantidadeMaiorQueDisponivel_LancaExcecao()
        {
            // Arrange
            var estoque = new Estoque(Guid.NewGuid(), 50);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => estoque.RemoverQuantidade(100));
        }

        [Fact]
        public void Estoque_EstaAbaixoDoMinimo_RetornaTrue()
        {
            // Arrange
            var estoque = new Estoque(Guid.NewGuid(), 5, 10);

            // Act & Assert
            Assert.True(estoque.EstaAbaixoDoMinimo());
        }

        [Fact]
        public void Estoque_EstaAbaixoDoMinimo_RetornaFalse()
        {
            // Arrange
            var estoque = new Estoque(Guid.NewGuid(), 20, 10);

            // Act & Assert
            Assert.False(estoque.EstaAbaixoDoMinimo());
        }

        [Fact]
        public void Estoque_TemEstoqueDisponivel_RetornaTrue()
        {
            // Arrange
            var estoque = new Estoque(Guid.NewGuid(), 50);

            // Act & Assert
            Assert.True(estoque.TemEstoqueDisponivel(30));
        }

        [Fact]
        public void Estoque_TemEstoqueDisponivel_RetornaFalse()
        {
            // Arrange
            var estoque = new Estoque(Guid.NewGuid(), 50);

            // Act & Assert
            Assert.False(estoque.TemEstoqueDisponivel(100));
        }

        [Fact]
        public void Estoque_AtualizarQuantidadeMinima_AtualizaCorretamente()
        {
            // Arrange
            var estoque = new Estoque(Guid.NewGuid(), 50, 10);

            // Act
            estoque.AtualizarQuantidadeMinima(20);

            // Assert
            Assert.Equal(20, estoque.QuantidadeMinima);
        }

        [Fact]
        public void Estoque_AtualizarQuantidadeMinimaNegativa_LancaExcecao()
        {
            // Arrange
            var estoque = new Estoque(Guid.NewGuid(), 50, 10);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => estoque.AtualizarQuantidadeMinima(-5));
        }

        [Fact]
        public void Estoque_AtualizarLocalizacao_AtualizaCorretamente()
        {
            // Arrange
            var estoque = new Estoque(Guid.NewGuid(), 50, 10, "Prateleira A1");

            // Act
            estoque.AtualizarLocalizacao("Prateleira B2");

            // Assert
            Assert.Equal("Prateleira B2", estoque.Localizacao);
        }
    }
}
