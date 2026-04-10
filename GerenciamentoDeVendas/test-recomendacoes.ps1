$base = "https://nexsell.up.railway.app/api"

$loginBody = @{ email = "admin@sistema.com"; senha = "Admin@123" } | ConvertTo-Json
$resLogin = Invoke-RestMethod -Uri "$base/auth/login" -Method POST -Body $loginBody -ContentType "application/json"
$token = $resLogin.token
Write-Host "Login OK" -ForegroundColor Green

$headers = @{ "Content-Type" = "application/json"; "Authorization" = "Bearer $token" }

function Invoke-Post($url, $body) {
    Invoke-RestMethod -Uri $url -Method POST -Headers $headers -Body ($body | ConvertTo-Json -Depth 5)
}
function Invoke-Get($url) {
    Invoke-RestMethod -Uri $url -Method GET -Headers $headers
}

Write-Host "`n=== TESTE DO MODULO DE RECOMENDACOES ===" -ForegroundColor Cyan

# -------------------------------------------------------
# 1. USAR CLIENTE EXISTENTE OU CRIAR UM
# -------------------------------------------------------
Write-Host "`n[1] Buscando cliente existente..." -ForegroundColor Yellow
$clientes = Invoke-Get "$base/clientes"
$cliente = $clientes | Where-Object { $_.ativo -eq $true } | Select-Object -First 1

if ($null -eq $cliente) {
    Write-Host "  Nenhum cliente encontrado, criando novo..." -ForegroundColor Yellow
    $cliente = Invoke-Post "$base/clientes" @{
        nome = "Ana Teste Recombee"
        documento = "123.456.789-09"
        contatoPrincipal = @{ telefone = $null; celular = "11988887777"; email = "ana@teste.com" }
        enderecoPrincipal = @{
            cep = "01310100"; logradouro = "Av. Paulista"; numero = "500"
            complemento = $null; bairro = "Bela Vista"; cidade = "Sao Paulo"; uf = "SP"
        }
    }
}

$clienteId = $cliente.id
Write-Host "  Usando cliente: $($cliente.nome) | ID: $clienteId" -ForegroundColor Green

# -------------------------------------------------------
# 2. CRIAR PRODUTOS (automaticamente sincronizados no Recombee)
# -------------------------------------------------------
Write-Host "`n[2] Criando produtos (serao sincronizados no Recombee)..." -ForegroundColor Yellow

$listaProdutos = @(
    @{ codigo = "REC-001"; nome = "Notebook Dell XPS";    preco = 4500.00; categoria = "Informatica"; descricao = "Notebook premium 15 polegadas" }
    @{ codigo = "REC-002"; nome = "Mouse Logitech MX";    preco = 350.00;  categoria = "Informatica"; descricao = "Mouse sem fio ergonomico" }
    @{ codigo = "REC-003"; nome = "Teclado Mecanico RGB"; preco = 280.00;  categoria = "Informatica"; descricao = "Teclado mecanico switch azul" }
    @{ codigo = "REC-004"; nome = "Monitor LG 27pol";     preco = 1800.00; categoria = "Informatica"; descricao = "Monitor IPS 4K" }
    @{ codigo = "REC-005"; nome = "Cadeira Gamer";        preco = 1200.00; categoria = "Mobiliario";  descricao = "Cadeira ergonomica gamer" }
)

$produtoIds = @()
foreach ($p in $listaProdutos) {
    try {
        $prod = Invoke-Post "$base/produtos" @{
            codigo = $p.codigo; nome = $p.nome
            precoUnitario = $p.preco; descricao = $p.descricao; categoria = $p.categoria
        }
        $produtoIds += $prod.id
        Write-Host "  [NOVO] $($prod.nome) | ID: $($prod.id)" -ForegroundColor Green
    } catch {
        # Produto ja existe, buscar pelo codigo
        $prod = Invoke-Get "$base/produtos/codigo/$($p.codigo)"
        $produtoIds += $prod.id
        Write-Host "  [JA EXISTE] $($prod.nome) | ID: $($prod.id)" -ForegroundColor DarkYellow
    }
}

