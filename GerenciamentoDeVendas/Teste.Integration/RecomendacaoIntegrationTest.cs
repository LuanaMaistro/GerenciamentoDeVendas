using Recombee.ApiClient;
using Recombee.ApiClient.ApiRequests;
using Recombee.ApiClient.Util;
using Xunit.Abstractions;

namespace Teste.Integration
{
    /// <summary>
    /// Testes de integração com o serviço Recombee.
    ///
    /// Fluxo:
    ///   1. Registra 50 produtos sintéticos no Recombee
    ///   2. Envia 220 compras de treino (4 × 55 clientes)
    ///   3. Solicita recomendações top-5 para cada cliente
    ///   4. Calcula Precision@5 e Recall@5 em 55 cenários
    ///   5. Valida que as métricas estão acima do mínimo aceitável
    ///
    /// Os IDs usados têm prefixo "ITESTE-" / "UTESTE-" para não
    /// conflitar com dados reais do banco tcc-dev.
    /// </summary>
    public class RecomendacaoIntegrationTest
    {
        // ─── Credenciais Recombee ──────────────────────────────────────────
        private const string DatabaseId   = "tcc-dev";
        private const string PrivateToken = "o502NoL5fRGjrXowoldpbNCyOTki8MSSB2inyrBSQ5EcIIhsRNWCj3jbbaSgdmvw";
        private const int    K            = 5;

        private readonly RecombeeClient    _client;
        private readonly ITestOutputHelper _output;

        public RecomendacaoIntegrationTest(ITestOutputHelper output)
        {
            _output = output;
            // Construtor v6.1.0: (databaseId, secretToken, useHttpsAsDefault, baseUri, port, region)
            _client = new RecombeeClient(DatabaseId, PrivateToken, region: Region.UsWest);
        }

        // ══════════════════════════════════════════════════════════════════
        // TESTE PRINCIPAL
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Executa os 55 cenários de recomendação e valida Precision@5 e Recall@5.
        /// Timeout: 5 minutos (inclui latência de rede ao Recombee + envio sequencial).
        /// </summary>
        [Fact(Timeout = 300_000)]
        public async Task Recombee_55Cenarios_PrecisionERecallAcimaDoMinimo()
        {
            _output.WriteLine("╔══════════════════════════════════════════════════════╗");
            _output.WriteLine("║  INTEGRAÇÃO RECOMBEE — Avaliação de Recomendações   ║");
            _output.WriteLine("╚══════════════════════════════════════════════════════╝");
            _output.WriteLine($"Produtos: {DadosSinteticos.Produtos.Count} | " +
                              $"Cenários: {DadosSinteticos.Cenarios.Count} | K={K}");
            _output.WriteLine("");

            // ─── Fase 0: Garantir schema de propriedades ─────────────────────
            await GarantirSchemaAsync();

            // ─── Fase 1: Registrar produtos ─────────────────────────────────
            await RegistrarProdutosAsync();

            // ─── Fase 2: Enviar interações de treino ─────────────────────────
            await EnviarComprasTreinoAsync();

            // ─── Fase 3: Aguardar processamento ──────────────────────────────
            _output.WriteLine("[3/4] Aguardando processamento do Recombee (3 s)...");
            await Task.Delay(3_000);

            // ─── Fase 4: Avaliar recomendações ───────────────────────────────
            var resultados = await AvaliarRecomendacoesAsync();

            // ─── Resultado final ─────────────────────────────────────────────
            ExibirRelatorio(resultados);

            var mediaPrecision = MetricasRecomendacao.Media(resultados.Select(r => r.Precision));
            var mediaRecall    = MetricasRecomendacao.Media(resultados.Select(r => r.Recall));

            Assert.True(mediaPrecision >= 0.05,
                $"Precision@{K} média ({mediaPrecision:P1}) abaixo do mínimo de 5 %.");

            Assert.True(mediaRecall >= 0.10,
                $"Recall@{K} médio ({mediaRecall:P1}) abaixo do mínimo de 10 %.");
        }

        // ══════════════════════════════════════════════════════════════════
        // AUXILIARES PRIVADOS
        // ══════════════════════════════════════════════════════════════════

        private async Task GarantirSchemaAsync()
        {
            _output.WriteLine("[0/4] Garantindo schema de propriedades no Recombee...");

            // Cria as propriedades de item caso ainda não existam.
            // AddItemProperty retorna erro se a propriedade já existe — basta ignorar.
            var propriedades = new[] { ("nome", "string"), ("categoria", "string"), ("preco", "double") };
            foreach (var (nome, tipo) in propriedades)
            {
                try   { await _client.SendAsync(new AddItemProperty(nome, tipo)); }
                catch { /* propriedade já existe — ok */ }
            }

            _output.WriteLine("     OK  Schema verificado (nome, categoria, preco).");
        }

