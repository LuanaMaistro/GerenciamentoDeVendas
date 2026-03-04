namespace Teste.Integration
{
    /// <summary>
    /// Cálculo de métricas de avaliação de recomendação.
    /// </summary>
    public static class MetricasRecomendacao
    {
        /// <summary>
        /// Precision@K = |recomendados ∩ relevantes| / K
        /// Mede quantos dos K itens recomendados são realmente relevantes.
        /// </summary>
        public static double PrecisionAtK(List<string> recomendados, List<string> relevantes, int k)
        {
            if (k <= 0) return 0.0;

            var topK = recomendados.Take(k).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var rel  = relevantes.ToHashSet(StringComparer.OrdinalIgnoreCase);

            return (double)topK.Intersect(rel).Count() / k;
        }

        /// <summary>
        /// Recall@K = |recomendados ∩ relevantes| / |relevantes|
        /// Mede quantos dos itens relevantes foram capturados nos K primeiros resultados.
        /// </summary>
        public static double RecallAtK(List<string> recomendados, List<string> relevantes, int k)
        {
            if (relevantes.Count == 0) return 0.0;

            var topK = recomendados.Take(k).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var rel  = relevantes.ToHashSet(StringComparer.OrdinalIgnoreCase);

            return (double)topK.Intersect(rel).Count() / rel.Count;
        }

        /// <summary>Média de uma sequência de valores.</summary>
        public static double Media(IEnumerable<double> valores)
        {
            var lista = valores.ToList();
            return lista.Count == 0 ? 0.0 : lista.Average();
        }
    }
}
