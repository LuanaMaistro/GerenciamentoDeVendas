using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> ObterTodos()
        {
            var clientes = await _clienteService.ObterTodosAsync();
            return Ok(clientes);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ClienteDTO>> ObterPorId(Guid id)
        {
            var cliente = await _clienteService.ObterPorIdAsync(id);

            if (cliente is null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpGet("documento/{documento}")]
        public async Task<ActionResult<ClienteDTO>> ObterPorDocumento(string documento)
        {
            var cliente = await _clienteService.ObterPorDocumentoAsync(documento);

            if (cliente is null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpGet("ativos")]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> ObterAtivos()
        {
            var clientes = await _clienteService.ObterAtivosAsync();
            return Ok(clientes);
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> BuscarPorNome([FromQuery] string nome)
        {
            var clientes = await _clienteService.BuscarPorNomeAsync(nome);
            return Ok(clientes);
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDTO>> Criar([FromBody] ClienteCreateDTO dto)
        {
            try
            {
                var cliente = await _clienteService.CriarAsync(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, cliente);
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
        public async Task<ActionResult<ClienteDTO>> Atualizar(Guid id, [FromBody] ClienteUpdateDTO dto)
        {
            try
            {
                var cliente = await _clienteService.AtualizarAsync(id, dto);
                return Ok(cliente);
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
                await _clienteService.AtivarAsync(id);
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
                await _clienteService.InativarAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{id:guid}/contatos")]
        public async Task<ActionResult> AdicionarContatoSecundario(Guid id, [FromBody] ContatoDTO contato)
        {
            try
            {
                await _clienteService.AdicionarContatoSecundarioAsync(id, contato);
                return NoContent();
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

        [HttpPost("{id:guid}/enderecos")]
        public async Task<ActionResult> AdicionarEnderecoSecundario(Guid id, [FromBody] EnderecoDTO endereco)
        {
            try
            {
                await _clienteService.AdicionarEnderecoSecundarioAsync(id, endereco);
                return NoContent();
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
    }
}
