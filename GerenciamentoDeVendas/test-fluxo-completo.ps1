$baseUrl = "http://localhost:5001/api"

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "  TESTE DE FLUXO COMPLETO - SEED DE DADOS" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

# =============================================
# ETAPA 1 - CLIENTES
# =============================================
Write-Host "`n>>> ETAPA 1: CRIANDO CLIENTES <<<" -ForegroundColor Magenta

# Cliente 1 - Pessoa Fisica
Write-Host "`n1.1 Criando Cliente 1 (PF - Maria Silva)..." -ForegroundColor Yellow
$cliente1 = @{
    nome = "Maria Silva"
    documento = "39053344705"
    contatoPrincipal = @{
        telefone = "1132221111"
        celular = "11987651234"
        email = "maria.silva@email.com"
    }
    contatosSecundarios = @(
        @{
            telefone = $null
            celular = "11976541234"
            email = "maria.trabalho@empresa.com"
        },
        @{
            telefone = "1144443333"
            celular = $null
            email = $null
        }
    )
    enderecoPrincipal = @{
        cep = "01310100"
        logradouro = "Avenida Paulista"
        numero = "1000"
        complemento = "Apto 301"
        bairro = "Bela Vista"
        cidade = "Sao Paulo"
        uf = "SP"
    }
    enderecosSecundarios = @(
        @{
            cep = "04543011"
            logradouro = "Rua Funchal"
            numero = "500"
            complemento = "Sala 12"
            bairro = "Vila Olimpia"
            cidade = "Sao Paulo"
            uf = "SP"
        }
    )
} | ConvertTo-Json -Depth 3

try {
    $resCliente1 = Invoke-RestMethod -Uri "$baseUrl/clientes" -Method POST -Body $cliente1 -ContentType "application/json"
    Write-Host "  Cliente 1 criado! ID: $($resCliente1.id)" -ForegroundColor Green
    $clienteId1 = $resCliente1.id
} catch {
    Write-Host "  Erro ao criar cliente 1: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
    exit 1
}

# Cliente 2 - Pessoa Fisica
Write-Host "`n1.2 Criando Cliente 2 (PF - Joao Santos)..." -ForegroundColor Yellow
$cliente2 = @{
    nome = "Joao Santos"
    documento = "11144477735"
    contatoPrincipal = @{
        telefone = "2133334444"
        celular = "21998765432"
        email = "joao.santos@email.com"
    }
    contatosSecundarios = @(
        @{
            telefone = $null
            celular = "21987654321"
            email = "joao.pessoal@gmail.com"
        }
    )
    enderecoPrincipal = @{
        cep = "22041080"
        logradouro = "Rua Barata Ribeiro"
        numero = "350"
        complemento = "Cobertura"
        bairro = "Copacabana"
        cidade = "Rio de Janeiro"
        uf = "RJ"
    }
    enderecosSecundarios = @(
        @{
            cep = "20040020"
            logradouro = "Rua do Ouvidor"
            numero = "100"
            complemento = "Sala 501"
            bairro = "Centro"
            cidade = "Rio de Janeiro"
            uf = "RJ"
        },
        @{
            cep = "30130000"
            logradouro = "Praca Sete de Setembro"
            numero = "200"
            complemento = $null
            bairro = "Centro"
            cidade = "Belo Horizonte"
            uf = "MG"
        }
    )
} | ConvertTo-Json -Depth 3

try {
    $resCliente2 = Invoke-RestMethod -Uri "$baseUrl/clientes" -Method POST -Body $cliente2 -ContentType "application/json"
    Write-Host "  Cliente 2 criado! ID: $($resCliente2.id)" -ForegroundColor Green
    $clienteId2 = $resCliente2.id
} catch {
    Write-Host "  Erro ao criar cliente 2: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
    exit 1
}

