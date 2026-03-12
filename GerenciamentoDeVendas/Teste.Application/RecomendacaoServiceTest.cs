using Application.DTOs;
using Application.Interfaces.Services;
using Moq;

namespace Teste.Application
{
    /// <summary>
    /// Testa o contrato de IRecomendacaoService (mock) e os comportamentos
    /// esperados pelos serviços que o consomem.
    /// A implementação real (RecomendacaoService) é coberta pelos Testes.Integration,
    /// pois depende de rede com o Recombee.
    /// </summary>
    public class RecomendacaoServiceTest
    {
        private readonly Mock<IRecomendacaoService> _serviceMock;

        public RecomendacaoServiceTest()
        {
            _serviceMock = new Mock<IRecomendacaoService>();
        }

        // ─── SincronizarProdutoAsync ───────────────────────────────────────

        [Fact]
        public async Task SincronizarProdutoAsync_QuandoChamado_CompletaSemErro()
        {
            _serviceMock.Setup(s => s.SincronizarProdutoAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<decimal>()))
                .Returns(Task.CompletedTask);

            await _serviceMock.Object.SincronizarProdutoAsync(Guid.NewGuid(), "Produto X", "Cat A", 99.90m);

            _serviceMock.Verify(s => s.SincronizarProdutoAsync(
                It.IsAny<Guid>(), "Produto X", "Cat A", 99.90m), Times.Once);
        }

        // ─── RegistrarVisualizacaoAsync ────────────────────────────────────

        [Fact]
        public async Task RegistrarVisualizacaoAsync_QuandoChamado_CompletaSemErro()
        {
            var clienteId = Guid.NewGuid();
            var produtoId = Guid.NewGuid();

            _serviceMock.Setup(s => s.RegistrarVisualizacaoAsync(clienteId, produtoId))
                .Returns(Task.CompletedTask);

            await _serviceMock.Object.RegistrarVisualizacaoAsync(clienteId, produtoId);

            _serviceMock.Verify(s => s.RegistrarVisualizacaoAsync(clienteId, produtoId), Times.Once);
        }

        // ─── RegistrarCompraAsync ──────────────────────────────────────────

        [Fact]
        public async Task RegistrarCompraAsync_QuandoChamado_CompletaSemErro()
        {
            var clienteId = Guid.NewGuid();
            var produtoId = Guid.NewGuid();

            _serviceMock.Setup(s => s.RegistrarCompraAsync(clienteId, produtoId, 3))
                .Returns(Task.CompletedTask);

            await _serviceMock.Object.RegistrarCompraAsync(clienteId, produtoId, 3);

            _serviceMock.Verify(s => s.RegistrarCompraAsync(clienteId, produtoId, 3), Times.Once);
        }

        // ─── ObterRecomendacoesAsync ───────────────────────────────────────

        [Fact]
        public async Task ObterRecomendacoesAsync_RetornaResultadoComItens()
        {
            var clienteId = Guid.NewGuid();
            var itens = new List<RecomendacaoItemDTO>
            {
                new(Guid.NewGuid(), "Produto A", "Eletrônicos", 1500m),
                new(Guid.NewGuid(), "Produto B", "Eletrônicos", 2000m)
            };
            var esperado = new RecomendacaoResultadoDTO(clienteId, itens);

            _serviceMock.Setup(s => s.ObterRecomendacoesAsync(clienteId, 5)).ReturnsAsync(esperado);

            var resultado = await _serviceMock.Object.ObterRecomendacoesAsync(clienteId, 5);

            Assert.Equal(clienteId, resultado.ClienteId);
            Assert.Equal(2, resultado.Itens.Count());
        }

        [Fact]
        public async Task ObterRecomendacoesAsync_SemHistorico_RetornaListaVazia()
        {
            var clienteId = Guid.NewGuid();
            var esperado = new RecomendacaoResultadoDTO(clienteId, Enumerable.Empty<RecomendacaoItemDTO>());

            _serviceMock.Setup(s => s.ObterRecomendacoesAsync(clienteId, It.IsAny<int>()))
                .ReturnsAsync(esperado);

            var resultado = await _serviceMock.Object.ObterRecomendacoesAsync(clienteId);

            Assert.Empty(resultado.Itens);
        }
    }
}
