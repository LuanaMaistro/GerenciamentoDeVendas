$baseUrl = "https://nexsell.up.railway.app/api"

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "  SEED DE DADOS - PRODUCAO NEXSELL" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

# =============================================
# LOGIN
# =============================================
Write-Host "`n>>> LOGIN <<<" -ForegroundColor Magenta
$loginBody = @{ email = "admin@sistema.com"; senha = "Admin@123" } | ConvertTo-Json
try {
    $resLogin = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method POST -Body $loginBody -ContentType "application/json"
    $token = $resLogin.token
    $headers = @{ Authorization = "Bearer $token" }
    Write-Host "  Login OK!" -ForegroundColor Green
} catch {
    Write-Host "  Erro no login: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# =============================================
# ETAPA 1 - CLIENTES
# =============================================
Write-Host "`n>>> ETAPA 1: CRIANDO CLIENTES <<<" -ForegroundColor Magenta

$clientesData = @(
    @{
        nome = "Maria Silva"; documento = "39053344705"
        contatoPrincipal = @{ telefone = "1132221111"; celular = "11987651234"; email = "maria.silva@email.com" }
        contatosSecundarios = @(@{ telefone = $null; celular = "11976541234"; email = "maria.trabalho@empresa.com" })
        enderecoPrincipal = @{ cep = "01310100"; logradouro = "Avenida Paulista"; numero = "1000"; complemento = "Apto 301"; bairro = "Bela Vista"; cidade = "Sao Paulo"; uf = "SP" }
        enderecosSecundarios = @()
    },
    @{
        nome = "Joao Santos"; documento = "11144477735"
        contatoPrincipal = @{ telefone = "2133334444"; celular = "21998765432"; email = "joao.santos@email.com" }
        contatosSecundarios = @()
        enderecoPrincipal = @{ cep = "22041080"; logradouro = "Rua Barata Ribeiro"; numero = "350"; complemento = "Cobertura"; bairro = "Copacabana"; cidade = "Rio de Janeiro"; uf = "RJ" }
        enderecosSecundarios = @()
    },
    @{
        nome = "Tech Solutions Ltda"; documento = "11222333000181"
        contatoPrincipal = @{ telefone = "1140001000"; celular = "11991001000"; email = "contato@techsolutions.com.br" }
        contatosSecundarios = @(@{ telefone = "1140001001"; celular = $null; email = "financeiro@techsolutions.com.br" })
        enderecoPrincipal = @{ cep = "04547000"; logradouro = "Rua Gomes de Carvalho"; numero = "1225"; complemento = "Bloco B, Andar 15"; bairro = "Vila Olimpia"; cidade = "Sao Paulo"; uf = "SP" }
        enderecosSecundarios = @()
    },
    @{
        nome = "Ana Oliveira"; documento = "52998224725"
        contatoPrincipal = @{ telefone = "4133334444"; celular = "41991234567"; email = "ana.oliveira@email.com" }
        contatosSecundarios = @()
        enderecoPrincipal = @{ cep = "80010000"; logradouro = "Rua XV de Novembro"; numero = "800"; complemento = $null; bairro = "Centro"; cidade = "Curitiba"; uf = "PR" }
        enderecosSecundarios = @()
    },
    @{
        nome = "Carlos Mendes"; documento = "35983408003"
        contatoPrincipal = @{ telefone = "5133334444"; celular = "51991234567"; email = "carlos.mendes@email.com" }
        contatosSecundarios = @()
        enderecoPrincipal = @{ cep = "90010000"; logradouro = "Avenida Borges de Medeiros"; numero = "500"; complemento = "Sala 10"; bairro = "Centro Historico"; cidade = "Porto Alegre"; uf = "RS" }
        enderecosSecundarios = @()
    },
    @{
        nome = "Inovacao Digital SA"; documento = "33444555000166"
        contatoPrincipal = @{ telefone = "1140002000"; celular = "11992002000"; email = "contato@inovacaodigital.com.br" }
        contatosSecundarios = @()
        enderecoPrincipal = @{ cep = "04571010"; logradouro = "Avenida das Nacoes Unidas"; numero = "14000"; complemento = "Torre B, Andar 20"; bairro = "Chacara Santo Antonio"; cidade = "Sao Paulo"; uf = "SP" }
        enderecosSecundarios = @()
    },
    @{
        nome = "Fernanda Costa"; documento = "71428793860"
        contatoPrincipal = @{ telefone = "6233334444"; celular = "62991234567"; email = "fernanda.costa@email.com" }
        contatosSecundarios = @()
        enderecoPrincipal = @{ cep = "74810100"; logradouro = "Avenida T-63"; numero = "1200"; complemento = "Apto 502"; bairro = "Setor Bueno"; cidade = "Goiania"; uf = "GO" }
        enderecosSecundarios = @()
    },
    @{
        nome = "Ricardo Almeida"; documento = "84375598638"
        contatoPrincipal = @{ telefone = "8533334444"; celular = "85991234567"; email = "ricardo.almeida@email.com" }
        contatosSecundarios = @()
        enderecoPrincipal = @{ cep = "60160050"; logradouro = "Avenida Beira Mar"; numero = "3000"; complemento = "Apto 801"; bairro = "Meireles"; cidade = "Fortaleza"; uf = "CE" }
        enderecosSecundarios = @()
    }
)

$clienteIds = @()
$i = 1
foreach ($c in $clientesData) {
    Write-Host "`n1.$i Criando Cliente: $($c.nome)..." -ForegroundColor Yellow
    $body = $c | ConvertTo-Json -Depth 4
    try {
        $res = Invoke-RestMethod -Uri "$baseUrl/clientes" -Method POST -Body $body -ContentType "application/json" -Headers $headers
        Write-Host "  OK! ID: $($res.id)" -ForegroundColor Green
        $clienteIds += $res.id
    } catch {
        Write-Host "  Pulando (ja existe ou erro): $($_.Exception.Message)" -ForegroundColor DarkYellow
    }
    $i++
}

# =============================================
# ETAPA 2 - PRODUTOS
# =============================================
Write-Host "`n>>> ETAPA 2: CRIANDO PRODUTOS <<<" -ForegroundColor Magenta

$produtosData = @(
    @{ codigo = "NOT-001"; nome = "Notebook Dell Inspiron 15";       precoUnitario = 3499.90; descricao = "Notebook Dell Inspiron 15, Intel i5, 8GB RAM, 256GB SSD"; categoria = "Informatica" },
    @{ codigo = "MON-001"; nome = "Monitor LG 27 polegadas";          precoUnitario = 1299.90; descricao = "Monitor LG UltraWide 27 IPS Full HD";                     categoria = "Informatica" },
    @{ codigo = "TEC-001"; nome = "Teclado Mecanico Logitech";        precoUnitario = 449.90;  descricao = "Teclado mecanico Logitech G Pro, switches GX Blue";       categoria = "Perifericos" },
    @{ codigo = "MOU-001"; nome = "Mouse Wireless Logitech MX Master"; precoUnitario = 599.90;  descricao = "Mouse sem fio Logitech MX Master 3S";                     categoria = "Perifericos" },
    @{ codigo = "CAD-001"; nome = "Cadeira Ergonomica DT3";           precoUnitario = 1899.90; descricao = "Cadeira gamer/escritorio DT3 Rhino";                       categoria = "Moveis" },
    @{ codigo = "WEB-001"; nome = "Webcam Logitech C920";             precoUnitario = 349.90;  descricao = "Webcam Full HD 1080p com microfone";                       categoria = "Perifericos" },
    @{ codigo = "HDS-001"; nome = "Headset Sony WH-1000XM5";         precoUnitario = 1599.90; descricao = "Fone de ouvido noise cancelling Sony";                     categoria = "Perifericos" },
    @{ codigo = "SSD-001"; nome = "SSD Externo Samsung 1TB";         precoUnitario = 699.90;  descricao = "SSD portatil Samsung T7 1TB USB 3.2";                      categoria = "Armazenamento" },
    @{ codigo = "ROT-001"; nome = "Roteador TP-Link AX3000";         precoUnitario = 499.90;  descricao = "Roteador Wi-Fi 6 dual band AX3000";                        categoria = "Redes" },
    @{ codigo = "NOT-002"; nome = "Notebook Lenovo IdeaPad";         precoUnitario = 2899.90; descricao = "Notebook Lenovo IdeaPad 3, Ryzen 5, 8GB, 512GB SSD";       categoria = "Informatica" }
)

$produtoIds = @()
$i = 1
foreach ($p in $produtosData) {
    Write-Host "`n2.$i Criando Produto: $($p.nome)..." -ForegroundColor Yellow
    $body = $p | ConvertTo-Json
    try {
        $res = Invoke-RestMethod -Uri "$baseUrl/produtos" -Method POST -Body $body -ContentType "application/json" -Headers $headers
        Write-Host "  OK! ID: $($res.id) | R$ $($res.precoUnitario)" -ForegroundColor Green
        $produtoIds += $res.id
    } catch {
        Write-Host "  Erro: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
        exit 1
    }
    $i++
}

# =============================================
# ETAPA 3 - ESTOQUE
# =============================================
Write-Host "`n>>> ETAPA 3: CRIANDO ESTOQUE <<<" -ForegroundColor Magenta

$estoquesData = @(
    @{ produtoId = $produtoIds[0]; quantidadeInicial = 50;  quantidadeMinima = 10; localizacao = "Galpao A - Prateleira 1" },
    @{ produtoId = $produtoIds[1]; quantidadeInicial = 30;  quantidadeMinima = 5;  localizacao = "Galpao A - Prateleira 2" },
    @{ produtoId = $produtoIds[2]; quantidadeInicial = 100; quantidadeMinima = 20; localizacao = "Galpao B - Prateleira 1" },
    @{ produtoId = $produtoIds[3]; quantidadeInicial = 80;  quantidadeMinima = 15; localizacao = "Galpao B - Prateleira 2" },
    @{ produtoId = $produtoIds[4]; quantidadeInicial = 15;  quantidadeMinima = 3;  localizacao = "Galpao C - Area de Moveis" },
    @{ produtoId = $produtoIds[5]; quantidadeInicial = 60;  quantidadeMinima = 10; localizacao = "Galpao B - Prateleira 3" },
    @{ produtoId = $produtoIds[6]; quantidadeInicial = 25;  quantidadeMinima = 5;  localizacao = "Galpao B - Prateleira 4" },
    @{ produtoId = $produtoIds[7]; quantidadeInicial = 40;  quantidadeMinima = 8;  localizacao = "Galpao A - Prateleira 3" },
    @{ produtoId = $produtoIds[8]; quantidadeInicial = 35;  quantidadeMinima = 7;  localizacao = "Galpao A - Prateleira 4" },
    @{ produtoId = $produtoIds[9]; quantidadeInicial = 20;  quantidadeMinima = 5;  localizacao = "Galpao A - Prateleira 5" }
)

$i = 1
foreach ($e in $estoquesData) {
    Write-Host "`n3.$i Criando estoque para produto $($e.produtoId)..." -ForegroundColor Yellow
    $body = $e | ConvertTo-Json
    try {
        $res = Invoke-RestMethod -Uri "$baseUrl/estoque" -Method POST -Body $body -ContentType "application/json" -Headers $headers
        Write-Host "  OK! Qtd: $($res.quantidade) | Min: $($res.quantidadeMinima)" -ForegroundColor Green
    } catch {
        Write-Host "  Erro: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
        exit 1
    }
    $i++
}

# =============================================
# ETAPA 4 - VENDAS
# =============================================
Write-Host "`n>>> ETAPA 4: CRIANDO VENDAS <<<" -ForegroundColor Magenta

$vendasData = @(
    @{ clienteIdx = 0; obs = "Home office Maria";             pag = "CartaoCredito"; itens = @(@{pi=0;q=1}, @{pi=2;q=2}) },
    @{ clienteIdx = 1; obs = "Montagem escritorio Joao";    pag = "Boleto";         itens = @(@{pi=1;q=2}, @{pi=3;q=1}, @{pi=4;q=1}) },
    @{ clienteIdx = 2; obs = "Compra corporativa";          pag = "Transferencia";  itens = @(@{pi=0;q=5}, @{pi=1;q=5}, @{pi=2;q=5}, @{pi=3;q=5}) },
    @{ clienteIdx = 3; obs = "Equipamentos home office";    pag = "CartaoDebito";   itens = @(@{pi=9;q=1}, @{pi=5;q=1}, @{pi=6;q=1}) },
    @{ clienteIdx = 4; obs = "Upgrade escritorio";          pag = "Pix";            itens = @(@{pi=1;q=1}, @{pi=7;q=2}) },
    @{ clienteIdx = 5; obs = "Equipar filial BH";           pag = "Transferencia";  itens = @(@{pi=0;q=10},@{pi=1;q=10},@{pi=2;q=10},@{pi=3;q=10},@{pi=8;q=5}) },
    @{ clienteIdx = 6; obs = "Presente aniversario";        pag = "CartaoCredito";  itens = @(@{pi=6;q=1}, @{pi=3;q=1}) },
    @{ clienteIdx = 7; obs = "Home office Ricardo";         pag = "Pix";            itens = @(@{pi=9;q=1}, @{pi=2;q=1}, @{pi=5;q=1}) },
    @{ clienteIdx = 0; obs = "Segunda compra Maria";        pag = "CartaoCredito";  itens = @(@{pi=7;q=1}, @{pi=8;q=1}) },
    @{ clienteIdx = 1; obs = "Acessorios adicionais Joao";  pag = "Boleto";         itens = @(@{pi=5;q=2}, @{pi=6;q=1}) },
    @{ clienteIdx = 2; obs = "Renovacao equipamentos TI";   pag = "Transferencia";  itens = @(@{pi=9;q=3}, @{pi=6;q=5}, @{pi=7;q=3}) },
    @{ clienteIdx = 3; obs = "Upgrade monitor Ana";         pag = "CartaoDebito";   itens = @(@{pi=1;q=2}) },
    @{ clienteIdx = 4; obs = "Rede novo escritorio Carlos"; pag = "Pix";            itens = @(@{pi=8;q=3}, @{pi=5;q=2}) },
    @{ clienteIdx = 6; obs = "Equipar novo colaborador";    pag = "CartaoCredito";  itens = @(@{pi=0;q=1}, @{pi=2;q=1}, @{pi=3;q=1}, @{pi=5;q=1}) },
    @{ clienteIdx = 7; obs = "Compra mensal Ricardo";       pag = "Pix";            itens = @(@{pi=7;q=2}, @{pi=4;q=1}) },
    @{ clienteIdx = 0; obs = "Terceira compra Maria";       pag = "Dinheiro";       itens = @(@{pi=6;q=1}) },
    @{ clienteIdx = 1; obs = "Notebook novo Joao";          pag = "CartaoCredito";  itens = @(@{pi=9;q=1}, @{pi=3;q=1}) },
    @{ clienteIdx = 2; obs = "Expansao storage";            pag = "Transferencia";  itens = @(@{pi=7;q=10}) },
    @{ clienteIdx = 3; obs = "Periferico Ana";              pag = "Pix";            itens = @(@{pi=2;q=1}, @{pi=3;q=1}) },
    @{ clienteIdx = 4; obs = "Headset Carlos";              pag = "CartaoDebito";   itens = @(@{pi=6;q=2}) },
    @{ clienteIdx = 5; obs = "Kit escritorio inovacao";     pag = "Transferencia";  itens = @(@{pi=2;q=20},@{pi=3;q=20},@{pi=5;q=10}) },
    @{ clienteIdx = 6; obs = "Monitor extra Fernanda";      pag = "CartaoCredito";  itens = @(@{pi=1;q=1}, @{pi=8;q=1}) },
    @{ clienteIdx = 7; obs = "SSD Ricardo";                 pag = "Pix";            itens = @(@{pi=7;q=3}) },
    @{ clienteIdx = 0; obs = "Roteador home office";        pag = "Dinheiro";       itens = @(@{pi=8;q=1}) },
    @{ clienteIdx = 1; obs = "Webcam reunioes Joao";        pag = "Boleto";         itens = @(@{pi=5;q=1}) },
    @{ clienteIdx = 2; obs = "Notebooks diretoria";         pag = "Transferencia";  itens = @(@{pi=0;q=8}, @{pi=9;q=8}) },
    @{ clienteIdx = 3; obs = "Home office completo Ana";    pag = "CartaoCredito";  itens = @(@{pi=9;q=1},@{pi=1;q=1},@{pi=2;q=1},@{pi=3;q=1},@{pi=5;q=1}) },
    @{ clienteIdx = 4; obs = "Upgrade total Carlos";        pag = "Pix";            itens = @(@{pi=0;q=1},@{pi=1;q=1},@{pi=6;q=1},@{pi=7;q=2}) },
    @{ clienteIdx = 5; obs = "Roteadores filiais";          pag = "Transferencia";  itens = @(@{pi=8;q=8}) },
    @{ clienteIdx = 6; obs = "Setup streamer Fernanda";     pag = "CartaoCredito";  itens = @(@{pi=0;q=1},@{pi=5;q=1},@{pi=6;q=1},@{pi=3;q=1}) }
)

$i = 1
foreach ($v in $vendasData) {
    Write-Host "`n4.$i Criando venda para cliente idx $($v.clienteIdx)..." -ForegroundColor Yellow
    $itens = $v.itens | ForEach-Object { @{ produtoId = $produtoIds[$_.pi]; quantidade = $_.q } }
    $body = @{
        clienteId  = $clienteIds[$v.clienteIdx]
        observacao = $v.obs
        itens      = $itens
    } | ConvertTo-Json -Depth 4
    try {
        $resV = Invoke-RestMethod -Uri "$baseUrl/vendas" -Method POST -Body $body -ContentType "application/json" -Headers $headers
        Write-Host "  Venda criada! ID: $($resV.id) | Total: R$ $($resV.valorTotal)" -ForegroundColor Green

        $confirmar = @{ formaPagamento = $v.pag } | ConvertTo-Json
        $resC = Invoke-RestMethod -Uri "$baseUrl/vendas/$($resV.id)/confirmar" -Method POST -Body $confirmar -ContentType "application/json" -Headers $headers
        Write-Host "  Confirmada! Pagamento: $($resC.formaPagamento)" -ForegroundColor Green
    } catch {
        Write-Host "  Erro: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
    $i++
}

Write-Host "`n=============================================" -ForegroundColor Cyan
Write-Host "  SEED CONCLUIDO!" -ForegroundColor Cyan
Write-Host "  Clientes: $($clientesData.Count)" -ForegroundColor White
Write-Host "  Produtos:  $($produtosData.Count)" -ForegroundColor White
Write-Host "  Estoques:  $($estoquesData.Count)" -ForegroundColor White
Write-Host "  Vendas:    $($vendasData.Count) confirmadas" -ForegroundColor White
Write-Host "=============================================" -ForegroundColor Cyan
