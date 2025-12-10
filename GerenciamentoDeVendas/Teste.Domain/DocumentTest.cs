using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain
{
    public class DocumentTest
    {
        [Fact]
        public void Documento_QuandoCPF_IdentificaTipoCPF()
        {
            // Arrange & Act
            var documento = new Documento("45502905870");

            // Assert
            Assert.True(documento.IsCPF);
            Assert.False(documento.IsCNPJ);
            Assert.Equal(TipoDocumento.CPF, documento.Tipo);
        }

        [Fact]
        public void Documento_QuandoCNPJ_IdentificaTipoCNPJ()
        {
            // Arrange & Act
            var documento = new Documento("11222333000181");

            // Assert
            Assert.True(documento.IsCNPJ);
            Assert.False(documento.IsCPF);
            Assert.Equal(TipoDocumento.CNPJ, documento.Tipo);
        }

        [Fact]
        public void Documento_CPFComMascara_LimpaEIdentifica()
        {
            // Arrange & Act
            var documento = new Documento("455.029.058-70");

            // Assert
            Assert.True(documento.IsCPF);
            Assert.Equal("45502905870", documento.Numero);
        }

        [Fact]
        public void Documento_CNPJComMascara_LimpaEIdentifica()
        {
            // Arrange & Act
            var documento = new Documento("11.222.333/0001-81");

            // Assert
            Assert.True(documento.IsCNPJ);
            Assert.Equal("11222333000181", documento.Numero);
        }

        [Fact]
        public void Documento_GetCPF_RetornaCPFValido()
        {
            // Arrange
            var documento = new Documento("45502905870");

            // Act
            var cpf = documento.GetCPF();

            // Assert
            Assert.NotNull(cpf);
            Assert.Equal("45502905870", cpf.Value);
        }

        [Fact]
        public void Documento_GetCNPJ_RetornaCNPJValido()
        {
            // Arrange
            var documento = new Documento("11222333000181");

            // Act
            var cnpj = documento.GetCNPJ();

            // Assert
            Assert.NotNull(cnpj);
            Assert.Equal("11222333000181", cnpj.Value);
        }

        [Fact]
        public void Documento_CPF_GetCNPJ_RetornaNull()
        {
            // Arrange
            var documento = new Documento("45502905870");

            // Act
            var cnpj = documento.GetCNPJ();

            // Assert
            Assert.Null(cnpj);
        }

        [Fact]
        public void Documento_CNPJ_GetCPF_RetornaNull()
        {
            // Arrange
            var documento = new Documento("11222333000181");

            // Act
            var cpf = documento.GetCPF();

            // Assert
            Assert.Null(cpf);
        }

        [Fact]
        public void Documento_GetFormatado_CPF_RetornaFormatado()
        {
            // Arrange
            var documento = new Documento("45502905870");

            // Act
            var formatado = documento.GetFormatado();

            // Assert
            Assert.Equal("455.029.058-70", formatado);
        }

        [Fact]
        public void Documento_GetFormatado_CNPJ_RetornaFormatado()
        {
            // Arrange
            var documento = new Documento("11222333000181");

            // Act
            var formatado = documento.GetFormatado();

            // Assert
            Assert.Equal("11.222.333/0001-81", formatado);
        }

        [Fact]
        public void Documento_ToString_RetornaDescricaoCompleta()
        {
            // Arrange
            var documentoCPF = new Documento("45502905870");
            var documentoCNPJ = new Documento("11222333000181");

            // Act & Assert
            Assert.Equal("CPF: 455.029.058-70", documentoCPF.ToString());
            Assert.Equal("CNPJ: 11.222.333/0001-81", documentoCNPJ.ToString());
        }

        [Fact]
        public void Documento_TamanhoInvalido_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Documento("123456789"));
        }

        [Fact]
        public void Documento_CPFInvalido_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Documento("12345678901"));
        }

        [Fact]
        public void Documento_CNPJInvalido_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Documento("12345678901234"));
        }

        [Fact]
        public void Documento_Equals_CPFsIguais_RetornaTrue()
        {
            // Arrange
            var doc1 = new Documento("45502905870");
            var doc2 = new Documento("455.029.058-70");

            // Act & Assert
            Assert.True(doc1 == doc2);
            Assert.True(doc1.Equals(doc2));
        }

        [Fact]
        public void Documento_Equals_CNPJsIguais_RetornaTrue()
        {
            // Arrange
            var doc1 = new Documento("11222333000181");
            var doc2 = new Documento("11.222.333/0001-81");

            // Act & Assert
            Assert.True(doc1 == doc2);
        }

        [Fact]
        public void Documento_Equals_TiposDiferentes_RetornaFalse()
        {
            // Arrange
            var docCPF = new Documento("45502905870");
            var docCNPJ = new Documento("11222333000181");

            // Act & Assert
            Assert.False(docCPF == docCNPJ);
            Assert.True(docCPF != docCNPJ);
        }

        [Fact]
        public void Documento_FromCPF_CriaDocumentoCPF()
        {
            // Arrange & Act
            var documento = Documento.FromCPF("45502905870");

            // Assert
            Assert.True(documento.IsCPF);
        }

        [Fact]
        public void Documento_FromCNPJ_CriaDocumentoCNPJ()
        {
            // Arrange & Act
            var documento = Documento.FromCNPJ("11222333000181");

            // Assert
            Assert.True(documento.IsCNPJ);
        }

        [Fact]
        public void Documento_GetHashCode_DocumentosIguaisTemMesmoHash()
        {
            // Arrange
            var doc1 = new Documento("45502905870");
            var doc2 = new Documento("455.029.058-70");

            // Act & Assert
            Assert.Equal(doc1.GetHashCode(), doc2.GetHashCode());
        }
    }
}
