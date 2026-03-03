using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecomendacoesController : ControllerBase
    {
        private readonly IRecomendacaoService _recomendacaoService;
        private readonly IClienteService _clienteService;

        public RecomendacoesController(IRecomendacaoService recomendacaoService, IClienteService clienteService)
        {
            _recomendacaoService = recomendacaoService;
            _clienteService = clienteService;
        }

        /// <summary>
        /// Retorna produtos recomendados para um cliente com base no histórico de compras e visualizações.
        /// </summary>
        [HttpGet("cliente/{clienteId:guid}")]
        public async Task<ActionResult<RecomendacaoResultadoDTO>> ObterRecomendacoes(
            Guid clienteId,
            [FromQuery] int quantidade = 5)
        {
            try
            {
                var resultado = await _recomendacaoService.ObterRecomendacoesAsync(clienteId, quantidade);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retorna os dados do cliente e suas recomendações personalizadas.
        /// </summary>
        [HttpGet("cliente/{clienteId:guid}/completo")]
        public async Task<ActionResult<ClienteRecomendacoesDTO>> ObterMinhasRecomendacoes(
            Guid clienteId,
            [FromQuery] int quantidade = 5)
        {
            var cliente = await _clienteService.ObterPorIdAsync(clienteId);
            if (cliente is null)
                return NotFound(new { message = "Cliente não encontrado" });

            try
            {
                var resultado = await _recomendacaoService.ObterRecomendacoesAsync(clienteId, quantidade);
                return Ok(new ClienteRecomendacoesDTO(cliente, resultado.Itens));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Visão admin: retorna todos os clientes ativos com suas respectivas recomendações.
        /// </summary>
        [HttpGet("todos")]
        public async Task<ActionResult<IEnumerable<ClienteRecomendacoesDTO>>> ObterTodosComRecomendacoes(
            [FromQuery] int quantidade = 5)
        {
            var clientes = await _clienteService.ObterAtivosAsync();
            var resultado = new List<ClienteRecomendacoesDTO>();

            foreach (var cliente in clientes)
            {
                try
                {
                    var rec = await _recomendacaoService.ObterRecomendacoesAsync(cliente.Id, quantidade);
                    resultado.Add(new ClienteRecomendacoesDTO(cliente, rec.Itens));
                }
                catch
                {
                    resultado.Add(new ClienteRecomendacoesDTO(cliente, []));
                }
            }

            return Ok(resultado);
        }

        /// <summary>
        /// Registra que um cliente visualizou um produto.
        /// Alimenta o algoritmo de recomendação com dados de comportamento.
        /// </summary>
        [HttpPost("eventos/visualizacao")]
        public async Task<ActionResult> RegistrarVisualizacao([FromBody] VisualizacaoEventoDTO dto)
        {
            try
            {
                await _recomendacaoService.RegistrarVisualizacaoAsync(dto.ClienteId, dto.ProdutoId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
