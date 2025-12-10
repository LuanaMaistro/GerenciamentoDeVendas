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
        public void Contato_TelefoneValido_CriaInstancia()
        {
            // Arrange & Act
            var contato = new Contato(TipoContato.Telefone, "1132223333");

            // Assert
            Assert.NotNull(contato);
            Assert.Equal(TipoContato.Telefone, contato.Tipo);
            Assert.Equal("1132223333", contato.Valor);
            Assert.False(contato.Principal);
        }

        [Fact]
        public void Contato_TelefoneComMascara_LimpaCorretamente()
        {
            // Arrange & Act
            var contato = new Contato(TipoContato.Telefone, "(11) 3222-3333");

            // Assert
            Assert.Equal("1132223333", contato.Valor);
        }

        [Fact]
        public void Contato_CelularValido_CriaInstancia()
        {
            // Arrange & Act
            var contato = new Contato(TipoContato.Celular, "11987654321");

            // Assert
            Assert.NotNull(contato);
            Assert.Equal(TipoContato.Celular, contato.Tipo);
            Assert.Equal("11987654321", contato.Valor);
        }

        [Fact]
        public void Contato_CelularComMascara_LimpaCorretamente()
        {
            // Arrange & Act
            var contato = new Contato(TipoContato.Celular, "(11) 98765-4321");

            // Assert
            Assert.Equal("11987654321", contato.Valor);
        }

        [Fact]
        public void Contato_EmailValido_CriaInstancia()
        {
            // Arrange & Act
            var contato = new Contato(TipoContato.Email, "teste@email.com");

            // Assert
            Assert.NotNull(contato);
            Assert.Equal(TipoContato.Email, contato.Tipo);
            Assert.Equal("teste@email.com", contato.Valor);
        }

        [Fact]
        public void Contato_EmailComMaiusculas_ConvertePraMinusculas()
        {
            // Arrange & Act
            var contato = new Contato(TipoContato.Email, "TESTE@EMAIL.COM");

            // Assert
            Assert.Equal("teste@email.com", contato.Valor);
        }

        [Fact]
        public void Contato_TelefoneInvalido_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Contato(TipoContato.Telefone, "123456"));
        }

        [Fact]
        public void Contato_CelularSemNove_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Contato(TipoContato.Celular, "11887654321"));
        }

        [Fact]
        public void Contato_EmailInvalido_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Contato(TipoContato.Email, "emailinvalido"));
        }

        [Fact]
        public void Contato_ValorVazio_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Contato(TipoContato.Email, ""));
        }

        [Fact]
        public void Contato_Principal_True()
        {
            // Arrange & Act
            var contato = new Contato(TipoContato.Email, "teste@email.com", true);

            // Assert
            Assert.True(contato.Principal);
        }

        [Fact]
        public void Contato_GetFormatado_Telefone_RetornaCorreto()
        {
            // Arrange
            var contato = new Contato(TipoContato.Telefone, "1132223333");

            // Act
            var formatado = contato.GetFormatado();

            // Assert
            Assert.Equal("(11) 3222-3333", formatado);
        }

        [Fact]
        public void Contato_GetFormatado_Celular_RetornaCorreto()
        {
            // Arrange
            var contato = new Contato(TipoContato.Celular, "11987654321");

            // Act
            var formatado = contato.GetFormatado();

            // Assert
            Assert.Equal("(11) 98765-4321", formatado);
        }

        [Fact]
        public void Contato_GetFormatado_Email_RetornaIgual()
        {
            // Arrange
            var contato = new Contato(TipoContato.Email, "teste@email.com");

            // Act
            var formatado = contato.GetFormatado();

            // Assert
            Assert.Equal("teste@email.com", formatado);
        }

        [Fact]
        public void Contato_ToString_RetornaDescricaoCompleta()
        {
            // Arrange
            var contato = new Contato(TipoContato.Email, "teste@email.com", true);

            // Act
            var resultado = contato.ToString();

            // Assert
            Assert.Equal("Email: teste@email.com (Principal)", resultado);
        }

        [Fact]
        public void Contato_Equals_QuandoIguais_RetornaTrue()
        {
            // Arrange
            var contato1 = new Contato(TipoContato.Email, "teste@email.com");
            var contato2 = new Contato(TipoContato.Email, "TESTE@EMAIL.COM");

            // Act & Assert
            Assert.True(contato1 == contato2);
        }

        [Fact]
        public void Contato_Equals_TiposDiferentes_RetornaFalse()
        {
            // Arrange
            var contato1 = new Contato(TipoContato.Telefone, "1132223333");
            var contato2 = new Contato(TipoContato.Celular, "11932223333");

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
            var contato = new Contato(TipoContato.Email, email);

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
            Assert.Throws<ArgumentException>(() => new Contato(TipoContato.Email, email));
        }
    }
}
