using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RelatoriosController : ControllerBase
    {
        private readonly IRelatorioService _relatorioService;

        public RelatoriosController(IRelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        [HttpGet("vendas/resumo")]
        public async Task<ActionResult<RelatorioVendasResumoDTO>> ObterResumoVendas(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim)
        {
            if (dataFim < dataInicio)
                return BadRequest(new { message = "dataFim deve ser maior ou igual a dataInicio" });

            var resumo = await _relatorioService.ObterResumoVendasAsync(dataInicio, dataFim);
            return Ok(resumo);
        }

        [HttpGet("vendas/por-produto")]
        public async Task<ActionResult<IEnumerable<ProdutoMaisVendidoDTO>>> ObterProdutosMaisVendidos(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int top = 10)
        {
            if (dataFim < dataInicio)
                return BadRequest(new { message = "dataFim deve ser maior ou igual a dataInicio" });

            if (top <= 0 || top > 100)
                return BadRequest(new { message = "top deve estar entre 1 e 100" });

            var produtos = await _relatorioService.ObterProdutosMaisVendidosAsync(dataInicio, dataFim, top);
            return Ok(produtos);
        }

        [HttpGet("vendas/por-cliente")]
        public async Task<ActionResult<IEnumerable<ClienteCompradorDTO>>> ObterClientesQueMaisCompraram(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int top = 10)
        {
            if (dataFim < dataInicio)
                return BadRequest(new { message = "dataFim deve ser maior ou igual a dataInicio" });

            if (top <= 0 || top > 100)
                return BadRequest(new { message = "top deve estar entre 1 e 100" });

            var clientes = await _relatorioService.ObterClientesQueMoreCompraramAsync(dataInicio, dataFim, top);
            return Ok(clientes);
        }

        [HttpGet("vendas/por-categoria")]
        public async Task<ActionResult<IEnumerable<CategoriaMaisVendidaDTO>>> ObterCategoriasMaisVendidas(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] int top = 10)
        {
            if (dataFim < dataInicio)
                return BadRequest(new { message = "dataFim deve ser maior ou igual a dataInicio" });

            if (top <= 0 || top > 100)
                return BadRequest(new { message = "top deve estar entre 1 e 100" });

            var categorias = await _relatorioService.ObterCategoriasMaisVendidasAsync(dataInicio, dataFim, top);
            return Ok(categorias);
        }

        [HttpGet("estoque")]
        public async Task<ActionResult<RelatorioEstoqueDTO>> ObterRelatorioEstoque()
        {
            var relatorio = await _relatorioService.ObterRelatorioEstoqueAsync();
            return Ok(relatorio);
        }
    }
}