# -------------------------------------------------------
# 3. GARANTIR ESTOQUE PARA CADA PRODUTO
# -------------------------------------------------------
Write-Host "`n[3] Verificando/criando estoque..." -ForegroundColor Yellow
foreach ($prodId in $produtoIds) {
    try {
        $est = Invoke-Post "$base/estoque" @{
            produtoId = $prodId; quantidadeInicial = 50; quantidadeMinima = 5; localizacao = "A1"
        }
        Write-Host "  Estoque criado: $($est.produtoNome) = $($est.quantidade) un." -ForegroundColor Green
    } catch {
        Write-Host "  Estoque ja existe para $prodId" -ForegroundColor DarkYellow
    }
}

# -------------------------------------------------------
# 4. REGISTRAR VISUALIZACOES (alimenta o algoritmo)
# -------------------------------------------------------
Write-Host "`n[4] Registrando visualizacoes de produtos pelo cliente..." -ForegroundColor Yellow
foreach ($prodId in $produtoIds[0..2]) {
    Invoke-Post "$base/recomendacoes/eventos/visualizacao" @{
        clienteId = $clienteId; produtoId = $prodId
    } | Out-Null
    Write-Host "  Visualizacao registrada: produto $prodId" -ForegroundColor Green
}

# -------------------------------------------------------
# 5. CRIAR VENDA E CONFIRMAR (dispara AddPurchase no Recombee)
# -------------------------------------------------------
Write-Host "`n[5] Criando venda com 2 produtos..." -ForegroundColor Yellow
$venda = Invoke-Post "$base/vendas" @{
    clienteId = $clienteId
    observacao = "Venda teste Recombee"
    itens = @(
        @{ produtoId = $produtoIds[0]; quantidade = 1 }
        @{ produtoId = $produtoIds[1]; quantidade = 2 }
    )
}
$vendaId = $venda.id
Write-Host "  Venda criada | ID: $vendaId | Total: R$ $($venda.valorTotal)" -ForegroundColor Green

Write-Host "`n[6] Confirmando venda (dispara AddPurchase no Recombee para cada item)..." -ForegroundColor Yellow
$vendaConfirmada = Invoke-Post "$base/vendas/$vendaId/confirmar" @{ formaPagamento = "Pix" }
Write-Host "  Status: $($vendaConfirmada.status) | Forma: $($vendaConfirmada.formaPagamento)" -ForegroundColor Green

# -------------------------------------------------------
# 6. AGUARDAR E BUSCAR RECOMENDACOES
# -------------------------------------------------------
Write-Host "`n[7] Aguardando processamento do Recombee (3s)..." -ForegroundColor Yellow
Start-Sleep -Seconds 3

Write-Host "`n[8] Buscando recomendacoes para o cliente..." -ForegroundColor Yellow
$recomendacoes = Invoke-Get "$base/recomendacoes/cliente/$clienteId`?quantidade=5"

Write-Host "`n  === RECOMENDACOES RECEBIDAS ===" -ForegroundColor Cyan
Write-Host "  Cliente ID: $clienteId"
Write-Host "  Total de itens recomendados: $($recomendacoes.itens.Count)"

if ($recomendacoes.itens.Count -eq 0) {
    Write-Host "  Sem recomendacoes personalizadas ainda." -ForegroundColor DarkYellow
    Write-Host "  Isso e normal: o Recombee precisa de mais interacoes para gerar sugestoes." -ForegroundColor DarkYellow
    Write-Host "  Execute este script mais vezes ou adicione mais compras/visualizacoes." -ForegroundColor DarkYellow
} else {
    foreach ($item in $recomendacoes.itens) {
        Write-Host "  -> $($item.produtoNome) | $($item.categoria) | R$ $($item.precoUnitario)" -ForegroundColor Green
    }
}

Write-Host "`n=== TESTE CONCLUIDO ===" -ForegroundColor Cyan
Write-Host "Verifique o painel: admin.recombee.com > tcc-dev > Data > Items e Interactions" -ForegroundColor Cyan
