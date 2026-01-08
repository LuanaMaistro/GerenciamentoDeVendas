using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> ObterTodos()
        {
            var produtos = await _produtoService.ObterTodosAsync();
            return Ok(produtos);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoDTO>> ObterPorId(Guid id)
        {
            var produto = await _produtoService.ObterPorIdAsync(id);

            if (produto is null)
                return NotFound();

            return Ok(produto);
        }

        [HttpGet("codigo/{codigo}")]
        public async Task<ActionResult<ProdutoDTO>> ObterPorCodigo(string codigo)
        {
            var produto = await _produtoService.ObterPorCodigoAsync(codigo);

            if (produto is null)
                return NotFound();

            return Ok(produto);
        }

        [HttpGet("ativos")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> ObterAtivos()
        {
            var produtos = await _produtoService.ObterAtivosAsync();
            return Ok(produtos);
        }

        [HttpGet("categoria/{categoria}")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> ObterPorCategoria(string categoria)
        {
            var produtos = await _produtoService.ObterPorCategoriaAsync(categoria);
            return Ok(produtos);
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> BuscarPorNome([FromQuery] string nome)
        {
            var produtos = await _produtoService.BuscarPorNomeAsync(nome);
            return Ok(produtos);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoDTO>> Criar([FromBody] ProdutoCreateDTO dto)
        {
            try
            {
                var produto = await _produtoService.CriarAsync(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProdutoDTO>> Atualizar(Guid id, [FromBody] ProdutoUpdateDTO dto)
        {
            try
            {
                var produto = await _produtoService.AtualizarAsync(id, dto);
                return Ok(produto);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:guid}/ativar")]
        public async Task<ActionResult> Ativar(Guid id)
        {
            try
            {
                await _produtoService.AtivarAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:guid}/inativar")]
        public async Task<ActionResult> Inativar(Guid id)
        {
            try
            {
                await _produtoService.InativarAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
