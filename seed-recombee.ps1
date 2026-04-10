# seed-recombee.ps1
# Executa apos reset do banco Recombee.
# 1. Re-sincroniza todos os produtos ao Recombee (via PUT)
# 2. Registra visualizacoes por cliente simulando preferencias por categoria

$baseUrl = "http://localhost:5001"
$headers = @{ "Content-Type" = "application/json" }
$erros   = 0

function Invoke-Api($method, $url, $body = $null) {
    try {
        $params = @{ Method = $method; Uri = $url; Headers = $headers; ErrorAction = "Stop" }
        if ($body) { $params.Body = ($body | ConvertTo-Json) }
        return Invoke-RestMethod @params
    } catch {
        Write-Host "  [ERRO] $method $url : $($_.Exception.Message)" -ForegroundColor Red
        $script:erros++
        return $null
    }
}

Write-Host ""
Write-Host "=========================================="
Write-Host "  SEED RECOMBEE - Dados Reais do Banco"
Write-Host "=========================================="
Write-Host ""

# 1. Buscar dados
Write-Host "[1/3] Buscando produtos e clientes ativos..." -ForegroundColor Cyan

$produtos = Invoke-Api "GET" "$baseUrl/api/produtos/ativos"
$clientes = Invoke-Api "GET" "$baseUrl/api/clientes/ativos"

if (-not $produtos -or $produtos.Count -eq 0) {
    Write-Host "Nenhum produto ativo encontrado. Abortando." -ForegroundColor Red
    exit 1
}
if (-not $clientes -or $clientes.Count -eq 0) {
    Write-Host "Nenhum cliente ativo encontrado. Abortando." -ForegroundColor Red
    exit 1
}

Write-Host "  Produtos ativos : $($produtos.Count)"
Write-Host "  Clientes ativos : $($clientes.Count)"

# 2. Re-sincronizar produtos ao Recombee via PUT
Write-Host ""
Write-Host "[2/3] Re-sincronizando produtos ao Recombee..." -ForegroundColor Cyan

$i = 0
foreach ($p in $produtos) {
    $i++
    $cat = if ($p.categoria) { $p.categoria } else { "sem categoria" }
    $body = @{
        nome             = $p.nome
        descricao        = $p.descricao
        precoUnitario    = $p.precoUnitario
        categoria        = $p.categoria
        quantidadeMinima = $p.quantidadeMinima
    }
    $resultado = Invoke-Api "PUT" "$baseUrl/api/produtos/$($p.id)" $body
    $status = if ($resultado) { "OK" } else { "ERRO" }
    Write-Host "  [$i/$($produtos.Count)] $($p.nome) ($cat) - $status"
}

# 3. Registrar visualizacoes por cliente
Write-Host ""
Write-Host "[3/3] Registrando visualizacoes por cliente..." -ForegroundColor Cyan
Write-Host "  (Sequencial - evita timeout do Recombee)"
Write-Host ""

$porCategoria = $produtos | Group-Object -Property categoria
$categorias   = @($porCategoria | ForEach-Object { $_.Name } | Where-Object { $_ })

if ($categorias.Count -eq 0) {
    $categorias   = @("geral")
    $porCategoria = @([PSCustomObject]@{ Name = "geral"; Group = $produtos })
}

$totalVisualizacoes = 0
$ci = 0

foreach ($cliente in $clientes) {
    $ci++

    $idxPrincipal  = $ci % $categorias.Count
    $idxSecundaria = ($ci + 1) % $categorias.Count

    $catPrincipal  = $porCategoria | Where-Object { $_.Name -eq $categorias[$idxPrincipal] }
    $catSecundaria = $porCategoria | Where-Object { $_.Name -eq $categorias[$idxSecundaria] }

    $selecao = @()
    if ($catPrincipal) {
        $selecao += @($catPrincipal.Group | Get-Random -Count ([Math]::Min(5, @($catPrincipal.Group).Count)))
    }
    if ($catSecundaria) {
        $selecao += @($catSecundaria.Group | Get-Random -Count ([Math]::Min(2, @($catSecundaria.Group).Count)))
    }

    $selecao = @($selecao | Sort-Object id -Unique)

    $nomeCliente = if ($cliente.nome) { $cliente.nome } else { $cliente.id }
    Write-Host "  Cliente $ci/$($clientes.Count): $nomeCliente" -NoNewline

    foreach ($produto in $selecao) {
        $body = @{ clienteId = $cliente.id; produtoId = $produto.id }
        $resultado = Invoke-Api "POST" "$baseUrl/api/recomendacoes/eventos/visualizacao" $body
        if ($resultado -ne $null) { $totalVisualizacoes++ }
    }

    Write-Host " -> $($selecao.Count) visualizacoes" -ForegroundColor Green
}

# Resumo
Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "  Produtos sincronizados : $($produtos.Count)"
Write-Host "  Clientes processados   : $($clientes.Count)"
Write-Host "  Visualizacoes enviadas : $totalVisualizacoes"
Write-Host "  Erros                  : $erros"
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

if ($erros -gt 0) {
    Write-Host "Seed concluido com $erros erro(s). Verifique se a API esta rodando em $baseUrl." -ForegroundColor Yellow
} else {
    Write-Host "Seed concluido com sucesso!" -ForegroundColor Green
    Write-Host "Teste: GET $baseUrl/api/recomendacoes/cliente/{id}/diagnostico" -ForegroundColor Gray
}
Write-Host ""
