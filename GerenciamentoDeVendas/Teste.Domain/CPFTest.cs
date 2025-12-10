using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain
{
    public class CPFTest
    {
        [Fact]
        public void CPF_QuandoValido_CriaInstancia()
        {
            // Arrange & Act
            var cpf = new CPF("45502905870");

            // Assert
            Assert.NotNull(cpf);
            Assert.Equal("45502905870", cpf.Value);
        }

        [Fact]
        public void CPF_QuandoValidoComMascara_CriaInstancia()
        {
            // Arrange & Act
            var cpf = new CPF("455.029.058-70");

            // Assert
            Assert.NotNull(cpf);
            Assert.Equal("45502905870", cpf.Value);
        }

        [Fact]
        public void CPF_QuandoInvalido_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new CPF("12345678901"));
        }

        [Fact]
        public void CPF_QuandoTodosDigitosIguais_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new CPF("11111111111"));
        }

        [Fact]
        public void CPF_QuandoTamanhoIncorreto_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new CPF("123456789"));
        }

        [Fact]
        public void CPF_GetFormatted_RetornaFormatoCorreto()
        {
            // Arrange
            var cpf = new CPF("45502905870");

            // Act
            var formatado = cpf.GetFormatted();

            // Assert
            Assert.Equal("455.029.058-70", formatado);
        }

        [Fact]
        public void CPF_Equals_QuandoIguais_RetornaTrue()
        {
            // Arrange
            var cpf1 = new CPF("45502905870");
            var cpf2 = new CPF("455.029.058-70");

            // Act & Assert
            Assert.True(cpf1 == cpf2);
            Assert.True(cpf1.Equals(cpf2));
        }

        [Fact]
        public void CPF_Equals_QuandoDiferentes_RetornaFalse()
        {
            // Arrange
            var cpf1 = new CPF("45502905870");
            var cpf2 = new CPF("36700137845");

            // Act & Assert
            Assert.False(cpf1 == cpf2);
            Assert.True(cpf1 != cpf2);
        }

        [Fact]
        public void CPF_ToString_RetornaFormatado()
        {
            // Arrange
            var cpf = new CPF("45502905870");

            // Act
            var resultado = cpf.ToString();

            // Assert
            Assert.Equal("455.029.058-70", resultado);
        }

        [Fact]
        public void CPF_GetHashCode_IguaisTemMesmoHash()
        {
            // Arrange
            var cpf1 = new CPF("45502905870");
            var cpf2 = new CPF("455.029.058-70");

            // Act & Assert
            Assert.Equal(cpf1.GetHashCode(), cpf2.GetHashCode());
        }

        [Fact]
        public void CPF_EmLista_Contains_FuncionaCorretamente()
        {
            // Arrange
            var cpf1 = new CPF("45502905870");
            var cpf2 = new CPF("36700137845");
            var lista = new List<CPF> { cpf1 };

            // Act & Assert
            Assert.Contains(new CPF("45502905870"), lista);
            Assert.DoesNotContain(cpf2, lista);
        }
    }
}
