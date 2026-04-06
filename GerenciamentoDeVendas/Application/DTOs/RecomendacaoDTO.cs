using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public record RecomendacaoItemDTO(
        Guid ProdutoId,
        string ProdutoNome,
        string? Categoria,
        decimal PrecoUnitario
    );

    public record RecomendacaoResultadoDTO(
        Guid ClienteId,
        IEnumerable<RecomendacaoItemDTO> Itens
    );

    public record ClienteRecomendacoesDTO(
        ClienteDTO Cliente,
        IEnumerable<RecomendacaoItemDTO> Recomendacoes
    );

    public record VisualizacaoEventoDTO(
        Guid ClienteId,
        Guid ProdutoId
    );
}
