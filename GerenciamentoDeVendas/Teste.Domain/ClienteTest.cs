using Domain.Entities;
using Domain.ValueObjects;

namespace Test.Domain
{
    public class ClienteTest
    {
        private static Documento CriarDocumentoValido()
        {
            return new Documento("529.982.247-25"); // CPF válido
        }

        private static Endereco CriarEnderecoValido()
        {
            return new Endereco("01310-100", "Av. Paulista", "1000", null, "Bela Vista", "São Paulo", "SP");
        }

        [Fact]
        public void Cliente_QuandoValido_CriaInstancia()
        {
            // Arrange & Act
            var cliente = new Cliente("João da Silva", CriarDocumentoValido());

            // Assert
            Assert.NotNull(cliente);
            Assert.NotEqual(Guid.Empty, cliente.Id);
            Assert.Equal("João da Silva", cliente.Nome);
            Assert.NotNull(cliente.Documento);
            Assert.True(cliente.Ativo);
        }

        [Fact]
        public void Cliente_NomeVazio_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Cliente("", CriarDocumentoValido()));
        }

        [Fact]
        public void Cliente_DocumentoNulo_LancaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Cliente("João", null!));
        }

        [Fact]
        public void Inativar_QuandoChamado_AtivoFalse()
        {
            // Arrange
            var cliente = new Cliente("João da Silva", CriarDocumentoValido());

            // Act
            cliente.Inativar();

            // Assert
            Assert.False(cliente.Ativo);
        }

        [Fact]
        public void Ativar_QuandoChamado_AtivoTrue()
        {
            // Arrange
            var cliente = new Cliente("João da Silva", CriarDocumentoValido());
            cliente.Inativar();

            // Act
            cliente.Ativar();

            // Assert
            Assert.True(cliente.Ativo);
        }

        [Fact]
        public void Cliente_PorPadrao_EstaAtivo()
        {
            // Arrange & Act
            var cliente = new Cliente("João da Silva", CriarDocumentoValido());

            // Assert
            Assert.True(cliente.Ativo);
        }

        [Fact]
        public void AtualizarNome_QuandoValido_AtualizaCorretamente()
        {
            // Arrange
            var cliente = new Cliente("João da Silva", CriarDocumentoValido());

            // Act
            cliente.AtualizarNome("Maria Santos");

            // Assert
            Assert.Equal("Maria Santos", cliente.Nome);
        }

        [Fact]
        public void AtualizarNome_NomeVazio_LancaExcecao()
        {
            // Arrange
            var cliente = new Cliente("João da Silva", CriarDocumentoValido());

            // Act & Assert
            Assert.Throws<ArgumentException>(() => cliente.AtualizarNome(""));
        }

        [Fact]
        public void SetEnderecoPrincipal_QuandoValido_DefinePrincipal()
        {
            // Arrange
            var cliente = new Cliente("João da Silva", CriarDocumentoValido());
            var endereco = CriarEnderecoValido();

            // Act
            cliente.SetEnderecoPrincipal(endereco);

            // Assert
            Assert.NotNull(cliente.EnderecoPrincipal);
            Assert.Equal(endereco, cliente.EnderecoPrincipal);
        }

        [Fact]
        public void SetEnderecoPrincipal_EnderecoNulo_LancaExcecao()
        {
            // Arrange
            var cliente = new Cliente("João da Silva", CriarDocumentoValido());

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => cliente.SetEnderecoPrincipal(null!));
        }

        [Fact]
        public void AdicionarEnderecoSecundario_QuandoValido_Adiciona()
        {
            // Arrange
            var cliente = new Cliente("João da Silva", CriarDocumentoValido());
            var endereco = CriarEnderecoValido();

            // Act
            cliente.AdicionarEnderecoSecundario(endereco);

            // Assert
            Assert.Single(cliente.EnderecosSecundarios);
        }

        [Fact]
        public void SetContatoPrincipal_QuandoValido_DefinePrincipal()
        {
            // Arrange
            var cliente = new Cliente("João da Silva", CriarDocumentoValido());
            var contato = new Contato(null, null, "joao@email.com");

            // Act
            cliente.SetContatoPrincipal(contato);

            // Assert
            Assert.NotNull(cliente.ContatoPrincipal);
            Assert.Equal("joao@email.com", cliente.ContatoPrincipal.Email);
        }

        [Fact]
        public void AdicionarContatoSecundario_QuandoValido_Adiciona()
        {
            // Arrange
            var cliente = new Cliente("João da Silva", CriarDocumentoValido());
            var contato = new Contato(null, "11987654321", null);

            // Act
            cliente.AdicionarContatoSecundario(contato);

            // Assert
            Assert.Single(cliente.ContatosSecundarios);
        }

        [Fact]
        public void ContatoPrincipal_QuandoNaoDefinido_RetornaNull()
        {
            // Arrange
            var cliente = new Cliente("João da Silva", CriarDocumentoValido());

            // Assert
            Assert.Null(cliente.ContatoPrincipal);
        }

        [Fact]
        public void Cliente_ComCPF_ArmazenaDocumentoCorreto()
        {
            // Arrange
            var documento = new Documento("529.982.247-25");

            // Act
            var cliente = new Cliente("João da Silva", documento);

            // Assert
            Assert.True(cliente.Documento.IsCPF);
            Assert.Equal("52998224725", cliente.Documento.Numero);
        }

        [Fact]
        public void Cliente_ComCNPJ_ArmazenaDocumentoCorreto()
        {
            // Arrange
            var documento = new Documento("11.222.333/0001-81");

            // Act
            var cliente = new Cliente("Empresa XYZ Ltda", documento);

            // Assert
            Assert.True(cliente.Documento.IsCNPJ);
        }
    }
}
