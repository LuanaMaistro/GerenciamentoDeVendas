$baseUrl = "http://localhost:5001/api"

Write-Host "=== TESTANDO FLUXO COMPLETO ===" -ForegroundColor Cyan

# 1. Criar Cliente com contato e endereco principal + secundarios
Write-Host "`n1. Criando Cliente (com contato/endereco principal e secundarios)..." -ForegroundColor Yellow
$cliente = @{
    nome = "Ana Paula Ferreira"
    documento = "42383022063"
    contatoPrincipal = @{
        telefone = "1132223333"
        celular = "11987654321"
        email = "ana.principal@email.com"
    }
    contatosSecundarios = @(
        @{
            telefone = "1144445555"
            celular = $null
            email = "ana.trabalho@empresa.com"
        },
        @{
            telefone = $null
            celular = "11976543210"
            email = $null
        }
    )
    enderecoPrincipal = @{
        cep = "01310100"
        logradouro = "Avenida Paulista"
        numero = "1500"
        complemento = "Andar 10"
        bairro = "Bela Vista"
        cidade = "Sao Paulo"
        uf = "SP"
    }
    enderecosSecundarios = @(
        @{
            cep = "04543011"
            logradouro = "Rua Funchal"
            numero = "418"
            complemento = "Sala 302"
            bairro = "Vila Olimpia"
            cidade = "Sao Paulo"
            uf = "SP"
        },
        @{
            cep = "22041080"
            logradouro = "Rua Barata Ribeiro"
            numero = "200"
            complemento = $null
            bairro = "Copacabana"
            cidade = "Rio de Janeiro"
            uf = "RJ"
        }
    )
} | ConvertTo-Json -Depth 3

try {
    $resCliente = Invoke-RestMethod -Uri "$baseUrl/clientes" -Method POST -Body $cliente -ContentType "application/json"
    Write-Host "Cliente criado com sucesso!" -ForegroundColor Green
    Write-Host "  ID: $($resCliente.id)"
    Write-Host "  Nome: $($resCliente.nome)"
    Write-Host "  Contato Principal:" -ForegroundColor Cyan
    Write-Host "    Tel: $($resCliente.contatoPrincipal.telefone)"
    Write-Host "    Cel: $($resCliente.contatoPrincipal.celular)"
    Write-Host "    Email: $($resCliente.contatoPrincipal.email)"
    Write-Host "  Contatos Secundarios: $($resCliente.contatosSecundarios.Count)" -ForegroundColor Cyan
    foreach ($cs in $resCliente.contatosSecundarios) {
        Write-Host "    - Tel: $($cs.telefone) | Cel: $($cs.celular) | Email: $($cs.email)"
    }
    Write-Host "  Endereco Principal: $($resCliente.enderecoPrincipal.logradouro), $($resCliente.enderecoPrincipal.numero)" -ForegroundColor Cyan
    Write-Host "  Enderecos Secundarios: $($resCliente.enderecosSecundarios.Count)" -ForegroundColor Cyan
    foreach ($es in $resCliente.enderecosSecundarios) {
        Write-Host "    - $($es.logradouro), $($es.numero) - $($es.bairro), $($es.cidade)/$($es.uf)"
    }
    $clienteId = $resCliente.id
} catch {
    Write-Host "Erro ao criar cliente: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.ErrorDetails.Message
    exit 1
}

# 2. Buscar o cliente para confirmar persistencia
Write-Host "`n2. Buscando cliente para confirmar persistencia..." -ForegroundColor Yellow
try {
    $clienteBuscado = Invoke-RestMethod -Uri "$baseUrl/clientes/$clienteId" -Method GET
    Write-Host "Cliente encontrado!" -ForegroundColor Green
    Write-Host "  Contato Principal:" -ForegroundColor Cyan
    Write-Host "    Tel: $($clienteBuscado.contatoPrincipal.telefone)"
    Write-Host "    Cel: $($clienteBuscado.contatoPrincipal.celular)"
    Write-Host "    Email: $($clienteBuscado.contatoPrincipal.email)"
    Write-Host "  Contatos Secundarios: $($clienteBuscado.contatosSecundarios.Count)" -ForegroundColor Cyan
    foreach ($cs in $clienteBuscado.contatosSecundarios) {
        Write-Host "    - Tel: $($cs.telefone) | Cel: $($cs.celular) | Email: $($cs.email)"
    }
    Write-Host "  Endereco Principal: $($clienteBuscado.enderecoPrincipal.logradouro), $($clienteBuscado.enderecoPrincipal.numero)" -ForegroundColor Cyan
    Write-Host "  Enderecos Secundarios: $($clienteBuscado.enderecosSecundarios.Count)" -ForegroundColor Cyan
    foreach ($es in $clienteBuscado.enderecosSecundarios) {
        Write-Host "    - $($es.logradouro), $($es.numero) - $($es.bairro), $($es.cidade)/$($es.uf)"
    }
} catch {
    Write-Host "Erro ao buscar cliente: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.ErrorDetails.Message
}

