using Domain.Entities;

namespace Test.Domain
{
    public class CustomerTest
    {
        [Fact]
        public void Inativa_QuandoChamado_InativoTrue()
        {
            // Arrange
            var cliente = new Cliente();

            // Act
            cliente.Inativa();

            // Assert
            Assert.True(cliente.Inativo);
        }

        [Fact]
        public void Ativa_QuandoChamado_InativoFalse()
        {
            // Arrange
            var cliente = new Cliente();
            cliente.Inativa(); // Primeiro inativa

            // Act
            cliente.Ativa(); // Depois ativa

            // Assert
            Assert.False(cliente.Inativo);
        }

        [Fact]
        public void Cliente_PorPadrao_NaoEstaInativo()
        {
            // Arrange & Act
            var cliente = new Cliente();

            // Assert
            Assert.False(cliente.Inativo);
        }
    }
}
