using Application.DTOs;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;
using Moq;

namespace Teste.Application
{
    public class VendaServiceTest
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IVendaRepository> _vendaRepoMock;
        private readonly Mock<IClienteRepository> _clienteRepoMock;
        private readonly Mock<IProdutoRepository> _produtoRepoMock;
        private readonly Mock<IEstoqueRepository> _estoqueRepoMock;
        private readonly Mock<IRecomendacaoService> _recomendacaoMock;
        private readonly VendaService _service;

        private const string CpfValido = "45502905870";

        public VendaServiceTest()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _vendaRepoMock = new Mock<IVendaRepository>();
            _clienteRepoMock = new Mock<IClienteRepository>();
            _produtoRepoMock = new Mock<IProdutoRepository>();
            _estoqueRepoMock = new Mock<IEstoqueRepository>();
            _recomendacaoMock = new Mock<IRecomendacaoService>();

            _uowMock.Setup(u => u.Vendas).Returns(_vendaRepoMock.Object);
            _uowMock.Setup(u => u.Clientes).Returns(_clienteRepoMock.Object);
            _uowMock.Setup(u => u.Produtos).Returns(_produtoRepoMock.Object);
            _uowMock.Setup(u => u.Estoques).Returns(_estoqueRepoMock.Object);
            _uowMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            _recomendacaoMock.Setup(r => r.RegistrarCompraAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            _service = new VendaService(_uowMock.Object, _recomendacaoMock.Object);
        }

        // ─── CriarAsync ────────────────────────────────────────────────────

