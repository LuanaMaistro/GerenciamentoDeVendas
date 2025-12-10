using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain
{
    public class EnderecoTest
    {
        [Fact]
        public void Endereco_QuandoValido_CriaInstancia()
        {
            // Arrange & Act
            var endereco = new Endereco(
                "01310100",
                "Avenida Paulista",
                "1000",
                "Sala 101",
                "Bela Vista",
                "São Paulo",
                "SP"
            );

            // Assert
            Assert.NotNull(endereco);
            Assert.Equal("01310100", endereco.CEP);
            Assert.Equal("Avenida Paulista", endereco.Logradouro);
            Assert.Equal("1000", endereco.Numero);
            Assert.Equal("Sala 101", endereco.Complemento);
            Assert.Equal("Bela Vista", endereco.Bairro);
            Assert.Equal("São Paulo", endereco.Cidade);
            Assert.Equal("SP", endereco.UF);
        }

        [Fact]
        public void Endereco_QuandoCEPComMascara_LimpaCorretamente()
        {
            // Arrange & Act
            var endereco = new Endereco(
                "01310-100",
                "Avenida Paulista",
                "1000",
                null,
                "Bela Vista",
                "São Paulo",
                "SP"
            );

            // Assert
            Assert.Equal("01310100", endereco.CEP);
        }

        [Fact]
        public void Endereco_QuandoCEPInvalido_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Endereco(
                "123",
                "Rua Teste",
                "100",
                null,
                "Centro",
                "São Paulo",
                "SP"
            ));
        }

        [Fact]
        public void Endereco_QuandoLogradouroVazio_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Endereco(
                "01310100",
                "",
                "100",
                null,
                "Centro",
                "São Paulo",
                "SP"
            ));
        }

        [Fact]
        public void Endereco_QuandoUFInvalida_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Endereco(
                "01310100",
                "Rua Teste",
                "100",
                null,
                "Centro",
                "São Paulo",
                "XX"
            ));
        }

        [Fact]
        public void Endereco_GetCEPFormatado_RetornaFormatoCorreto()
        {
            // Arrange
            var endereco = new Endereco(
                "01310100",
                "Avenida Paulista",
                "1000",
                null,
                "Bela Vista",
                "São Paulo",
                "SP"
            );

            // Act
            var cepFormatado = endereco.GetCEPFormatado();

            // Assert
            Assert.Equal("01310-100", cepFormatado);
        }

        [Fact]
        public void Endereco_GetEnderecoCompleto_ComComplemento_RetornaCorreto()
        {
            // Arrange
            var endereco = new Endereco(
                "01310100",
                "Avenida Paulista",
                "1000",
                "Sala 101",
                "Bela Vista",
                "São Paulo",
                "SP"
            );

            // Act
            var completo = endereco.GetEnderecoCompleto();

            // Assert
            Assert.Equal("Avenida Paulista, 1000 - Sala 101, Bela Vista, São Paulo - SP, 01310-100", completo);
        }

        [Fact]
        public void Endereco_GetEnderecoCompleto_SemComplemento_RetornaCorreto()
        {
            // Arrange
            var endereco = new Endereco(
                "01310100",
                "Avenida Paulista",
                "1000",
                null,
                "Bela Vista",
                "São Paulo",
                "SP"
            );

            // Act
            var completo = endereco.GetEnderecoCompleto();

            // Assert
            Assert.Equal("Avenida Paulista, 1000, Bela Vista, São Paulo - SP, 01310-100", completo);
        }

        [Fact]
        public void Endereco_Equals_QuandoIguais_RetornaTrue()
        {
            // Arrange
            var endereco1 = new Endereco("01310100", "Avenida Paulista", "1000", null, "Bela Vista", "São Paulo", "SP");
            var endereco2 = new Endereco("01310-100", "Avenida Paulista", "1000", null, "Bela Vista", "São Paulo", "sp");

            // Act & Assert
            Assert.True(endereco1 == endereco2);
        }

        [Fact]
        public void Endereco_Equals_QuandoDiferentes_RetornaFalse()
        {
            // Arrange
            var endereco1 = new Endereco("01310100", "Avenida Paulista", "1000", null, "Bela Vista", "São Paulo", "SP");
            var endereco2 = new Endereco("01310100", "Avenida Paulista", "2000", null, "Bela Vista", "São Paulo", "SP");

            // Act & Assert
            Assert.False(endereco1 == endereco2);
            Assert.True(endereco1 != endereco2);
        }

        [Fact]
        public void Endereco_ComplementoVazio_TransformaEmNull()
        {
            // Arrange & Act
            var endereco = new Endereco(
                "01310100",
                "Avenida Paulista",
                "1000",
                "   ",
                "Bela Vista",
                "São Paulo",
                "SP"
            );

            // Assert
            Assert.Null(endereco.Complemento);
        }

        [Theory]
        [InlineData("AC")]
        [InlineData("SP")]
        [InlineData("RJ")]
        [InlineData("MG")]
        [InlineData("RS")]
        public void Endereco_TodasUFsValidas_CriaInstancia(string uf)
        {
            // Arrange & Act
            var endereco = new Endereco("01310100", "Rua Teste", "100", null, "Centro", "Cidade", uf);

            // Assert
            Assert.Equal(uf, endereco.UF);
        }
    }
}
