using Application.DTOs;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Moq;

namespace Teste.Application
{
    public class ProdutoServiceTest
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IProdutoRepository> _produtoRepoMock;
        private readonly Mock<IEstoqueRepository> _estoqueRepoMock;
        private readonly Mock<IRecomendacaoService> _recomendacaoMock;
        private readonly ProdutoService _service;

        public ProdutoServiceTest()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _produtoRepoMock = new Mock<IProdutoRepository>();
            _estoqueRepoMock = new Mock<IEstoqueRepository>();
            _recomendacaoMock = new Mock<IRecomendacaoService>();

            _uowMock.Setup(u => u.Produtos).Returns(_produtoRepoMock.Object);
            _uowMock.Setup(u => u.Estoques).Returns(_estoqueRepoMock.Object);

            // Padrão: ObterPorProdutoIdAsync retorna null (produto sem estoque)
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Estoque?)null);
            _estoqueRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Estoque>()))
                .Returns(Task.CompletedTask);

            _recomendacaoMock.Setup(r => r.SincronizarProdutoAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<decimal>()))
                .Returns(Task.CompletedTask);

            _service = new ProdutoService(_uowMock.Object, _recomendacaoMock.Object);
        }

        // ─── ObterPorIdAsync ───────────────────────────────────────────────

        [Fact]
        public async Task ObterPorIdAsync_QuandoExiste_RetornaDTO()
        {
            var produto = new Produto("P001", "Notebook", 2500m, "Desc", "Eletrônicos");
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);

            var resultado = await _service.ObterPorIdAsync(produto.Id);

            Assert.NotNull(resultado);
            Assert.Equal("P001", resultado.Codigo);
            Assert.Equal("Notebook", resultado.Nome);
            Assert.Equal(0, resultado.Quantidade);
            Assert.Equal(0, resultado.QuantidadeMinima);
        }

        [Fact]
        public async Task ObterPorIdAsync_QuandoNaoExiste_RetornaNull()
        {
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

            var resultado = await _service.ObterPorIdAsync(Guid.NewGuid());

            Assert.Null(resultado);
        }

        // ─── CriarAsync ────────────────────────────────────────────────────

        [Fact]
        public async Task CriarAsync_CodigoDuplicado_LancaInvalidOperationException()
        {
            _produtoRepoMock.Setup(r => r.CodigoJaCadastradoAsync("P001")).ReturnsAsync(true);

            var dto = new ProdutoCreateDTO("P001", "Produto", 10m, null, null, 0, 0);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        }

        [Fact]
        public async Task CriarAsync_DadosValidos_CriaEstoqueERetornaDTO()
        {
            _produtoRepoMock.Setup(r => r.CodigoJaCadastradoAsync("P001")).ReturnsAsync(false);
            _produtoRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Produto>())).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var dto = new ProdutoCreateDTO("P001", "Notebook", 2500m, "Descrição", "Eletrônicos", 10, 5);

            var resultado = await _service.CriarAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal("P001", resultado.Codigo);
            Assert.Equal("Notebook", resultado.Nome);
            _produtoRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Produto>()), Times.Once);
            _estoqueRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Estoque>()), Times.Once);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_FalhaRecombee_NaoBloqueiaCriacao()
        {
            _produtoRepoMock.Setup(r => r.CodigoJaCadastradoAsync("P001")).ReturnsAsync(false);
            _produtoRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Produto>())).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _recomendacaoMock.Setup(r => r.SincronizarProdutoAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<decimal>()))
                .ThrowsAsync(new Exception("Recombee indisponível"));

            var dto = new ProdutoCreateDTO("P001", "Notebook", 2500m, null, null, 0, 0);

            var resultado = await _service.CriarAsync(dto);

            Assert.NotNull(resultado);
        }

        // ─── AtualizarAsync ────────────────────────────────────────────────

        [Fact]
        public async Task AtualizarAsync_ProdutoNaoEncontrado_LancaInvalidOperationException()
        {
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

            var dto = new ProdutoUpdateDTO("Novo Nome", null, 10m, null, 0);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AtualizarAsync(Guid.NewGuid(), dto));
        }

        [Fact]
        public async Task AtualizarAsync_DadosValidos_AtualizaERetornaDTO()
        {
            var produto = new Produto("P001", "Notebook", 2500m, null, "Eletrônicos");
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);
            _uowMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var dto = new ProdutoUpdateDTO("Notebook Pro", "Nova desc", 3000m, "Eletrônicos", 5);

            var resultado = await _service.AtualizarAsync(produto.Id, dto);

            Assert.Equal("Notebook Pro", resultado.Nome);
            Assert.Equal(3000m, resultado.PrecoUnitario);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_ComEstoqueExistente_AtualizaQuantidadeMinima()
        {
            var produto = new Produto("P001", "Notebook", 2500m);
            var estoque = new Estoque(produto.Id, 10, 2);
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produto.Id)).ReturnsAsync(estoque);
            _uowMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var dto = new ProdutoUpdateDTO("Notebook", null, 2500m, null, 8);

            await _service.AtualizarAsync(produto.Id, dto);

            Assert.Equal(8, estoque.QuantidadeMinima);
            _estoqueRepoMock.Verify(r => r.Atualizar(estoque), Times.Once);
        }

        // ─── AtivarAsync / InativarAsync ───────────────────────────────────

        [Fact]
        public async Task AtivarAsync_ProdutoNaoEncontrado_LancaInvalidOperationException()
        {
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AtivarAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task InativarAsync_ProdutoNaoEncontrado_LancaInvalidOperationException()
        {
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.InativarAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task InativarAsync_ProdutoAtivo_InativaEPersiste()
        {
            var produto = new Produto("P001", "Notebook", 2500m);
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);
            _uowMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            await _service.InativarAsync(produto.Id);

            Assert.False(produto.Ativo);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task AtivarAsync_ProdutoInativo_AtivaEPersiste()
        {
            var produto = new Produto("P001", "Notebook", 2500m);
            produto.Inativar();
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);
            _uowMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            await _service.AtivarAsync(produto.Id);

            Assert.True(produto.Ativo);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        // ─── AdicionarQuantidadeAsync / RemoverQuantidadeAsync ─────────────

        [Fact]
        public async Task AdicionarQuantidadeAsync_ProdutoNaoEncontrado_LancaInvalidOperationException()
        {
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AdicionarQuantidadeAsync(Guid.NewGuid(), 5));
        }

        [Fact]
        public async Task AdicionarQuantidadeAsync_EstoqueNaoEncontrado_LancaInvalidOperationException()
        {
            var produto = new Produto("P001", "Notebook", 2500m);
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produto.Id)).ReturnsAsync((Estoque?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AdicionarQuantidadeAsync(produto.Id, 5));
        }

        [Fact]
        public async Task AdicionarQuantidadeAsync_DadosValidos_AdicionaEPersiste()
        {
            var produto = new Produto("P001", "Notebook", 2500m);
            var estoque = new Estoque(produto.Id, 10, 5);
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produto.Id)).ReturnsAsync(estoque);
            _uowMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            await _service.AdicionarQuantidadeAsync(produto.Id, 3);

            Assert.Equal(13, estoque.Quantidade);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoverQuantidadeAsync_QuantidadeInsuficiente_LancaInvalidOperationException()
        {
            var produto = new Produto("P001", "Notebook", 2500m);
            var estoque = new Estoque(produto.Id, 2, 0);
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produto.Id)).ReturnsAsync(estoque);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.RemoverQuantidadeAsync(produto.Id, 5));
        }

        [Fact]
        public async Task RemoverQuantidadeAsync_DadosValidos_RemoveEPersiste()
        {
            var produto = new Produto("P001", "Notebook", 2500m);
            var estoque = new Estoque(produto.Id, 10, 5);
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produto.Id)).ReturnsAsync(estoque);
            _uowMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            await _service.RemoverQuantidadeAsync(produto.Id, 4);

            Assert.Equal(6, estoque.Quantidade);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        // ─── CodigoJaCadastradoAsync ───────────────────────────────────────

        [Fact]
        public async Task CodigoJaCadastradoAsync_DelegaAoRepositorio()
        {
            _produtoRepoMock.Setup(r => r.CodigoJaCadastradoAsync("P001")).ReturnsAsync(true);

            var resultado = await _service.CodigoJaCadastradoAsync("P001");

            Assert.True(resultado);
        }
    }
}
