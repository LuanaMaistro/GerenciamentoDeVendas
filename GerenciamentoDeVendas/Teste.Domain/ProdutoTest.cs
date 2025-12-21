using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain
{
    public class ProdutoTest
    {
        [Fact]
        public void Produto_QuandoValido_CriaInstancia()
        {
            // Arrange & Act
            var produto = new Produto("PROD001", "Notebook Dell", 3500.00m);

            // Assert
            Assert.NotNull(produto);
            Assert.NotEqual(Guid.Empty, produto.Id);
            Assert.Equal("PROD001", produto.Codigo);
            Assert.Equal("Notebook Dell", produto.Nome);
            Assert.Equal(3500.00m, produto.PrecoUnitario);
            Assert.True(produto.Ativo);
        }

        [Fact]
        public void Produto_ComDescricaoECategoria_CriaInstancia()
        {
            // Arrange & Act
            var produto = new Produto("PROD001", "Notebook Dell", 3500.00m, "Notebook Intel i7 16GB RAM", "Eletrônicos");

            // Assert
            Assert.Equal("Notebook Intel i7 16GB RAM", produto.Descricao);
            Assert.Equal("Eletrônicos", produto.Categoria);
        }

        [Fact]
        public void Produto_CodigoVazio_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Produto("", "Notebook", 1000m));
        }

        [Fact]
        public void Produto_NomeVazio_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Produto("PROD001", "", 1000m));
        }

        [Fact]
        public void Produto_PrecoNegativo_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Produto("PROD001", "Notebook", -100m));
        }

        [Fact]
        public void Produto_Inativar_MudaStatusParaFalse()
        {
            // Arrange
            var produto = new Produto("PROD001", "Notebook", 1000m);

            // Act
            produto.Inativar();

            // Assert
            Assert.False(produto.Ativo);
        }

        [Fact]
        public void Produto_Ativar_MudaStatusParaTrue()
        {
            // Arrange
            var produto = new Produto("PROD001", "Notebook", 1000m);
            produto.Inativar();

            // Act
            produto.Ativar();

            // Assert
            Assert.True(produto.Ativo);
        }

        [Fact]
        public void Produto_AtualizarPreco_AtualizaCorretamente()
        {
            // Arrange
            var produto = new Produto("PROD001", "Notebook", 1000m);

            // Act
            produto.AtualizarPreco(1500m);

            // Assert
            Assert.Equal(1500m, produto.PrecoUnitario);
        }

        [Fact]
        public void Produto_AtualizarPrecoNegativo_LancaExcecao()
        {
            // Arrange
            var produto = new Produto("PROD001", "Notebook", 1000m);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => produto.AtualizarPreco(-500m));
        }

        [Fact]
        public void Produto_AtualizarNome_AtualizaCorretamente()
        {
            // Arrange
            var produto = new Produto("PROD001", "Notebook", 1000m);

            // Act
            produto.AtualizarNome("Notebook Dell Inspiron");

            // Assert
            Assert.Equal("Notebook Dell Inspiron", produto.Nome);
        }

        [Fact]
        public void Produto_AtualizarNomeVazio_LancaExcecao()
        {
            // Arrange
            var produto = new Produto("PROD001", "Notebook", 1000m);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => produto.AtualizarNome(""));
        }

        [Fact]
        public void Produto_AtualizarDescricao_AtualizaCorretamente()
        {
            // Arrange
            var produto = new Produto("PROD001", "Notebook", 1000m);

            // Act
            produto.AtualizarDescricao("Nova descrição");

            // Assert
            Assert.Equal("Nova descrição", produto.Descricao);
        }

        [Fact]
        public void Produto_AtualizarCategoria_AtualizaCorretamente()
        {
            // Arrange
            var produto = new Produto("PROD001", "Notebook", 1000m);

            // Act
            produto.AtualizarCategoria("Informática");

            // Assert
            Assert.Equal("Informática", produto.Categoria);
        }
    }
}
