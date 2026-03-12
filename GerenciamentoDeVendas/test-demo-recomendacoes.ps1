$base    = "http://localhost:5001/api"
$headers = @{ "Content-Type" = "application/json" }

function Invoke-Post($url, $body) {
    Invoke-RestMethod -Uri $url -Method POST -Headers $headers -Body ($body | ConvertTo-Json -Depth 5)
}
function Invoke-Put($url, $body) {
    Invoke-RestMethod -Uri $url -Method PUT -Headers $headers -Body ($body | ConvertTo-Json -Depth 5)
}
function Invoke-Get($url) {
    Invoke-RestMethod -Uri $url -Method GET -Headers $headers
}

Write-Host "`n=== DEMO: MODULO DE RECOMENDACOES ===" -ForegroundColor Cyan
Write-Host "Objetivo: gerar recomendacoes reais via filtragem colaborativa" -ForegroundColor Gray

# -------------------------------------------------------
# 1. BUSCAR PRODUTOS EXISTENTES E RE-SINCRONIZAR NO RECOMBEE
#    (chamar PUT dispara SincronizarProdutoAsync com as propriedades corretas)
# -------------------------------------------------------
Write-Host "`n[1/6] Re-sincronizando produtos no Recombee..." -ForegroundColor Yellow

$produtos = Invoke-Get "$base/produtos/ativos"
$produtoIds = @()

foreach ($p in $produtos) {
    try {
        Invoke-Put "$base/produtos/$($p.id)" @{
            nome          = $p.nome
            descricao     = $p.descricao
            precoUnitario = $p.precoUnitario
            categoria     = $p.categoria
        } | Out-Null
        $produtoIds += $p.id
        Write-Host "  Sincronizado: $($p.nome) [$($p.categoria)]" -ForegroundColor Green
    } catch {
        Write-Host "  Erro em $($p.nome): $_" -ForegroundColor Red
    }
}

if ($produtoIds.Count -lt 3) {
    Write-Host "  Poucos produtos ativos. Verifique a API." -ForegroundColor Red
    exit 1
}

# Pega os 4 primeiros produtos para o cenario de demo
$pA = $produtoIds[0]   # produto "base" — comprado por todos
$pB = $produtoIds[1]   # produto "base" — comprado por todos
$pC = $produtoIds[2]   # produto "sugerido" — so os similares compraram
$pD = if ($produtoIds.Count -ge 4) { $produtoIds[3] } else { $produtoIds[2] }

Write-Host "  Produtos para o cenario:" -ForegroundColor Cyan
Write-Host "    Base A : $pA"
Write-Host "    Base B : $pB"
Write-Host "    Sugest C: $pC"
Write-Host "    Sugest D: $pD"

# -------------------------------------------------------
# 2. BUSCAR / CRIAR CLIENTES PARA O CENARIO
#    Cliente principal (o que vai receber recomendacoes) + 4 clientes similares
# -------------------------------------------------------
Write-Host "`n[2/6] Preparando clientes para o cenario..." -ForegroundColor Yellow

$todosClientes = Invoke-Get "$base/clientes"
$ativos = $todosClientes | Where-Object { $_.ativo -eq $true }

# Cliente principal — usa o primeiro ativo existente
$clientePrincipal = $ativos | Select-Object -First 1
Write-Host "  Cliente principal : $($clientePrincipal.nome) | $($clientePrincipal.id)" -ForegroundColor Green

# Clientes similares — usa os proximos 4 ativos, criando se necessario
$clientesSimilares = @()
$candidatos = $ativos | Select-Object -Skip 1

$nomesSimilares = @("Carlos Demo", "Mariana Demo", "Pedro Demo", "Lucia Demo")
$docs  = @("111.222.333-01", "111.222.333-02", "111.222.333-03", "111.222.333-04")

for ($i = 0; $i -lt 4; $i++) {
    if ($i -lt $candidatos.Count) {
        $clientesSimilares += $candidatos[$i]
        Write-Host "  Cliente similar $($i+1) : $($candidatos[$i].nome) | $($candidatos[$i].id)" -ForegroundColor Green
    } else {
        try {
            $novo = Invoke-Post "$base/clientes" @{
                nome             = $nomesSimilares[$i]
                documento        = $docs[$i]
                contatoPrincipal = @{ telefone = $null; celular = "11900000000"; email = "demo$i@demo.com" }
                enderecoPrincipal = @{
                    cep = "01310100"; logradouro = "Av. Paulista"; numero = "1"
                    complemento = $null; bairro = "Bela Vista"; cidade = "Sao Paulo"; uf = "SP"
                }
            }
            $clientesSimilares += $novo
            Write-Host "  Cliente similar $($i+1) criado: $($novo.nome) | $($novo.id)" -ForegroundColor Green
        } catch {
            Write-Host "  Nao foi possivel criar cliente similar $($i+1): $_" -ForegroundColor Red
        }
    }
}