# Cliente 3 - Pessoa Juridica
Write-Host "`n1.3 Criando Cliente 3 (PJ - Tech Solutions)..." -ForegroundColor Yellow
$cliente3 = @{
    nome = "Tech Solutions Ltda"
    documento = "11222333000181"
    contatoPrincipal = @{
        telefone = "1140001000"
        celular = "11991001000"
        email = "contato@techsolutions.com.br"
    }
    contatosSecundarios = @(
        @{
            telefone = "1140001001"
            celular = $null
            email = "financeiro@techsolutions.com.br"
        },
        @{
            telefone = $null
            celular = "11992002000"
            email = "compras@techsolutions.com.br"
        }
    )
    enderecoPrincipal = @{
        cep = "04547000"
        logradouro = "Rua Gomes de Carvalho"
        numero = "1225"
        complemento = "Bloco B, Andar 15"
        bairro = "Vila Olimpia"
        cidade = "Sao Paulo"
        uf = "SP"
    }
    enderecosSecundarios = @(
        @{
            cep = "13015100"
            logradouro = "Rua Barao de Jaguara"
            numero = "800"
            complemento = "Sala 303"
            bairro = "Centro"
            cidade = "Campinas"
            uf = "SP"
        }
    )
} | ConvertTo-Json -Depth 3