# 3. Adicionar mais um contato secundario via endpoint
Write-Host "`n3. Adicionando contato secundario via endpoint..." -ForegroundColor Yellow
$novoContato = @{
    telefone = $null
    celular = $null
    email = "ana.pessoal@gmail.com"
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "$baseUrl/clientes/$clienteId/contatos" -Method POST -Body $novoContato -ContentType "application/json"
    Write-Host "Contato secundario adicionado!" -ForegroundColor Green
} catch {
    Write-Host "Erro ao adicionar contato: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.ErrorDetails.Message
}

# 4. Adicionar mais um endereco secundario via endpoint
Write-Host "`n4. Adicionando endereco secundario via endpoint..." -ForegroundColor Yellow
$novoEndereco = @{
    cep = "30130000"
    logradouro = "Praca Sete de Setembro"
    numero = "100"
    complemento = $null
    bairro = "Centro"
    cidade = "Belo Horizonte"
    uf = "MG"
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "$baseUrl/clientes/$clienteId/enderecos" -Method POST -Body $novoEndereco -ContentType "application/json"
    Write-Host "Endereco secundario adicionado!" -ForegroundColor Green
} catch {
    Write-Host "Erro ao adicionar endereco: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.ErrorDetails.Message
}

# 5. Buscar novamente para confirmar tudo salvo
Write-Host "`n5. Buscando cliente final para confirmar tudo..." -ForegroundColor Yellow
try {
    $clienteFinal = Invoke-RestMethod -Uri "$baseUrl/clientes/$clienteId" -Method GET
    Write-Host "=== RESULTADO FINAL ===" -ForegroundColor Cyan
    Write-Host "  Nome: $($clienteFinal.nome)"
    Write-Host "  Documento: $($clienteFinal.documento)"
    Write-Host ""
    Write-Host "  CONTATO PRINCIPAL:" -ForegroundColor Green
    Write-Host "    Tel: $($clienteFinal.contatoPrincipal.telefone)"
    Write-Host "    Cel: $($clienteFinal.contatoPrincipal.celular)"
    Write-Host "    Email: $($clienteFinal.contatoPrincipal.email)"
    Write-Host ""
    Write-Host "  CONTATOS SECUNDARIOS ($($clienteFinal.contatosSecundarios.Count)):" -ForegroundColor Green
    $i = 1
    foreach ($cs in $clienteFinal.contatosSecundarios) {
        Write-Host "    [$i] Tel: $($cs.telefone) | Cel: $($cs.celular) | Email: $($cs.email)"
        $i++
    }
    Write-Host ""
    Write-Host "  ENDERECO PRINCIPAL:" -ForegroundColor Green
    Write-Host "    $($clienteFinal.enderecoPrincipal.logradouro), $($clienteFinal.enderecoPrincipal.numero) - $($clienteFinal.enderecoPrincipal.bairro)"
    Write-Host "    $($clienteFinal.enderecoPrincipal.cidade)/$($clienteFinal.enderecoPrincipal.uf) - CEP: $($clienteFinal.enderecoPrincipal.cep)"
    Write-Host ""
    Write-Host "  ENDERECOS SECUNDARIOS ($($clienteFinal.enderecosSecundarios.Count)):" -ForegroundColor Green
    $i = 1
    foreach ($es in $clienteFinal.enderecosSecundarios) {
        Write-Host "    [$i] $($es.logradouro), $($es.numero) - $($es.bairro)"
        Write-Host "        $($es.cidade)/$($es.uf) - CEP: $($es.cep)"
        $i++
    }

    # Validacao
    Write-Host ""
    $contatosOk = $clienteFinal.contatosSecundarios.Count -eq 3
    $enderecosOk = $clienteFinal.enderecosSecundarios.Count -eq 3
    if ($contatosOk -and $enderecosOk) {
        Write-Host "TUDO PERSISTIDO CORRETAMENTE! (3 contatos sec. + 3 enderecos sec.)" -ForegroundColor Green
    } else {
        Write-Host "ATENCAO: Esperava 3 contatos sec. e 3 enderecos sec." -ForegroundColor Red
        Write-Host "  Contatos sec.: $($clienteFinal.contatosSecundarios.Count)"
        Write-Host "  Enderecos sec.: $($clienteFinal.enderecosSecundarios.Count)"
    }
} catch {
    Write-Host "Erro ao buscar cliente: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.ErrorDetails.Message
}

Write-Host "`n=== FIM DO TESTE ===" -ForegroundColor Cyan
