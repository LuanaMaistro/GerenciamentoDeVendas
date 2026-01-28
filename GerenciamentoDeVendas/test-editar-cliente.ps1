$baseUrl = "http://localhost:5001/api"

Write-Host "=== TESTANDO EDICAO DE CLIENTE ===" -ForegroundColor Cyan

# 1. Buscar cliente pelo documento para pegar o ID
Write-Host "`n1. Buscando cliente pelo documento 42383022063..." -ForegroundColor Yellow
try {
    $cliente = Invoke-RestMethod -Uri "$baseUrl/clientes/documento/42383022063" -Method GET
    Write-Host "Cliente encontrado!" -ForegroundColor Green
    Write-Host "  ID: $($cliente.id)"
    Write-Host "  Nome: $($cliente.nome)"
    $clienteId = $cliente.id
} catch {
    Write-Host "Erro ao buscar cliente: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.ErrorDetails.Message
    exit 1
}

# 2. Primeira edicao - alterar nome, contato e endereco principal
Write-Host "`n2. [1a EDICAO] Alterando nome, contato e endereco principal..." -ForegroundColor Yellow
$update1 = @{
    nome = "Ana Paula Ferreira Santos"
    contatoPrincipal = @{
        telefone = "1133334444"
        celular = "11999887766"
        email = "ana.novo@email.com"
    }
    enderecoPrincipal = @{
        cep = "04547000"
        logradouro = "Rua Gomes de Carvalho"
        numero = "1225"
        complemento = "Bloco B, Sala 10"
        bairro = "Vila Olimpia"
        cidade = "Sao Paulo"
        uf = "SP"
    }
} | ConvertTo-Json -Depth 3

try {
    $res1 = Invoke-RestMethod -Uri "$baseUrl/clientes/$clienteId" -Method PUT -Body $update1 -ContentType "application/json"
    Write-Host "Atualizado!" -ForegroundColor Green
    Write-Host "  Nome: $($res1.nome)"
    Write-Host "  Tel: $($res1.contatoPrincipal.telefone) | Cel: $($res1.contatoPrincipal.celular) | Email: $($res1.contatoPrincipal.email)"
    Write-Host "  End: $($res1.enderecoPrincipal.logradouro), $($res1.enderecoPrincipal.numero) - $($res1.enderecoPrincipal.cidade)/$($res1.enderecoPrincipal.uf)"
} catch {
    Write-Host "Erro: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.ErrorDetails.Message
}

# 3. Adicionar contatos e enderecos secundarios
Write-Host "`n3. Adicionando contatos e enderecos secundarios..." -ForegroundColor Yellow

$contato1 = @{ telefone = $null; celular = "11912345678"; email = "ana.trabalho@empresa.com" } | ConvertTo-Json
$contato2 = @{ telefone = "2133334444"; celular = $null; email = "ana.pessoal@gmail.com" } | ConvertTo-Json
$endereco1 = @{ cep = "22041080"; logradouro = "Rua Barata Ribeiro"; numero = "200"; complemento = "Apto 501"; bairro = "Copacabana"; cidade = "Rio de Janeiro"; uf = "RJ" } | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "$baseUrl/clientes/$clienteId/contatos" -Method POST -Body $contato1 -ContentType "application/json"
    Write-Host "  Contato sec. 1 adicionado!" -ForegroundColor Green
    Invoke-RestMethod -Uri "$baseUrl/clientes/$clienteId/contatos" -Method POST -Body $contato2 -ContentType "application/json"
    Write-Host "  Contato sec. 2 adicionado!" -ForegroundColor Green
    Invoke-RestMethod -Uri "$baseUrl/clientes/$clienteId/enderecos" -Method POST -Body $endereco1 -ContentType "application/json"
    Write-Host "  Endereco sec. 1 adicionado!" -ForegroundColor Green
} catch {
    Write-Host "Erro: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.ErrorDetails.Message
}

# 4. Segunda edicao - alterar novamente nome, contato e endereco principal
Write-Host "`n4. [2a EDICAO] Alterando dados principais de novo..." -ForegroundColor Yellow
$update2 = @{
    nome = "Ana Paula F. Santos Silva"
    contatoPrincipal = @{
        telefone = "1155556666"
        celular = "11911112222"
        email = "ana.silva@novaempresa.com"
    }
    enderecoPrincipal = @{
        cep = "30130000"
        logradouro = "Praca Sete de Setembro"
        numero = "50"
        complemento = "Sala 201"
        bairro = "Centro"
        cidade = "Belo Horizonte"
        uf = "MG"
    }
} | ConvertTo-Json -Depth 3

try {
    $res2 = Invoke-RestMethod -Uri "$baseUrl/clientes/$clienteId" -Method PUT -Body $update2 -ContentType "application/json"
    Write-Host "Atualizado!" -ForegroundColor Green
    Write-Host "  Nome: $($res2.nome)"
    Write-Host "  Tel: $($res2.contatoPrincipal.telefone) | Cel: $($res2.contatoPrincipal.celular) | Email: $($res2.contatoPrincipal.email)"
    Write-Host "  End: $($res2.enderecoPrincipal.logradouro), $($res2.enderecoPrincipal.numero) - $($res2.enderecoPrincipal.cidade)/$($res2.enderecoPrincipal.uf)"
} catch {
    Write-Host "Erro: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.ErrorDetails.Message
}

# 5. Buscar cliente final para confirmar tudo
Write-Host "`n5. Buscando cliente final para confirmar tudo..." -ForegroundColor Yellow
try {
    $clienteFinal = Invoke-RestMethod -Uri "$baseUrl/clientes/$clienteId" -Method GET
    Write-Host ""
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
    Write-Host "    $($clienteFinal.enderecoPrincipal.logradouro), $($clienteFinal.enderecoPrincipal.numero)"
    Write-Host "    $($clienteFinal.enderecoPrincipal.bairro) - $($clienteFinal.enderecoPrincipal.cidade)/$($clienteFinal.enderecoPrincipal.uf)"
    Write-Host "    CEP: $($clienteFinal.enderecoPrincipal.cep)"
    Write-Host ""
    Write-Host "  ENDERECOS SECUNDARIOS ($($clienteFinal.enderecosSecundarios.Count)):" -ForegroundColor Green
    $i = 1
    foreach ($es in $clienteFinal.enderecosSecundarios) {
        Write-Host "    [$i] $($es.logradouro), $($es.numero) - $($es.bairro)"
        Write-Host "        $($es.cidade)/$($es.uf) - CEP: $($es.cep)"
        $i++
    }
} catch {
    Write-Host "Erro ao buscar cliente: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.ErrorDetails.Message
}

Write-Host "`n=== FIM DO TESTE DE EDICAO ===" -ForegroundColor Cyan