        [Fact]
        public async Task CriarAsync_ClienteNaoEncontrado_LancaInvalidOperationException()
        {
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cliente?)null);

            var dto = new VendaCreateDTO(
                Guid.NewGuid(),
                null,
                new[] { new ItemVendaCreateDTO(Guid.NewGuid(), 1) });

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        }

        [Fact]
        public async Task CriarAsync_ProdutoNaoEncontrado_LancaInvalidOperationException()
        {
            var cliente = CriarCliente();
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

            var dto = new VendaCreateDTO(
                cliente.Id,
                null,
                new[] { new ItemVendaCreateDTO(Guid.NewGuid(), 1) });

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        }

        [Fact]
        public async Task CriarAsync_ProdutoInativo_LancaInvalidOperationException()
        {
            var cliente = CriarCliente();
            var produto = new Produto("P001", "Notebook", 2500m);
            produto.Inativar();

            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);

            var dto = new VendaCreateDTO(
                cliente.Id,
                null,
                new[] { new ItemVendaCreateDTO(produto.Id, 1) });

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        }

        [Fact]
        public async Task CriarAsync_DadosValidos_CriaVendaComItens()
        {
            var cliente = CriarCliente();
            var produto = new Produto("P001", "Notebook", 2500m);

            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);
            _vendaRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Venda>())).Returns(Task.CompletedTask);

            var dto = new VendaCreateDTO(
                cliente.Id,
                "Venda teste",
                new[] { new ItemVendaCreateDTO(produto.Id, 2) });

            var resultado = await _service.CriarAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal(cliente.Id, resultado.ClienteId);
            Assert.Single(resultado.Itens);
            Assert.Equal(5000m, resultado.ValorTotal);
            Assert.Equal("Pendente", resultado.Status);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        // ─── ConfirmarAsync ────────────────────────────────────────────────

        [Fact]
        public async Task ConfirmarAsync_VendaNaoEncontrada_LancaInvalidOperationException()
        {
            _vendaRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Venda?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ConfirmarAsync(Guid.NewGuid(), new VendaConfirmarDTO("Pix")));
        }

        [Fact]
        public async Task ConfirmarAsync_SemEstoque_LancaInvalidOperationException()
        {
            var venda = CriarVendaComItem(out var produtoId);
            _vendaRepoMock.Setup(r => r.ObterPorIdAsync(venda.Id)).ReturnsAsync(venda);
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produtoId)).ReturnsAsync((Estoque?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ConfirmarAsync(venda.Id, new VendaConfirmarDTO("Pix")));
        }

        [Fact]
        public async Task ConfirmarAsync_EstoqueInsuficiente_LancaInvalidOperationException()
        {
            var venda = CriarVendaComItem(out var produtoId, quantidade: 10);
            var estoque = new Estoque(produtoId, 5, 0);

            _vendaRepoMock.Setup(r => r.ObterPorIdAsync(venda.Id)).ReturnsAsync(venda);
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produtoId)).ReturnsAsync(estoque);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ConfirmarAsync(venda.Id, new VendaConfirmarDTO("Pix")));
        }

        [Fact]
        public async Task ConfirmarAsync_DadosValidos_ConfirmaEBaixaEstoque()
        {
            var venda = CriarVendaComItem(out var produtoId, quantidade: 3);
            var estoque = new Estoque(produtoId, 10, 0);
            var cliente = CriarCliente();

            _vendaRepoMock.Setup(r => r.ObterPorIdAsync(venda.Id)).ReturnsAsync(venda);
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produtoId)).ReturnsAsync(estoque);
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(cliente);

            var resultado = await _service.ConfirmarAsync(venda.Id, new VendaConfirmarDTO("Pix"));

            Assert.Equal("Confirmada", resultado.Status);
            Assert.Equal("Pix", resultado.FormaPagamento);
            Assert.Equal(7, estoque.Quantidade); // 10 - 3
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task ConfirmarAsync_FalhaRecombee_NaoBloqueiaConfirmacao()
        {
            var venda = CriarVendaComItem(out var produtoId, quantidade: 1);
            var estoque = new Estoque(produtoId, 10, 0);
            var cliente = CriarCliente();

            _vendaRepoMock.Setup(r => r.ObterPorIdAsync(venda.Id)).ReturnsAsync(venda);
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produtoId)).ReturnsAsync(estoque);
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(cliente);
            _recomendacaoMock.Setup(r => r.RegistrarCompraAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Recombee indisponível"));

            var resultado = await _service.ConfirmarAsync(venda.Id, new VendaConfirmarDTO("Pix"));

            Assert.Equal("Confirmada", resultado.Status);
        }

        // ─── CancelarAsync ─────────────────────────────────────────────────

        [Fact]
        public async Task CancelarAsync_VendaNaoEncontrada_LancaInvalidOperationException()
        {
            _vendaRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Venda?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CancelarAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task CancelarAsync_VendaPendente_CancelaSemDevolverEstoque()
        {
            var venda = CriarVendaComItem(out var produtoId);
            var cliente = CriarCliente();

            _vendaRepoMock.Setup(r => r.ObterPorIdAsync(venda.Id)).ReturnsAsync(venda);
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(cliente);

            var resultado = await _service.CancelarAsync(venda.Id);

            Assert.Equal("Cancelada", resultado.Status);
            // Venda pendente não aciona devolução de estoque
            _estoqueRepoMock.Verify(r => r.ObterPorProdutoIdAsync(It.IsAny<Guid>()), Times.Never);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelarAsync_VendaConfirmada_DevolveEstoque()
        {
            var venda = CriarVendaComItem(out var produtoId, quantidade: 3);
            var estoque = new Estoque(produtoId, 10, 0); // começa com 10
            var cliente = CriarCliente();

            _vendaRepoMock.Setup(r => r.ObterPorIdAsync(venda.Id)).ReturnsAsync(venda);
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produtoId)).ReturnsAsync(estoque);
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(cliente);

            // Confirma → estoque: 10 - 3 = 7
            await _service.ConfirmarAsync(venda.Id, new VendaConfirmarDTO("Pix"));
            Assert.Equal(7, estoque.Quantidade);

            // Cancela → estoque deve voltar: 7 + 3 = 10
            var resultado = await _service.CancelarAsync(venda.Id);

            Assert.Equal("Cancelada", resultado.Status);
            Assert.Equal(10, estoque.Quantidade);
        }

        [Fact]
        public async Task CancelarAsync_VendaJaCancelada_LancaInvalidOperationException()
        {
            var venda = CriarVendaComItem(out _);
            venda.Cancelar();
            var cliente = CriarCliente();

            _vendaRepoMock.Setup(r => r.ObterPorIdAsync(venda.Id)).ReturnsAsync(venda);
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(cliente);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CancelarAsync(venda.Id));
        }

        // ─── Helpers ──────────────────────────────────────────────────────

        private static Cliente CriarCliente()
        {
            var documento = new Documento(CpfValido);
            return new Cliente("João Silva", documento);
        }

        private static Venda CriarVendaComItem(out Guid produtoId, int quantidade = 2)
        {
            var clienteId = Guid.NewGuid();
            produtoId = Guid.NewGuid();
            var venda = new Venda(clienteId);
            venda.AdicionarItem(produtoId, "Notebook", quantidade, 2500m);
            return venda;
        }
    }
}