# -------------------------------------------------------
# 3. GARANTIR ESTOQUE
# -------------------------------------------------------
Write-Host "`n[3/6] Garantindo estoque..." -ForegroundColor Yellow
foreach ($prodId in @($pA, $pB, $pC, $pD) | Select-Object -Unique) {
    try {
        Invoke-Post "$base/estoque" @{
            produtoId = $prodId; quantidadeInicial = 500; quantidadeMinima = 5; localizacao = "A1"
        } | Out-Null
        Write-Host "  Estoque criado para $prodId" -ForegroundColor Green
    } catch {
        # Ja existe — tenta repor via movimentacao de entrada
        try {
            Invoke-Post "$base/estoque/$prodId/entrada" @{ quantidade = 200; observacao = "Reposicao demo" } | Out-Null
            Write-Host "  Estoque reposto para $prodId" -ForegroundColor Green
        } catch {
            Write-Host "  Estoque ja existe para $prodId" -ForegroundColor DarkYellow
        }
    }
}

# -------------------------------------------------------
# 4. CLIENTES SIMILARES: compram pA + pB + pC + pD
#    (historico completo — sao a base do algoritmo colaborativo)
# -------------------------------------------------------
Write-Host "`n[4/6] Registrando compras dos clientes similares (treino do algoritmo)..." -ForegroundColor Yellow

foreach ($cs in $clientesSimilares) {
    try {
        $venda = Invoke-Post "$base/vendas" @{
            clienteId  = $cs.id
            observacao = "Venda demo CF"
            itens      = @(
                @{ produtoId = $pA; quantidade = 1 }
                @{ produtoId = $pB; quantidade = 1 }
                @{ produtoId = $pC; quantidade = 1 }
                @{ produtoId = $pD; quantidade = 1 }
            )
        }
        Invoke-Post "$base/vendas/$($venda.id)/confirmar" @{ formaPagamento = "Pix" } | Out-Null
        Write-Host "  $($cs.nome): venda confirmada (pA + pB + pC + pD)" -ForegroundColor Green
    } catch {
        Write-Host "  Erro para $($cs.nome): $_" -ForegroundColor Red
    }
}

# -------------------------------------------------------
# 5. CLIENTE PRINCIPAL: compra apenas pA + pB
#    (o algoritmo deve sugerir pC e pD com base nos similares)
# -------------------------------------------------------
Write-Host "`n[5/6] Registrando compras do cliente principal (apenas pA + pB)..." -ForegroundColor Yellow

$clienteId = $clientePrincipal.id

# Registra visualizacoes apenas dos produtos que o cliente principal "viu/comprou"
# (NAO registrar pC e pD para que o Recombee possa recomenda-los)
foreach ($prodId in @($pA, $pB)) {
    try {
        Invoke-Post "$base/recomendacoes/eventos/visualizacao" @{
            clienteId = $clienteId; produtoId = $prodId
        } | Out-Null
    } catch {}
}
Write-Host "  Visualizacoes registradas (apenas pA e pB)" -ForegroundColor Green

# Cria e confirma venda so com pA e pB
try {
    $venda = Invoke-Post "$base/vendas" @{
        clienteId  = $clienteId
        observacao = "Venda demo cliente principal"
        itens      = @(
            @{ produtoId = $pA; quantidade = 1 }
            @{ produtoId = $pB; quantidade = 1 }
        )
    }
    Invoke-Post "$base/vendas/$($venda.id)/confirmar" @{ formaPagamento = "Credito" } | Out-Null
    Write-Host "  Venda confirmada para $($clientePrincipal.nome) (pA + pB)" -ForegroundColor Green
} catch {
    Write-Host "  Erro na venda do cliente principal: $_" -ForegroundColor Red
}

# -------------------------------------------------------
# 6. BUSCAR RECOMENDACOES
# -------------------------------------------------------
Write-Host "`n[6/6] Aguardando Recombee (3s) e buscando recomendacoes..." -ForegroundColor Yellow
Start-Sleep -Seconds 3

$rec = Invoke-Get "$base/recomendacoes/cliente/$clienteId/completo?quantidade=5"

Write-Host "`n┌──────────────────────────────────────────────┐" -ForegroundColor Cyan
Write-Host "│         RESULTADO DAS RECOMENDACOES         │" -ForegroundColor Cyan
Write-Host "└──────────────────────────────────────────────┘" -ForegroundColor Cyan
Write-Host "  Cliente : $($rec.cliente.nome)"
Write-Host "  ID      : $($rec.cliente.id)"
Write-Host ""

if ($rec.recomendacoes.Count -eq 0) {
    Write-Host "  Sem recomendacoes retornadas." -ForegroundColor DarkYellow
    Write-Host "  Dica: execute o script mais uma vez para acumular mais historico." -ForegroundColor DarkYellow
} else {
    Write-Host "  Produtos recomendados:" -ForegroundColor Green
    $i = 1
    foreach ($item in $rec.recomendacoes) {
        Write-Host "  $i. $($item.produtoNome) | $($item.categoria) | R$ $($item.precoUnitario)" -ForegroundColor Green
        $i++
    }
}

Write-Host ""
Write-Host "=== DEMO CONCLUIDO ===" -ForegroundColor Cyan
Write-Host "Para o print do Swagger, use o endpoint:" -ForegroundColor Gray
Write-Host "  GET /api/recomendacoes/cliente/$clienteId/completo" -ForegroundColor White
