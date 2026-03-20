using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Moq;

namespace Teste.Application
{
    public class EstoqueServiceTest
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IEstoqueRepository> _estoqueRepoMock;
        private readonly Mock<IProdutoRepository> _produtoRepoMock;
        private readonly EstoqueService _service;

        public EstoqueServiceTest()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _estoqueRepoMock = new Mock<IEstoqueRepository>();
            _produtoRepoMock = new Mock<IProdutoRepository>();

            _uowMock.Setup(u => u.Estoques).Returns(_estoqueRepoMock.Object);
            _uowMock.Setup(u => u.Produtos).Returns(_produtoRepoMock.Object);
            _uowMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            _service = new EstoqueService(_uowMock.Object);
        }

        // ─── ObterPorIdAsync ───────────────────────────────────────────────

        [Fact]
        public async Task ObterPorIdAsync_QuandoExiste_RetornaDTO()
        {
            var produtoId = Guid.NewGuid();
            var estoque = new Estoque(produtoId, 50, 10);
            var produto = new Produto("P001", "Notebook", 2500m);

            _estoqueRepoMock.Setup(r => r.ObterPorIdAsync(estoque.Id)).ReturnsAsync(estoque);
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produtoId)).ReturnsAsync(produto);

            var resultado = await _service.ObterPorIdAsync(estoque.Id);

            Assert.NotNull(resultado);
            Assert.Equal(50, resultado.Quantidade);
            Assert.Equal("Notebook", resultado.ProdutoNome);
        }

        [Fact]
        public async Task ObterPorIdAsync_QuandoNaoExiste_RetornaNull()
        {
            _estoqueRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Estoque?)null);

            var resultado = await _service.ObterPorIdAsync(Guid.NewGuid());

            Assert.Null(resultado);
        }

        // ─── CriarAsync ────────────────────────────────────────────────────

        [Fact]
        public async Task CriarAsync_ProdutoNaoEncontrado_LancaInvalidOperationException()
        {
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

            var dto = new EstoqueCreateDTO(Guid.NewGuid(), 10, 2);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        }

        [Fact]
        public async Task CriarAsync_ProdutoJaTemEstoque_IncrementaQuantidadeExistente()
        {
            var produto = new Produto("P001", "Notebook", 2500m);
            var estoqueExistente = new Estoque(produto.Id, 20, 5);

            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produto.Id)).ReturnsAsync(estoqueExistente);

            var dto = new EstoqueCreateDTO(produto.Id, 10, 99);

            var resultado = await _service.CriarAsync(dto);

            Assert.Equal(30, resultado.Quantidade);
            Assert.Equal(5, resultado.QuantidadeMinima); // QuantidadeMinima não é alterada no upsert
            _estoqueRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Estoque>()), Times.Never);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_DadosValidos_CriaEstoqueERetornaDTO()
        {
            var produto = new Produto("P001", "Notebook", 2500m);
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produto.Id)).ReturnsAsync((Estoque?)null);
            _estoqueRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Estoque>())).Returns(Task.CompletedTask);

            var dto = new EstoqueCreateDTO(produto.Id, 50, 5);

            var resultado = await _service.CriarAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal(50, resultado.Quantidade);
            Assert.Equal(5, resultado.QuantidadeMinima);
            Assert.Equal("Notebook", resultado.ProdutoNome);
            _estoqueRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Estoque>()), Times.Once);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        // ─── AtualizarAsync ────────────────────────────────────────────────

        [Fact]
        public async Task AtualizarAsync_EstoqueNaoEncontrado_LancaInvalidOperationException()
        {
            _estoqueRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Estoque?)null);

            var dto = new EstoqueUpdateDTO(10);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AtualizarAsync(Guid.NewGuid(), dto));
        }

        [Fact]
        public async Task AtualizarAsync_DadosValidos_AtualizaERetornaDTO()
        {
            var produtoId = Guid.NewGuid();
            var estoque = new Estoque(produtoId, 50, 5);
            var produto = new Produto("P001", "Notebook", 2500m);

            _estoqueRepoMock.Setup(r => r.ObterPorIdAsync(estoque.Id)).ReturnsAsync(estoque);
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produtoId)).ReturnsAsync(produto);

            var dto = new EstoqueUpdateDTO(20);

            var resultado = await _service.AtualizarAsync(estoque.Id, dto);

            Assert.Equal(20, resultado.QuantidadeMinima);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        // ─── AdicionarQuantidadeAsync ──────────────────────────────────────

        [Fact]
        public async Task AdicionarQuantidadeAsync_EstoqueNaoEncontrado_LancaInvalidOperationException()
        {
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(It.IsAny<Guid>())).ReturnsAsync((Estoque?)null);

            var dto = new EstoqueMovimentacaoDTO(Guid.NewGuid(), 10, null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AdicionarQuantidadeAsync(dto));
        }

        [Fact]
        public async Task AdicionarQuantidadeAsync_DadosValidos_IncrementaQuantidade()
        {
            var produtoId = Guid.NewGuid();
            var estoque = new Estoque(produtoId, 50, 5);

            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produtoId)).ReturnsAsync(estoque);
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produtoId)).ReturnsAsync((Produto?)null);

            var dto = new EstoqueMovimentacaoDTO(produtoId, 30, null);

            var resultado = await _service.AdicionarQuantidadeAsync(dto);

            Assert.Equal(80, resultado.Quantidade);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        // ─── RemoverQuantidadeAsync ────────────────────────────────────────

        [Fact]
        public async Task RemoverQuantidadeAsync_EstoqueNaoEncontrado_LancaInvalidOperationException()
        {
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(It.IsAny<Guid>())).ReturnsAsync((Estoque?)null);

            var dto = new EstoqueMovimentacaoDTO(Guid.NewGuid(), 10, null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoverQuantidadeAsync(dto));
        }

        [Fact]
        public async Task RemoverQuantidadeAsync_DadosValidos_DecrementaQuantidade()
        {
            var produtoId = Guid.NewGuid();
            var estoque = new Estoque(produtoId, 50, 5);

            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produtoId)).ReturnsAsync(estoque);
            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produtoId)).ReturnsAsync((Produto?)null);

            var dto = new EstoqueMovimentacaoDTO(produtoId, 20, null);

            var resultado = await _service.RemoverQuantidadeAsync(dto);

            Assert.Equal(30, resultado.Quantidade);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoverQuantidadeAsync_QuantidadeInsuficiente_PropagaExcecaoDaDomain()
        {
            var produtoId = Guid.NewGuid();
            var estoque = new Estoque(produtoId, 5, 0);

            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produtoId)).ReturnsAsync(estoque);

            var dto = new EstoqueMovimentacaoDTO(produtoId, 100, null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoverQuantidadeAsync(dto));
        }

        // ─── TemEstoqueDisponivelAsync ─────────────────────────────────────

        [Fact]
        public async Task TemEstoqueDisponivelAsync_SemRegistroDeEstoque_RetornaFalse()
        {
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(It.IsAny<Guid>())).ReturnsAsync((Estoque?)null);

            var resultado = await _service.TemEstoqueDisponivelAsync(Guid.NewGuid(), 1);

            Assert.False(resultado);
        }

        [Fact]
        public async Task TemEstoqueDisponivelAsync_ComEstoqueSuficiente_RetornaTrue()
        {
            var produtoId = Guid.NewGuid();
            var estoque = new Estoque(produtoId, 100, 5);
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produtoId)).ReturnsAsync(estoque);

            var resultado = await _service.TemEstoqueDisponivelAsync(produtoId, 50);

            Assert.True(resultado);
        }

        [Fact]
        public async Task TemEstoqueDisponivelAsync_SemEstoqueSuficiente_RetornaFalse()
        {
            var produtoId = Guid.NewGuid();
            var estoque = new Estoque(produtoId, 10, 0);
            _estoqueRepoMock.Setup(r => r.ObterPorProdutoIdAsync(produtoId)).ReturnsAsync(estoque);

            var resultado = await _service.TemEstoqueDisponivelAsync(produtoId, 50);

            Assert.False(resultado);
        }
    }
}