        private async Task RegistrarProdutosAsync()
        {
            _output.WriteLine("[1/4] Registrando produtos no Recombee (sequencial)...");

            foreach (var p in DadosSinteticos.Produtos)
            {
                await _client.SendAsync(new SetItemValues(
                    p.Id,
                    new Dictionary<string, object>
                    {
                        ["nome"]      = p.Nome,
                        ["categoria"] = p.Categoria,
                        ["preco"]     = 100.0,
                    },
                    cascadeCreate: true));
            }

            _output.WriteLine($"     OK  {DadosSinteticos.Produtos.Count} produtos registrados.");
        }

        private async Task EnviarComprasTreinoAsync()
        {
            _output.WriteLine("[2/4] Enviando interações de treino (sequencial)...");

            int total = 0;
            foreach (var cenario in DadosSinteticos.Cenarios)
            {
                foreach (var produtoId in cenario.Treino)
                {
                    await _client.SendAsync(
                        new AddPurchase(cenario.ClienteId, produtoId, cascadeCreate: true));
                    total++;
                }
            }

            _output.WriteLine($"     OK  {total} interações " +
                              $"({DadosSinteticos.Cenarios.Count} clientes × " +
                              $"{DadosSinteticos.Cenarios[0].Treino.Count} compras).");
        }

        private async Task<List<ResultadoCenario>> AvaliarRecomendacoesAsync()
        {
            _output.WriteLine("[4/4] Obtendo e avaliando recomendações...");

            var resultados = new List<ResultadoCenario>();

            foreach (var cenario in DadosSinteticos.Cenarios)
            {
                try
                {
                    var rec = await _client.SendAsync(
                        new RecommendItemsToUser(cenario.ClienteId, K));

                    var recomendados = rec.Recomms.Select(r => r.Id).ToList();

                    resultados.Add(new ResultadoCenario(
                        cenario.ClienteId,
                        cenario.Perfil,
                        recomendados,
                        cenario.Teste,
                        MetricasRecomendacao.PrecisionAtK(recomendados, cenario.Teste, K),
                        MetricasRecomendacao.RecallAtK(recomendados, cenario.Teste, K)));
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"     [ERRO] {cenario.ClienteId}: {ex.Message}");
                    resultados.Add(new ResultadoCenario(
                        cenario.ClienteId, cenario.Perfil, [], cenario.Teste, 0.0, 0.0));
                }
            }

            _output.WriteLine($"     OK  {resultados.Count} cenários avaliados.");
            return resultados;
        }

        // ══════════════════════════════════════════════════════════════════
        // RELATÓRIO DE SAÍDA
        // ══════════════════════════════════════════════════════════════════

        private void ExibirRelatorio(List<ResultadoCenario> resultados)
        {
            _output.WriteLine("");
            _output.WriteLine("┌─────────────────────────────────────────────────┐");
            _output.WriteLine("│             MÉTRICAS POR PERFIL                 │");
            _output.WriteLine("├───────────────────┬──────┬─────────────┬────────┤");
            _output.WriteLine("│ Perfil            │  N   │ Precision@5 │Recall@5│");
            _output.WriteLine("├───────────────────┼──────┼─────────────┼────────┤");

            foreach (var grupo in resultados.GroupBy(r => r.Perfil).OrderBy(g => g.Key))
            {
                var p = MetricasRecomendacao.Media(grupo.Select(r => r.Precision));
                var r = MetricasRecomendacao.Media(grupo.Select(r => r.Recall));
                _output.WriteLine($"│ {grupo.Key,-17} │ {grupo.Count(),4} │ {p,10:P1} │{r,7:P1} │");
            }

            _output.WriteLine("├───────────────────┼──────┼─────────────┼────────┤");

            var totalP = MetricasRecomendacao.Media(resultados.Select(r => r.Precision));
            var totalR = MetricasRecomendacao.Media(resultados.Select(r => r.Recall));
            _output.WriteLine($"│ {"TOTAL",-17} │ {resultados.Count,4} │ {totalP,10:P1} │{totalR,7:P1} │");
            _output.WriteLine("└───────────────────┴──────┴─────────────┴────────┘");
            _output.WriteLine("");

            // Detalhamento por cenário
            _output.WriteLine("Detalhamento por cenário:");
            foreach (var r in resultados)
            {
                var hit = r.Precision > 0 || r.Recall > 0 ? "OK" : "--";
                _output.WriteLine(
                    $"  [{hit}] {r.ClienteId,-32} " +
                    $"P@5={r.Precision:P0}  R@5={r.Recall:P0}");
                _output.WriteLine(
                    $"        Recomendados : {(r.Recomendados.Count > 0 ? string.Join(", ", r.Recomendados) : "(vazio)")}");
                _output.WriteLine(
                    $"        Relevantes   : {string.Join(", ", r.Relevantes)}");
            }
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // MODELO DE RESULTADO
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>Resultado de um único cenário de avaliação.</summary>
    public record ResultadoCenario(
        string       ClienteId,
        string       Perfil,
        List<string> Recomendados,
        List<string> Relevantes,
        double       Precision,
        double       Recall
    );
}
