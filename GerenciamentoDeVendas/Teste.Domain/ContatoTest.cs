using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain
{
    public class ContatoTest
    {
        [Fact]
        public void Contato_SomenteTelefoneValido_CriaInstancia()
        {
            // Arrange & Act
            var contato = new Contato("1132223333", null, null);

            // Assert
            Assert.NotNull(contato);
            Assert.Equal("1132223333", contato.Telefone);
            Assert.Null(contato.Celular);
            Assert.Null(contato.Email);
        }

        [Fact]
        public void Contato_SomenteCelularValido_CriaInstancia()
        {
            // Arrange & Act
            var contato = new Contato(null, "11987654321", null);

            // Assert
            Assert.NotNull(contato);
            Assert.Null(contato.Telefone);
            Assert.Equal("11987654321", contato.Celular);
            Assert.Null(contato.Email);
        }

        [Fact]
        public void Contato_SomenteEmailValido_CriaInstancia()
        {
            // Arrange & Act
            var contato = new Contato(null, null, "teste@email.com");

            // Assert
            Assert.NotNull(contato);
            Assert.Null(contato.Telefone);
            Assert.Null(contato.Celular);
            Assert.Equal("teste@email.com", contato.Email);
        }

        [Fact]
        public void Contato_TodosPreenchidos_CriaInstancia()
        {
            // Arrange & Act
            var contato = new Contato("1132223333", "11987654321", "teste@email.com");

            // Assert
            Assert.Equal("1132223333", contato.Telefone);
            Assert.Equal("11987654321", contato.Celular);
            Assert.Equal("teste@email.com", contato.Email);
        }

        [Fact]
        public void Contato_TelefoneComMascara_LimpaCorretamente()
        {
            // Arrange & Act
            var contato = new Contato("(11) 3222-3333", null, null);

            // Assert
            Assert.Equal("1132223333", contato.Telefone);
        }

        [Fact]
        public void Contato_CelularComMascara_LimpaCorretamente()
        {
            // Arrange & Act
            var contato = new Contato(null, "(11) 98765-4321", null);

            // Assert
            Assert.Equal("11987654321", contato.Celular);
        }

        [Fact]
        public void Contato_EmailComMaiusculas_ConvertePraMinusculas()
        {
            // Arrange & Act
            var contato = new Contato(null, null, "TESTE@EMAIL.COM");

            // Assert
            Assert.Equal("teste@email.com", contato.Email);
        }

        [Fact]
        public void Contato_NenhumCampo_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Contato(null, null, null));
        }

        [Fact]
        public void Contato_TelefoneInvalido_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Contato("123456", null, null));
        }

        [Fact]
        public void Contato_CelularSemNove_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Contato(null, "11887654321", null));
        }

        [Fact]
        public void Contato_EmailInvalido_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Contato(null, null, "emailinvalido"));
        }

        [Fact]
        public void Contato_GetTelefoneFormatado_RetornaCorreto()
        {
            // Arrange
            var contato = new Contato("1132223333", null, null);

            // Act
            var formatado = contato.GetTelefoneFormatado();

            // Assert
            Assert.Equal("(11) 3222-3333", formatado);
        }

        [Fact]
        public void Contato_GetCelularFormatado_RetornaCorreto()
        {
            // Arrange
            var contato = new Contato(null, "11987654321", null);

            // Act
            var formatado = contato.GetCelularFormatado();

            // Assert
            Assert.Equal("(11) 98765-4321", formatado);
        }

        [Fact]
        public void Contato_GetTelefoneFormatado_SemTelefone_RetornaVazio()
        {
            // Arrange
            var contato = new Contato(null, null, "teste@email.com");

            // Act & Assert
            Assert.Equal(string.Empty, contato.GetTelefoneFormatado());
            Assert.Equal(string.Empty, contato.GetCelularFormatado());
        }

        [Fact]
        public void Contato_Equals_QuandoIguais_RetornaTrue()
        {
            // Arrange
            var contato1 = new Contato(null, null, "teste@email.com");
            var contato2 = new Contato(null, null, "TESTE@EMAIL.COM");

            // Act & Assert
            Assert.True(contato1 == contato2);
        }

        [Fact]
        public void Contato_Equals_QuandoDiferentes_RetornaFalse()
        {
            // Arrange
            var contato1 = new Contato("1132223333", null, null);
            var contato2 = new Contato(null, null, "teste@email.com");

            // Act & Assert
            Assert.False(contato1 == contato2);
        }

        [Theory]
        [InlineData("teste@email.com")]
        [InlineData("usuario.nome@empresa.com.br")]
        [InlineData("nome+tag@gmail.com")]
        public void Contato_EmailsValidos_CriaInstancia(string email)
        {
            // Arrange & Act
            var contato = new Contato(null, null, email);

            // Assert
            Assert.NotNull(contato);
        }

        [Theory]
        [InlineData("emailsemarroba")]
        [InlineData("@semdominio.com")]
        [InlineData("email@.com")]
        public void Contato_EmailsInvalidos_LancaExcecao(string email)
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Contato(null, null, email));
        }
    }
}
