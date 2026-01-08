using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController : ControllerBase
    {
        private readonly IEstoqueService _estoqueService;

        public EstoqueController(IEstoqueService estoqueService)
        {
            _estoqueService = estoqueService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EstoqueDTO>>> ObterTodos()
        {
            var estoques = await _estoqueService.ObterTodosAsync();
            return Ok(estoques);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EstoqueDTO>> ObterPorId(Guid id)
        {
            var estoque = await _estoqueService.ObterPorIdAsync(id);

            if (estoque is null)
                return NotFound();

            return Ok(estoque);
        }

        [HttpGet("produto/{produtoId:guid}")]
        public async Task<ActionResult<EstoqueDTO>> ObterPorProdutoId(Guid produtoId)
        {
            var estoque = await _estoqueService.ObterPorProdutoIdAsync(produtoId);

            if (estoque is null)
                return NotFound();

            return Ok(estoque);
        }

        [HttpGet("baixo")]
        public async Task<ActionResult<IEnumerable<EstoqueDTO>>> ObterEstoqueBaixo()
        {
            var estoques = await _estoqueService.ObterEstoqueBaixoAsync();
            return Ok(estoques);
        }

        [HttpGet("localizacao/{localizacao}")]
        public async Task<ActionResult<IEnumerable<EstoqueDTO>>> ObterPorLocalizacao(string localizacao)
        {
            var estoques = await _estoqueService.ObterPorLocalizacaoAsync(localizacao);
            return Ok(estoques);
        }

        [HttpPost]
        public async Task<ActionResult<EstoqueDTO>> Criar([FromBody] EstoqueCreateDTO dto)
        {
            try
            {
                var estoque = await _estoqueService.CriarAsync(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = estoque.Id }, estoque);
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
        public async Task<ActionResult<EstoqueDTO>> Atualizar(Guid id, [FromBody] EstoqueUpdateDTO dto)
        {
            try
            {
                var estoque = await _estoqueService.AtualizarAsync(id, dto);
                return Ok(estoque);
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

        [HttpPost("entrada")]
        public async Task<ActionResult<EstoqueDTO>> AdicionarQuantidade([FromBody] EstoqueMovimentacaoDTO dto)
        {
            try
            {
                var estoque = await _estoqueService.AdicionarQuantidadeAsync(dto);
                return Ok(estoque);
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

        [HttpPost("saida")]
        public async Task<ActionResult<EstoqueDTO>> RemoverQuantidade([FromBody] EstoqueMovimentacaoDTO dto)
        {
            try
            {
                var estoque = await _estoqueService.RemoverQuantidadeAsync(dto);
                return Ok(estoque);
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

        [HttpGet("disponivel/{produtoId:guid}/{quantidade:int}")]
        public async Task<ActionResult<bool>> VerificarDisponibilidade(Guid produtoId, int quantidade)
        {
            var disponivel = await _estoqueService.TemEstoqueDisponivelAsync(produtoId, quantidade);
            return Ok(new { disponivel });
        }
    }
}
