using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using Moq;

namespace Teste.Application
{
    public class ClienteServiceTest
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IClienteRepository> _clienteRepoMock;
        private readonly ClienteService _service;

        // CPF válido reutilizado nos testes
        private const string CpfValido = "45502905870";

        public ClienteServiceTest()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _clienteRepoMock = new Mock<IClienteRepository>();
            _uowMock.Setup(u => u.Clientes).Returns(_clienteRepoMock.Object);
            _service = new ClienteService(_uowMock.Object);
        }

        // ─── ObterPorIdAsync ───────────────────────────────────────────────

        [Fact]
        public async Task ObterPorIdAsync_QuandoExiste_RetornaDTO()
        {
            var cliente = CriarCliente("Maria Silva", CpfValido);
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);

            var resultado = await _service.ObterPorIdAsync(cliente.Id);

            Assert.NotNull(resultado);
            Assert.Equal(cliente.Id, resultado.Id);
            Assert.Equal("Maria Silva", resultado.Nome);
        }

        [Fact]
        public async Task ObterPorIdAsync_QuandoNaoExiste_RetornaNull()
        {
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cliente?)null);

            var resultado = await _service.ObterPorIdAsync(Guid.NewGuid());

            Assert.Null(resultado);
        }

        // ─── ObterTodosAsync ───────────────────────────────────────────────

        [Fact]
        public async Task ObterTodosAsync_RetornaListaMapeada()
        {
            var clientes = new List<Cliente>
            {
                CriarCliente("Cliente A", "45502905870"),
                CriarCliente("Cliente B", "36700137845")
            };
            _clienteRepoMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(clientes);

            var resultado = await _service.ObterTodosAsync();

            Assert.Equal(2, resultado.Count());
        }

        // ─── CriarAsync ────────────────────────────────────────────────────

        [Fact]
        public async Task CriarAsync_DocumentoDuplicado_LancaInvalidOperationException()
        {
            var doc = new Documento(CpfValido);
            _clienteRepoMock.Setup(r => r.DocumentoJaCadastradoAsync(It.IsAny<Documento>())).ReturnsAsync(true);

            var dto = new ClienteCreateDTO("João Silva", CpfValido, null, null, null, null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        }

        [Fact]
        public async Task CriarAsync_DadosValidos_CriaERetornaDTO()
        {
            _clienteRepoMock.Setup(r => r.DocumentoJaCadastradoAsync(It.IsAny<Documento>())).ReturnsAsync(false);
            _clienteRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Cliente>())).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var dto = new ClienteCreateDTO("João Silva", CpfValido, null, null, null, null);

            var resultado = await _service.CriarAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal("João Silva", resultado.Nome);
            _clienteRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Cliente>()), Times.Once);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        // ─── AtualizarAsync ────────────────────────────────────────────────

        [Fact]
        public async Task AtualizarAsync_ClienteNaoEncontrado_LancaInvalidOperationException()
        {
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cliente?)null);

            var dto = new ClienteUpdateDTO("Novo Nome", null, null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AtualizarAsync(Guid.NewGuid(), dto));
        }

        [Fact]
        public async Task AtualizarAsync_ClienteEncontrado_AtualizaNomeERetornaDTO()
        {
            var cliente = CriarCliente("Nome Antigo", CpfValido);
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);
            _uowMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var dto = new ClienteUpdateDTO("Nome Novo", null, null);

            var resultado = await _service.AtualizarAsync(cliente.Id, dto);

            Assert.Equal("Nome Novo", resultado.Nome);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        // ─── AtivarAsync / InativarAsync ───────────────────────────────────

        [Fact]
        public async Task AtivarAsync_ClienteNaoEncontrado_LancaInvalidOperationException()
        {
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cliente?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AtivarAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task InativarAsync_ClienteNaoEncontrado_LancaInvalidOperationException()
        {
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cliente?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.InativarAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task InativarAsync_ClienteAtivo_InativaEPersiste()
        {
            var cliente = CriarCliente("João", CpfValido);
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);
            _uowMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            await _service.InativarAsync(cliente.Id);

            Assert.False(cliente.Ativo);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task AtivarAsync_ClienteInativo_AtivaEPersiste()
        {
            var cliente = CriarCliente("João", CpfValido);
            cliente.Inativar();
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);
            _uowMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            await _service.AtivarAsync(cliente.Id);

            Assert.True(cliente.Ativo);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        // ─── ExisteAsync / DocumentoJaCadastradoAsync ─────────────────────

        [Fact]
        public async Task ExisteAsync_DelegaAoRepositorio()
        {
            var id = Guid.NewGuid();
            _clienteRepoMock.Setup(r => r.ExisteAsync(id)).ReturnsAsync(true);

            var resultado = await _service.ExisteAsync(id);

            Assert.True(resultado);
        }

        [Fact]
        public async Task DocumentoJaCadastradoAsync_DelegaAoRepositorio()
        {
            _clienteRepoMock.Setup(r => r.DocumentoJaCadastradoAsync(It.IsAny<Documento>())).ReturnsAsync(true);

            var resultado = await _service.DocumentoJaCadastradoAsync(CpfValido);

            Assert.True(resultado);
        }

        // ─── Helpers ──────────────────────────────────────────────────────

        private static Cliente CriarCliente(string nome, string cpf)
        {
            var documento = new Documento(cpf);
            return new Cliente(nome, documento);
        }
    }
}