try {
    $resCliente3 = Invoke-RestMethod -Uri "$baseUrl/clientes" -Method POST -Body $cliente3 -ContentType "application/json"
    Write-Host "  Cliente 3 criado! ID: $($resCliente3.id)" -ForegroundColor Green
    $clienteId3 = $resCliente3.id
} catch {
    Write-Host "  Erro ao criar cliente 3: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "`n  Resumo Clientes:" -ForegroundColor Cyan
Write-Host "    Cliente 1: $clienteId1 (Maria Silva - PF)"
Write-Host "    Cliente 2: $clienteId2 (Joao Santos - PF)"
Write-Host "    Cliente 3: $clienteId3 (Tech Solutions - PJ)"

# =============================================
# ETAPA 2 - PRODUTOS
# =============================================
Write-Host "`n>>> ETAPA 2: CRIANDO PRODUTOS <<<" -ForegroundColor Magenta

$produtosData = @(
    @{ codigo = "NOT-001"; nome = "Notebook Dell Inspiron 15"; precoUnitario = 3499.90; descricao = "Notebook Dell Inspiron 15, Intel i5, 8GB RAM, 256GB SSD"; categoria = "Informatica" },
    @{ codigo = "MON-001"; nome = "Monitor LG 27 polegadas"; precoUnitario = 1299.90; descricao = "Monitor LG UltraWide 27 IPS Full HD"; categoria = "Informatica" },
    @{ codigo = "TEC-001"; nome = "Teclado Mecanico Logitech"; precoUnitario = 449.90; descricao = "Teclado mecanico Logitech G Pro, switches GX Blue"; categoria = "Perifericos" },
    @{ codigo = "MOU-001"; nome = "Mouse Wireless Logitech MX Master"; precoUnitario = 599.90; descricao = "Mouse sem fio Logitech MX Master 3S"; categoria = "Perifericos" },
    @{ codigo = "CAD-001"; nome = "Cadeira Ergonomica DT3"; precoUnitario = 1899.90; descricao = "Cadeira gamer/escritorio DT3 Rhino"; categoria = "Moveis" }
)

$produtoIds = @()

foreach ($p in $produtosData) {
    Write-Host "`n2.$($produtosData.IndexOf($p) + 1) Criando Produto: $($p.nome)..." -ForegroundColor Yellow
    $produtoBody = $p | ConvertTo-Json
    try {
        $resProduto = Invoke-RestMethod -Uri "$baseUrl/produtos" -Method POST -Body $produtoBody -ContentType "application/json"
        Write-Host "  Produto criado! ID: $($resProduto.id) | Codigo: $($resProduto.codigo) | Preco: R$ $($resProduto.precoUnitario)" -ForegroundColor Green
        $produtoIds += $resProduto.id
    } catch {
        Write-Host "  Erro ao criar produto: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
        exit 1
    }
}

Write-Host "`n  Resumo Produtos:" -ForegroundColor Cyan
for ($i = 0; $i -lt $produtosData.Count; $i++) {
    Write-Host "    $($produtosData[$i].nome): $($produtoIds[$i])"
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
    @{ produtoId = $produtoIds[4]; quantidadeInicial = 15;  quantidadeMinima = 3;  localizacao = "Galpao C - Area de Moveis" }
)

$estoqueIds = @()

foreach ($e in $estoquesData) {
    $idx = $estoquesData.IndexOf($e)
    Write-Host "`n3.$($idx + 1) Criando Estoque para: $($produtosData[$idx].nome)..." -ForegroundColor Yellow
    $estoqueBody = $e | ConvertTo-Json
    try {
        $resEstoque = Invoke-RestMethod -Uri "$baseUrl/estoque" -Method POST -Body $estoqueBody -ContentType "application/json"
        Write-Host "  Estoque criado! ID: $($resEstoque.id) | Qtd: $($resEstoque.quantidade) | Min: $($resEstoque.quantidadeMinima) | Local: $($resEstoque.localizacao)" -ForegroundColor Green
        $estoqueIds += $resEstoque.id
    } catch {
        Write-Host "  Erro ao criar estoque: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
        exit 1
    }
}

# =============================================
# ETAPA 4 - VENDAS
# =============================================
Write-Host "`n>>> ETAPA 4: CRIANDO VENDAS <<<" -ForegroundColor Magenta

# Venda 1 - Maria compra Notebook + Teclado
Write-Host "`n4.1 Criando Venda 1 (Maria - Notebook + Teclado)..." -ForegroundColor Yellow
$venda1 = @{
    clienteId = $clienteId1
    observacao = "Compra de equipamentos para home office"
    itens = @(
        @{ produtoId = $produtoIds[0]; quantidade = 1 },
        @{ produtoId = $produtoIds[2]; quantidade = 2 }
    )
} | ConvertTo-Json -Depth 3

try {
    $resVenda1 = Invoke-RestMethod -Uri "$baseUrl/vendas" -Method POST -Body $venda1 -ContentType "application/json"
    Write-Host "  Venda 1 criada! ID: $($resVenda1.id) | Status: $($resVenda1.status) | Total: R$ $($resVenda1.valorTotal)" -ForegroundColor Green
    $vendaId1 = $resVenda1.id
    foreach ($item in $resVenda1.itens) {
        Write-Host "    - $($item.produtoNome) x$($item.quantidade) = R$ $($item.subtotal)"
    }
} catch {
    Write-Host "  Erro ao criar venda 1: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
    exit 1
}

# Confirmar Venda 1
Write-Host "`n4.2 Confirmando Venda 1..." -ForegroundColor Yellow
$confirmar1 = @{ formaPagamento = "Cartao de Credito" } | ConvertTo-Json
try {
    $resConfirma1 = Invoke-RestMethod -Uri "$baseUrl/vendas/$vendaId1/confirmar" -Method POST -Body $confirmar1 -ContentType "application/json"
    Write-Host "  Venda 1 confirmada! Status: $($resConfirma1.status) | Pagamento: $($resConfirma1.formaPagamento)" -ForegroundColor Green
} catch {
    Write-Host "  Erro ao confirmar venda 1: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
}

# Venda 2 - Joao compra Monitor + Mouse + Cadeira
Write-Host "`n4.3 Criando Venda 2 (Joao - Monitor + Mouse + Cadeira)..." -ForegroundColor Yellow
$venda2 = @{
    clienteId = $clienteId2
    observacao = "Montagem de escritorio"
    itens = @(
        @{ produtoId = $produtoIds[1]; quantidade = 2 },
        @{ produtoId = $produtoIds[3]; quantidade = 1 },
        @{ produtoId = $produtoIds[4]; quantidade = 1 }
    )
} | ConvertTo-Json -Depth 3

try {
    $resVenda2 = Invoke-RestMethod -Uri "$baseUrl/vendas" -Method POST -Body $venda2 -ContentType "application/json"
    Write-Host "  Venda 2 criada! ID: $($resVenda2.id) | Status: $($resVenda2.status) | Total: R$ $($resVenda2.valorTotal)" -ForegroundColor Green
    $vendaId2 = $resVenda2.id
    foreach ($item in $resVenda2.itens) {
        Write-Host "    - $($item.produtoNome) x$($item.quantidade) = R$ $($item.subtotal)"
    }
} catch {
    Write-Host "  Erro ao criar venda 2: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
    exit 1
}

# Confirmar Venda 2
Write-Host "`n4.4 Confirmando Venda 2..." -ForegroundColor Yellow
$confirmar2 = @{ formaPagamento = "Boleto Bancario" } | ConvertTo-Json
try {
    $resConfirma2 = Invoke-RestMethod -Uri "$baseUrl/vendas/$vendaId2/confirmar" -Method POST -Body $confirmar2 -ContentType "application/json"
    Write-Host "  Venda 2 confirmada! Status: $($resConfirma2.status) | Pagamento: $($resConfirma2.formaPagamento)" -ForegroundColor Green
} catch {
    Write-Host "  Erro ao confirmar venda 2: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
}

# Venda 3 - Tech Solutions compra em quantidade (Notebooks + Monitores + Teclados + Mouses)
Write-Host "`n4.5 Criando Venda 3 (Tech Solutions - compra corporativa)..." -ForegroundColor Yellow
$venda3 = @{
    clienteId = $clienteId3
    observacao = "Compra corporativa - equipar novo escritorio"
    itens = @(
        @{ produtoId = $produtoIds[0]; quantidade = 5 },
        @{ produtoId = $produtoIds[1]; quantidade = 5 },
        @{ produtoId = $produtoIds[2]; quantidade = 5 },
        @{ produtoId = $produtoIds[3]; quantidade = 5 }
    )
} | ConvertTo-Json -Depth 3

try {
    $resVenda3 = Invoke-RestMethod -Uri "$baseUrl/vendas" -Method POST -Body $venda3 -ContentType "application/json"
    Write-Host "  Venda 3 criada! ID: $($resVenda3.id) | Status: $($resVenda3.status) | Total: R$ $($resVenda3.valorTotal)" -ForegroundColor Green
    $vendaId3 = $resVenda3.id
    foreach ($item in $resVenda3.itens) {
        Write-Host "    - $($item.produtoNome) x$($item.quantidade) = R$ $($item.subtotal)"
    }
} catch {
    Write-Host "  Erro ao criar venda 3: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
    exit 1
}

# Confirmar Venda 3
Write-Host "`n4.6 Confirmando Venda 3..." -ForegroundColor Yellow
$confirmar3 = @{ formaPagamento = "Transferencia Bancaria" } | ConvertTo-Json
try {
    $resConfirma3 = Invoke-RestMethod -Uri "$baseUrl/vendas/$vendaId3/confirmar" -Method POST -Body $confirmar3 -ContentType "application/json"
    Write-Host "  Venda 3 confirmada! Status: $($resConfirma3.status) | Pagamento: $($resConfirma3.formaPagamento)" -ForegroundColor Green
} catch {
    Write-Host "  Erro ao confirmar venda 3: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
}

# Venda 4 - Maria faz outra compra (Mouse) e depois cancela
Write-Host "`n4.7 Criando Venda 4 (Maria - Mouse, sera cancelada)..." -ForegroundColor Yellow
$venda4 = @{
    clienteId = $clienteId1
    observacao = "Compra adicional"
    itens = @(
        @{ produtoId = $produtoIds[3]; quantidade = 1 }
    )
} | ConvertTo-Json -Depth 3

try {
    $resVenda4 = Invoke-RestMethod -Uri "$baseUrl/vendas" -Method POST -Body $venda4 -ContentType "application/json"
    Write-Host "  Venda 4 criada! ID: $($resVenda4.id) | Status: $($resVenda4.status) | Total: R$ $($resVenda4.valorTotal)" -ForegroundColor Green
    $vendaId4 = $resVenda4.id
} catch {
    Write-Host "  Erro ao criar venda 4: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
    exit 1
}

# Cancelar Venda 4
Write-Host "`n4.8 Cancelando Venda 4..." -ForegroundColor Yellow
try {
    $resCancela4 = Invoke-RestMethod -Uri "$baseUrl/vendas/$vendaId4/cancelar" -Method POST
    Write-Host "  Venda 4 cancelada! Status: $($resCancela4.status)" -ForegroundColor Green
} catch {
    Write-Host "  Erro ao cancelar venda 4: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  $($_.ErrorDetails.Message)" -ForegroundColor Red
}

# =============================================
# ETAPA 5 - VERIFICACAO FINAL
# =============================================
Write-Host "`n>>> ETAPA 5: VERIFICACAO FINAL <<<" -ForegroundColor Magenta

# Listar todos os clientes
Write-Host "`n5.1 Listando todos os clientes..." -ForegroundColor Yellow
try {
    $todosClientes = Invoke-RestMethod -Uri "$baseUrl/clientes" -Method GET
    Write-Host "  Total de clientes: $($todosClientes.Count)" -ForegroundColor Cyan
    foreach ($c in $todosClientes) {
        Write-Host "    - $($c.nome) | Doc: $($c.documento) | Contatos Sec.: $($c.contatosSecundarios.Count) | Enderecos Sec.: $($c.enderecosSecundarios.Count)"
    }
} catch {
    Write-Host "  Erro: $($_.Exception.Message)" -ForegroundColor Red
}

# Listar todos os produtos
Write-Host "`n5.2 Listando todos os produtos..." -ForegroundColor Yellow
try {
    $todosProdutos = Invoke-RestMethod -Uri "$baseUrl/produtos" -Method GET
    Write-Host "  Total de produtos: $($todosProdutos.Count)" -ForegroundColor Cyan
    foreach ($p in $todosProdutos) {
        Write-Host "    - [$($p.codigo)] $($p.nome) | R$ $($p.precoUnitario) | Ativo: $($p.ativo)"
    }
} catch {
    Write-Host "  Erro: $($_.Exception.Message)" -ForegroundColor Red
}

# Listar estoque
Write-Host "`n5.3 Listando estoque..." -ForegroundColor Yellow
try {
    $todosEstoques = Invoke-RestMethod -Uri "$baseUrl/estoque" -Method GET
    Write-Host "  Total de registros de estoque: $($todosEstoques.Count)" -ForegroundColor Cyan
    foreach ($e in $todosEstoques) {
        $statusEstoque = if ($e.abaixoDoMinimo) { "[BAIXO]" } else { "[OK]" }
        $corEstoque = if ($e.abaixoDoMinimo) { "Red" } else { "Green" }
        Write-Host "    - $($e.produtoNome) | Qtd: $($e.quantidade) | Min: $($e.quantidadeMinima) | $statusEstoque" -ForegroundColor $corEstoque
    }
} catch {
    Write-Host "  Erro: $($_.Exception.Message)" -ForegroundColor Red
}

# Listar todas as vendas
Write-Host "`n5.4 Listando todas as vendas..." -ForegroundColor Yellow
try {
    $todasVendas = Invoke-RestMethod -Uri "$baseUrl/vendas" -Method GET
    Write-Host "  Total de vendas: $($todasVendas.Count)" -ForegroundColor Cyan
    foreach ($v in $todasVendas) {
        $corStatus = switch ($v.status) {
            "Confirmada" { "Green" }
            "Cancelada" { "Red" }
            default { "Yellow" }
        }
        Write-Host "    - Cliente: $($v.clienteNome) | Status: $($v.status) | Total: R$ $($v.valorTotal) | Itens: $($v.itens.Count) | Pagamento: $($v.formaPagamento)" -ForegroundColor $corStatus
    }
} catch {
    Write-Host "  Erro: $($_.Exception.Message)" -ForegroundColor Red
}

# Resumo
Write-Host "`n=============================================" -ForegroundColor Cyan
Write-Host "  RESUMO FINAL" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "  Clientes criados:  3 (2 PF + 1 PJ)"
Write-Host "  Produtos criados:  5"
Write-Host "  Estoques criados:  5"
Write-Host "  Vendas criadas:    4 (3 confirmadas + 1 cancelada)"
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "  FIM DO SEED DE DADOS" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
