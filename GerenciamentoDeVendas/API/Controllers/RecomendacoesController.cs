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

        public RecomendacoesController(IRecomendacaoService recomendacaoService)
        {
            _recomendacaoService = recomendacaoService;
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
