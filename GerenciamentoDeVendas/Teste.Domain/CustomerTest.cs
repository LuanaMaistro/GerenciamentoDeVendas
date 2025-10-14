using Domain.Entities;

namespace Test.Domain
{
    public class CustomerTest
    {
        [Fact]
        public void AtivaInativaCliente_QuandoAtivo_InativoTrue()
        {
            //Arrange
            var cliente = new Cliente();

            //Act
            cliente.AtivaInativa();

            //Assert
            Assert.True(cliente.Inativo);


        }

        [Fact]

        //Quando o cliente era inativo e agora vai ficar ativo, quero a flag de inativo false
        public void AtivaInativaCliente_QuandoInativo_InativoFalse()
        {
            //Arrange
            var cliente = new Cliente();
            cliente.AtivaInativa();

            //Act
            cliente.AtivaInativa();

            //Assert
            Assert.False(cliente.Inativo);


        }
    }
}