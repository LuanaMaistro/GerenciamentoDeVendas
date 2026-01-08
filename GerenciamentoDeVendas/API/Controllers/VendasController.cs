using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendasController : ControllerBase
    {
        private readonly IVendaService _vendaService;

        public VendasController(IVendaService vendaService)
        {
            _vendaService = vendaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendaDTO>>> ObterTodos()
        {
            var vendas = await _vendaService.ObterTodosAsync();
            return Ok(vendas);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<VendaDTO>> ObterPorId(Guid id)
        {
            var venda = await _vendaService.ObterPorIdAsync(id);

            if (venda is null)
                return NotFound();

            return Ok(venda);
        }

        [HttpGet("cliente/{clienteId:guid}")]
        public async Task<ActionResult<IEnumerable<VendaDTO>>> ObterPorClienteId(Guid clienteId)
        {
            var vendas = await _vendaService.ObterPorClienteIdAsync(clienteId);
            return Ok(vendas);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<VendaDTO>>> ObterPorStatus(string status)
        {
            try
            {
                var vendas = await _vendaService.ObterPorStatusAsync(status);
                return Ok(vendas);
            }
            catch (ArgumentException)
            {
                return BadRequest(new { message = "Status inválido. Use: Pendente, Confirmada ou Cancelada" });
            }
        }

        [HttpGet("periodo")]
        public async Task<ActionResult<IEnumerable<VendaDTO>>> ObterPorPeriodo(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim)
        {
            var vendas = await _vendaService.ObterPorPeriodoAsync(dataInicio, dataFim);
            return Ok(vendas);
        }

        [HttpGet("total")]
        public async Task<ActionResult<decimal>> ObterTotalVendasPorPeriodo(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim)
        {
            var total = await _vendaService.ObterTotalVendasPorPeriodoAsync(dataInicio, dataFim);
            return Ok(new { total });
        }

        [HttpPost]
        public async Task<ActionResult<VendaDTO>> Criar([FromBody] VendaCreateDTO dto)
        {
            try
            {
                var venda = await _vendaService.CriarAsync(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = venda.Id }, venda);
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

        [HttpPost("{id:guid}/itens")]
        public async Task<ActionResult<VendaDTO>> AdicionarItem(Guid id, [FromBody] ItemVendaCreateDTO item)
        {
            try
            {
                var venda = await _vendaService.AdicionarItemAsync(id, item);
                return Ok(venda);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}/itens/{itemId:guid}")]
        public async Task<ActionResult<VendaDTO>> RemoverItem(Guid id, Guid itemId)
        {
            try
            {
                var venda = await _vendaService.RemoverItemAsync(id, itemId);
                return Ok(venda);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:guid}/itens/{itemId:guid}/quantidade")]
        public async Task<ActionResult<VendaDTO>> AtualizarQuantidadeItem(
            Guid id,
            Guid itemId,
            [FromQuery] int quantidade)
        {
            try
            {
                var venda = await _vendaService.AtualizarQuantidadeItemAsync(id, itemId, quantidade);
                return Ok(venda);
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

        [HttpPost("{id:guid}/confirmar")]
        public async Task<ActionResult<VendaDTO>> Confirmar(Guid id, [FromBody] VendaConfirmarDTO dto)
        {
            try
            {
                var venda = await _vendaService.ConfirmarAsync(id, dto);
                return Ok(venda);
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

        [HttpPost("{id:guid}/cancelar")]
        public async Task<ActionResult<VendaDTO>> Cancelar(Guid id)
        {
            try
            {
                var venda = await _vendaService.CancelarAsync(id);
                return Ok(venda);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
