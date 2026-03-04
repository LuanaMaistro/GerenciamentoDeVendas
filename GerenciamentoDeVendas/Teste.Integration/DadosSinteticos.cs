namespace Teste.Integration
{
    /// <summary>
    /// Produto sintético usado exclusivamente nos testes de integração.
    /// IDs com prefixo "ITESTE-" para não conflitar com dados reais.
    /// </summary>
    public record ProdutoTeste(string Id, string Nome, string Categoria);

    /// <summary>
    /// Cenário de teste para um cliente:
    /// - Treino: compras enviadas ao Recombee para aprendizado (índices 0-3)
    /// - Teste:  compras retidas como ground truth para avaliação (índices 4-5)
    /// </summary>
    public record CenarioTeste(string ClienteId, string Perfil, List<string> Treino, List<string> Teste);

    public static class DadosSinteticos
    {
        // -------------------------------------------------------
        // 50 produtos em 5 categorias (10 por categoria)
        // -------------------------------------------------------
        public static readonly List<ProdutoTeste> Produtos =
        [
            // Informatica
            new("ITESTE-INF-01", "Notebook Gamer",      "Informatica"),
            new("ITESTE-INF-02", "Mouse Sem Fio",        "Informatica"),
            new("ITESTE-INF-03", "Teclado Mecanico",     "Informatica"),
            new("ITESTE-INF-04", "Monitor 4K",           "Informatica"),
            new("ITESTE-INF-05", "Headset Gamer",        "Informatica"),
            new("ITESTE-INF-06", "SSD 1TB",              "Informatica"),
            new("ITESTE-INF-07", "Webcam HD",            "Informatica"),
            new("ITESTE-INF-08", "Hub USB",              "Informatica"),
            new("ITESTE-INF-09", "Mousepad XL",          "Informatica"),
            new("ITESTE-INF-10", "Placa de Video",       "Informatica"),

            // Eletronicos
            new("ITESTE-ELE-01", "Smartphone Android",  "Eletronicos"),
            new("ITESTE-ELE-02", "Tablet",               "Eletronicos"),
            new("ITESTE-ELE-03", "Smart TV 55pol",       "Eletronicos"),
            new("ITESTE-ELE-04", "Fone Bluetooth",       "Eletronicos"),
            new("ITESTE-ELE-05", "Caixa de Som BT",      "Eletronicos"),
            new("ITESTE-ELE-06", "Camera Digital",       "Eletronicos"),
            new("ITESTE-ELE-07", "Smartwatch",           "Eletronicos"),
            new("ITESTE-ELE-08", "Carregador Rapido",    "Eletronicos"),
            new("ITESTE-ELE-09", "Cabo HDMI 4K",         "Eletronicos"),
            new("ITESTE-ELE-10", "Controle Universal",   "Eletronicos"),

            // Livros
            new("ITESTE-LIV-01", "Clean Code",           "Livros"),
            new("ITESTE-LIV-02", "Design Patterns",      "Livros"),
            new("ITESTE-LIV-03", "Domain-Driven Design", "Livros"),
            new("ITESTE-LIV-04", "Algoritmos",           "Livros"),
            new("ITESTE-LIV-05", "Python Fluente",       "Livros"),
            new("ITESTE-LIV-06", "Arquitetura Limpa",    "Livros"),
            new("ITESTE-LIV-07", "Microservices",        "Livros"),
            new("ITESTE-LIV-08", "DevOps Handbook",      "Livros"),
            new("ITESTE-LIV-09", "Cloud Native",         "Livros"),
            new("ITESTE-LIV-10", "TDD by Example",       "Livros"),

            // Esportes
            new("ITESTE-ESP-01", "Tenis Running",        "Esportes"),
            new("ITESTE-ESP-02", "Camiseta Dry-Fit",     "Esportes"),
            new("ITESTE-ESP-03", "Shorts Esportivo",     "Esportes"),
            new("ITESTE-ESP-04", "Mochila Esportiva",    "Esportes"),
            new("ITESTE-ESP-05", "Garrafa Termica",      "Esportes"),
            new("ITESTE-ESP-06", "Suplemento Proteico",  "Esportes"),
            new("ITESTE-ESP-07", "Luva de Academia",     "Esportes"),
            new("ITESTE-ESP-08", "Bola de Futebol",      "Esportes"),
            new("ITESTE-ESP-09", "Toalha Esportiva",     "Esportes"),
            new("ITESTE-ESP-10", "Raquete de Tenis",     "Esportes"),

            // Casa
            new("ITESTE-CAS-01", "Cafeteira Espresso",  "Casa"),
            new("ITESTE-CAS-02", "Liquidificador",       "Casa"),
            new("ITESTE-CAS-03", "Aspirador Robo",       "Casa"),
            new("ITESTE-CAS-04", "Ventilador Torre",     "Casa"),
            new("ITESTE-CAS-05", "Luminaria LED",        "Casa"),
            new("ITESTE-CAS-06", "Tapete Persiano",      "Casa"),
            new("ITESTE-CAS-07", "Almofada Ergonomica",  "Casa"),
            new("ITESTE-CAS-08", "Escrivaninha Gamer",   "Casa"),
            new("ITESTE-CAS-09", "Cadeira Ergonomica",   "Casa"),
            new("ITESTE-CAS-10", "Prateleira Flutuante", "Casa"),
        ];

        // -------------------------------------------------------
        // 55 cenários de teste — 5 perfis × 11 clientes cada.
        // Cada linha é um array plano de 6 IDs:
        //   índices 0-3 → treino (enviados ao Recombee)
        //   índices 4-5 → teste  (ground truth para avaliação)
        // -------------------------------------------------------
        public static readonly List<CenarioTeste> Cenarios = GerarCenarios();

        private static List<CenarioTeste> GerarCenarios()
        {
            var cenarios = new List<CenarioTeste>();

            // Perfil 1 — Programador: compra Informatica + Livros
            string[][] programadores =
            [
                ["ITESTE-INF-01","ITESTE-INF-02","ITESTE-LIV-01","ITESTE-LIV-02","ITESTE-INF-03","ITESTE-LIV-03"],
                ["ITESTE-INF-02","ITESTE-INF-04","ITESTE-LIV-02","ITESTE-LIV-04","ITESTE-INF-05","ITESTE-LIV-05"],
                ["ITESTE-INF-01","ITESTE-INF-06","ITESTE-LIV-01","ITESTE-LIV-06","ITESTE-INF-07","ITESTE-LIV-07"],
                ["ITESTE-INF-03","ITESTE-INF-05","ITESTE-LIV-03","ITESTE-LIV-05","ITESTE-INF-08","ITESTE-LIV-08"],
                ["ITESTE-INF-04","ITESTE-INF-07","ITESTE-LIV-04","ITESTE-LIV-07","ITESTE-INF-09","ITESTE-LIV-09"],
                ["ITESTE-INF-02","ITESTE-INF-08","ITESTE-LIV-02","ITESTE-LIV-08","ITESTE-INF-10","ITESTE-LIV-10"],
                ["ITESTE-INF-01","ITESTE-INF-03","ITESTE-LIV-01","ITESTE-LIV-03","ITESTE-INF-04","ITESTE-LIV-04"],
                ["ITESTE-INF-05","ITESTE-INF-09","ITESTE-LIV-05","ITESTE-LIV-09","ITESTE-INF-06","ITESTE-LIV-06"],
                ["ITESTE-INF-06","ITESTE-INF-10","ITESTE-LIV-06","ITESTE-LIV-10","ITESTE-INF-01","ITESTE-LIV-01"],
                ["ITESTE-INF-07","ITESTE-INF-08","ITESTE-LIV-07","ITESTE-LIV-08","ITESTE-INF-02","ITESTE-LIV-02"],
                ["ITESTE-INF-09","ITESTE-INF-10","ITESTE-LIV-09","ITESTE-LIV-10","ITESTE-INF-03","ITESTE-LIV-03"],
            ];

            // Perfil 2 — Gamer: compra Informatica + Eletronicos
            string[][] gamers =
            [
                ["ITESTE-INF-01","ITESTE-INF-05","ITESTE-ELE-01","ITESTE-ELE-04","ITESTE-INF-10","ITESTE-ELE-05"],
                ["ITESTE-INF-02","ITESTE-INF-03","ITESTE-ELE-02","ITESTE-ELE-03","ITESTE-INF-04","ITESTE-ELE-07"],
                ["ITESTE-INF-04","ITESTE-INF-06","ITESTE-ELE-04","ITESTE-ELE-07","ITESTE-INF-09","ITESTE-ELE-01"],
                ["ITESTE-INF-05","ITESTE-INF-10","ITESTE-ELE-05","ITESTE-ELE-10","ITESTE-INF-01","ITESTE-ELE-03"],
                ["ITESTE-INF-03","ITESTE-INF-07","ITESTE-ELE-03","ITESTE-ELE-06","ITESTE-INF-06","ITESTE-ELE-09"],
                ["ITESTE-INF-08","ITESTE-INF-09","ITESTE-ELE-08","ITESTE-ELE-09","ITESTE-INF-02","ITESTE-ELE-02"],
                ["ITESTE-INF-01","ITESTE-INF-02","ITESTE-ELE-01","ITESTE-ELE-02","ITESTE-INF-05","ITESTE-ELE-06"],
                ["ITESTE-INF-06","ITESTE-INF-07","ITESTE-ELE-06","ITESTE-ELE-07","ITESTE-INF-08","ITESTE-ELE-08"],
                ["ITESTE-INF-03","ITESTE-INF-04","ITESTE-ELE-03","ITESTE-ELE-04","ITESTE-INF-07","ITESTE-ELE-05"],
                ["ITESTE-INF-09","ITESTE-INF-10","ITESTE-ELE-09","ITESTE-ELE-10","ITESTE-INF-03","ITESTE-ELE-01"],
                ["ITESTE-INF-01","ITESTE-INF-04","ITESTE-ELE-01","ITESTE-ELE-05","ITESTE-INF-08","ITESTE-ELE-10"],
            ];

            // Perfil 3 — Leitor: compra exclusivamente Livros
            string[][] leitores =
            [
                ["ITESTE-LIV-01","ITESTE-LIV-02","ITESTE-LIV-03","ITESTE-LIV-04","ITESTE-LIV-05","ITESTE-LIV-06"],
                ["ITESTE-LIV-02","ITESTE-LIV-04","ITESTE-LIV-06","ITESTE-LIV-08","ITESTE-LIV-07","ITESTE-LIV-09"],
                ["ITESTE-LIV-01","ITESTE-LIV-03","ITESTE-LIV-05","ITESTE-LIV-07","ITESTE-LIV-08","ITESTE-LIV-10"],
                ["ITESTE-LIV-03","ITESTE-LIV-06","ITESTE-LIV-07","ITESTE-LIV-09","ITESTE-LIV-01","ITESTE-LIV-02"],
                ["ITESTE-LIV-04","ITESTE-LIV-05","ITESTE-LIV-08","ITESTE-LIV-10","ITESTE-LIV-03","ITESTE-LIV-06"],
                ["ITESTE-LIV-01","ITESTE-LIV-05","ITESTE-LIV-09","ITESTE-LIV-10","ITESTE-LIV-04","ITESTE-LIV-07"],
                ["ITESTE-LIV-02","ITESTE-LIV-06","ITESTE-LIV-08","ITESTE-LIV-10","ITESTE-LIV-01","ITESTE-LIV-05"],
                ["ITESTE-LIV-03","ITESTE-LIV-07","ITESTE-LIV-08","ITESTE-LIV-09","ITESTE-LIV-02","ITESTE-LIV-04"],
                ["ITESTE-LIV-01","ITESTE-LIV-02","ITESTE-LIV-07","ITESTE-LIV-08","ITESTE-LIV-06","ITESTE-LIV-09"],
                ["ITESTE-LIV-04","ITESTE-LIV-06","ITESTE-LIV-09","ITESTE-LIV-10","ITESTE-LIV-02","ITESTE-LIV-08"],
                ["ITESTE-LIV-02","ITESTE-LIV-03","ITESTE-LIV-05","ITESTE-LIV-06","ITESTE-LIV-07","ITESTE-LIV-10"],
            ];

            // Perfil 4 — Esportista: compra exclusivamente Esportes
            string[][] esportistas =
            [
                ["ITESTE-ESP-01","ITESTE-ESP-02","ITESTE-ESP-03","ITESTE-ESP-04","ITESTE-ESP-05","ITESTE-ESP-06"],
                ["ITESTE-ESP-02","ITESTE-ESP-04","ITESTE-ESP-06","ITESTE-ESP-08","ITESTE-ESP-07","ITESTE-ESP-09"],
                ["ITESTE-ESP-01","ITESTE-ESP-03","ITESTE-ESP-05","ITESTE-ESP-07","ITESTE-ESP-08","ITESTE-ESP-10"],
                ["ITESTE-ESP-03","ITESTE-ESP-06","ITESTE-ESP-07","ITESTE-ESP-09","ITESTE-ESP-01","ITESTE-ESP-02"],
                ["ITESTE-ESP-04","ITESTE-ESP-05","ITESTE-ESP-08","ITESTE-ESP-10","ITESTE-ESP-03","ITESTE-ESP-06"],
                ["ITESTE-ESP-01","ITESTE-ESP-05","ITESTE-ESP-09","ITESTE-ESP-10","ITESTE-ESP-04","ITESTE-ESP-07"],
                ["ITESTE-ESP-02","ITESTE-ESP-06","ITESTE-ESP-08","ITESTE-ESP-10","ITESTE-ESP-01","ITESTE-ESP-05"],
                ["ITESTE-ESP-03","ITESTE-ESP-07","ITESTE-ESP-08","ITESTE-ESP-09","ITESTE-ESP-02","ITESTE-ESP-04"],
                ["ITESTE-ESP-01","ITESTE-ESP-02","ITESTE-ESP-07","ITESTE-ESP-08","ITESTE-ESP-06","ITESTE-ESP-09"],
                ["ITESTE-ESP-04","ITESTE-ESP-06","ITESTE-ESP-09","ITESTE-ESP-10","ITESTE-ESP-02","ITESTE-ESP-08"],
                ["ITESTE-ESP-02","ITESTE-ESP-03","ITESTE-ESP-05","ITESTE-ESP-06","ITESTE-ESP-07","ITESTE-ESP-10"],
            ];

            // Perfil 5 — Home Office: compra Casa + Informatica
            string[][] homeoffice =
            [
                ["ITESTE-CAS-01","ITESTE-CAS-08","ITESTE-INF-01","ITESTE-INF-07","ITESTE-CAS-09","ITESTE-INF-04"],
                ["ITESTE-CAS-02","ITESTE-CAS-09","ITESTE-INF-02","ITESTE-INF-04","ITESTE-CAS-03","ITESTE-INF-07"],
                ["ITESTE-CAS-03","ITESTE-CAS-05","ITESTE-INF-03","ITESTE-INF-06","ITESTE-CAS-08","ITESTE-INF-01"],
                ["ITESTE-CAS-04","ITESTE-CAS-07","ITESTE-INF-04","ITESTE-INF-08","ITESTE-CAS-01","ITESTE-INF-02"],
                ["ITESTE-CAS-05","ITESTE-CAS-06","ITESTE-INF-05","ITESTE-INF-09","ITESTE-CAS-04","ITESTE-INF-03"],
                ["ITESTE-CAS-06","ITESTE-CAS-08","ITESTE-INF-06","ITESTE-INF-07","ITESTE-CAS-05","ITESTE-INF-10"],
                ["ITESTE-CAS-07","ITESTE-CAS-09","ITESTE-INF-07","ITESTE-INF-10","ITESTE-CAS-06","ITESTE-INF-05"],
                ["ITESTE-CAS-01","ITESTE-CAS-03","ITESTE-INF-01","ITESTE-INF-03","ITESTE-CAS-07","ITESTE-INF-06"],
                ["ITESTE-CAS-02","ITESTE-CAS-04","ITESTE-INF-02","ITESTE-INF-05","ITESTE-CAS-10","ITESTE-INF-09"],
                ["ITESTE-CAS-08","ITESTE-CAS-10","ITESTE-INF-08","ITESTE-INF-09","ITESTE-CAS-02","ITESTE-INF-04"],
                ["ITESTE-CAS-09","ITESTE-CAS-10","ITESTE-INF-09","ITESTE-INF-10","ITESTE-CAS-01","ITESTE-INF-01"],
            ];

            var perfis = new (string nome, string[][] dados)[]
            {
                ("Programador", programadores),
                ("Gamer",       gamers),
                ("Leitor",      leitores),
                ("Esportista",  esportistas),
                ("HomeOffice",  homeoffice),
            };

            foreach (var (perfil, dados) in perfis)
            {
                for (int i = 0; i < dados.Length; i++)
                {
                    cenarios.Add(new CenarioTeste(
                        ClienteId: $"UTESTE-{perfil.ToUpper()}-{i + 1:D2}",
                        Perfil:    perfil,
                        Treino:    dados[i][0..4].ToList(),
                        Teste:     dados[i][4..].ToList()
                    ));
                }
            }

            return cenarios;
        }
    }
}
