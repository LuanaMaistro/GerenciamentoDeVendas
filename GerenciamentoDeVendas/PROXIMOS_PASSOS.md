# Próximos Passos - Sistema de Gerenciamento de Vendas e Estoque

## Estado Atual do Projeto

O projeto está em estágio inicial, seguindo uma arquitetura **Domain-Driven Design (DDD)** com .NET 8.0.

### O que já está implementado

| Componente | Status | Observações |
|------------|--------|-------------|
| `CNPJ.cs` | ✅ Completo | Validação, formatação, comparação de igualdade |
| `Cliente.cs` | ⚠️ Parcial | Estrutura básica com ativação/inativação |
| `Produto.cs` | ❌ Stub | Classe vazia |
| `Venda.cs` | ❌ Stub | Classe vazia |
| `Estoque.cs` | ❌ Stub | Classe vazia |
| `CPF.cs` | ❌ Stub | Classe vazia |
| `Contato.cs` | ❌ Stub | Classe vazia |
| `Endereco.cs` | ❌ Stub | Classe vazia |
| `Documento.cs` | ⚠️ Mínimo | Apenas flag booleana |

### Estrutura Atual

```
GerenciamentoDeVendas/
├── Domain/
│   ├── Entities/
│   │   ├── Cliente.cs
│   │   ├── Produto.cs
│   │   ├── Venda.cs
│   │   └── Estoque.cs
│   └── ValueObjects/
│       ├── CNPJ.cs
│       ├── CPF.cs
│       ├── Documento.cs
│       ├── Contato.cs
│       └── Endereco.cs
└── Teste.Domain/
    ├── CNPJTest.cs
    ├── CPFTest.cs
    ├── CustomerTest.cs
    └── DocumentTest.cs
```

---

## Próximos Passos

### Fase 1: Completar a Camada de Domínio

#### 1.1 Value Objects

**CPF.cs**
- Implementar validação do CPF (algoritmo dos dígitos verificadores)
- Método `Clean()` para remover caracteres não numéricos
- Método `GetFormatted()` para retornar no formato XXX.XXX.XXX-XX
- Implementar `IEquatable<CPF>`
- Sobrescrever operadores de igualdade

**Endereco.cs**
- Propriedades: CEP, Logradouro, Numero, Complemento, Bairro, Cidade, UF
- Validação de CEP
- Método para endereço formatado

**Contato.cs**
- Propriedades: Tipo (Telefone, Celular, Email), Valor, Principal
- Validação de formato de telefone e email

**Documento.cs**
- Refatorar para ser uma classe abstrata ou usar pattern Strategy
- Permitir comportamento polimórfico entre CPF e CNPJ

#### 1.2 Entidades

**Produto.cs**
```csharp
// Propriedades sugeridas:
- Id (Guid)
- Codigo (string)
- Nome (string)
- Descricao (string)
- PrecoUnitario (decimal)
- Categoria (string ou enum)
- Ativo (bool)
- DataCadastro (DateTime)
```

**Estoque.cs**
```csharp
// Propriedades sugeridas:
- Id (Guid)
- ProdutoId (Guid)
- Quantidade (int)
- QuantidadeMinima (int)
- Localizacao (string)

// Métodos sugeridos:
- AdicionarQuantidade(int quantidade)
- RemoverQuantidade(int quantidade)
- EstaAbaixoDoMinimo()
```

**Venda.cs**
```csharp
// Propriedades sugeridas:
- Id (Guid)
- ClienteId (Guid)
- DataVenda (DateTime)
- Itens (List<ItemVenda>)
- ValorTotal (decimal)
- Status (enum: Pendente, Confirmada, Cancelada)
- FormaPagamento (enum)

// Métodos sugeridos:
- AdicionarItem(ItemVenda item)
- RemoverItem(Guid itemId)
- CalcularTotal()
- Confirmar()
- Cancelar()
```

**ItemVenda.cs** (Nova entidade)
```csharp
// Propriedades sugeridas:
- Id (Guid)
- VendaId (Guid)
- ProdutoId (Guid)
- Quantidade (int)
- PrecoUnitario (decimal)
- Subtotal (decimal)
```

---

### Fase 2: Criar Camada de Application

#### Estrutura proposta:

```
Application/
├── Application.csproj
├── Interfaces/
│   ├── Repositories/
│   │   ├── IClienteRepository.cs
│   │   ├── IProdutoRepository.cs
│   │   ├── IVendaRepository.cs
│   │   └── IEstoqueRepository.cs
│   └── Services/
│       ├── IClienteService.cs
│       ├── IProdutoService.cs
│       ├── IVendaService.cs
│       └── IEstoqueService.cs
├── Services/
│   ├── ClienteService.cs
│   ├── ProdutoService.cs
│   ├── VendaService.cs
│   └── EstoqueService.cs
├── DTOs/
│   ├── ClienteDTO.cs
│   ├── ProdutoDTO.cs
│   ├── VendaDTO.cs
│   └── EstoqueDTO.cs
└── Mappings/
    └── AutoMapperProfile.cs
```

#### Responsabilidades:
- **Interfaces de Repository**: Contratos para acesso a dados
- **Services**: Lógica de negócio e orquestração
- **DTOs**: Objetos de transferência de dados entre camadas

---

### Fase 3: Criar Camada de Infrastructure

#### Estrutura proposta:

```
Infrastructure/
├── Infrastructure.csproj
├── Data/
│   ├── AppDbContext.cs
│   └── Configurations/
│       ├── ClienteConfiguration.cs
│       ├── ProdutoConfiguration.cs
│       ├── VendaConfiguration.cs
│       └── EstoqueConfiguration.cs
├── Repositories/
│   ├── ClienteRepository.cs
│   ├── ProdutoRepository.cs
│   ├── VendaRepository.cs
│   └── EstoqueRepository.cs
└── Migrations/
```

#### Tecnologias sugeridas:
- **Entity Framework Core** - ORM
- **SQL Server** ou **PostgreSQL** - Banco de dados
- **Fluent API** - Configuração de mapeamento

---

### Fase 4: Criar Camada de API

#### Estrutura proposta:

```
API/
├── API.csproj
├── Program.cs
├── appsettings.json
├── Controllers/
│   ├── ClientesController.cs
│   ├── ProdutosController.cs
│   ├── VendasController.cs
│   └── EstoqueController.cs
└── Middlewares/
    └── ExceptionMiddleware.cs
```

#### Endpoints sugeridos:

**Clientes**
- `GET /api/clientes` - Listar todos
- `GET /api/clientes/{id}` - Buscar por ID
- `POST /api/clientes` - Criar novo
- `PUT /api/clientes/{id}` - Atualizar
- `DELETE /api/clientes/{id}` - Remover
- `PATCH /api/clientes/{id}/ativar` - Ativar cliente
- `PATCH /api/clientes/{id}/inativar` - Inativar cliente

**Produtos**
- `GET /api/produtos` - Listar todos
- `GET /api/produtos/{id}` - Buscar por ID
- `POST /api/produtos` - Criar novo
- `PUT /api/produtos/{id}` - Atualizar
- `DELETE /api/produtos/{id}` - Remover

**Estoque**
- `GET /api/estoque` - Listar todos
- `GET /api/estoque/{produtoId}` - Buscar por produto
- `POST /api/estoque/entrada` - Registrar entrada
- `POST /api/estoque/saida` - Registrar saída
- `GET /api/estoque/baixo` - Produtos com estoque baixo

**Vendas**
- `GET /api/vendas` - Listar todas
- `GET /api/vendas/{id}` - Buscar por ID
- `POST /api/vendas` - Criar nova venda
- `POST /api/vendas/{id}/confirmar` - Confirmar venda
- `POST /api/vendas/{id}/cancelar` - Cancelar venda
- `GET /api/vendas/cliente/{clienteId}` - Vendas por cliente

---

### Fase 5: Testes

#### Correções necessárias:
- `CustomerTest.cs` - O método `AtivaInativa()` chamado nos testes não existe na classe `Cliente` (existem apenas `Ativa()` e `Inativa()`)
- `DocumentTest.cs` - Código está comentado, implementar testes reais

#### Novos testes a implementar:
- Testes unitários para CPF
- Testes unitários para Endereco e Contato
- Testes unitários para Produto, Venda e Estoque
- Testes de integração para repositories
- Testes de integração para API

---

## Ordem de Prioridade

| # | Tarefa | Complexidade | Impacto |
|---|--------|--------------|---------|
| 1 | Completar Value Objects (CPF, Endereco, Contato) | Média | Alto |
| 2 | Implementar entidade Produto | Baixa | Alto |
| 3 | Implementar entidade Estoque | Média | Alto |
| 4 | Implementar entidades Venda e ItemVenda | Alta | Alto |
| 5 | Criar projeto Application com interfaces | Média | Alto |
| 6 | Criar projeto Infrastructure com EF Core | Alta | Alto |
| 7 | Criar projeto API com controllers | Média | Alto |
| 8 | Configurar Dependency Injection | Baixa | Médio |
| 9 | Implementar autenticação/autorização | Alta | Médio |
| 10 | Expandir cobertura de testes | Média | Médio |

---

## Considerações Adicionais

### Padrões e Boas Práticas
- Manter separação clara entre camadas
- Usar injeção de dependência
- Implementar validações no domínio (fail fast)
- Usar Result Pattern para tratamento de erros
- Seguir convenções de nomenclatura C#

### Funcionalidades Futuras (Pós-MVP)
- Relatórios de vendas
- Dashboard com métricas
- Notificações de estoque baixo
- Histórico de movimentações
- Exportação de dados (PDF, Excel)
- Integração com sistemas de pagamento

---

## Arquitetura Final Esperada

```
GerenciamentoDeVendas/
├── Domain/                 # Entidades, Value Objects, Enums
├── Application/            # Interfaces, Services, DTOs
├── Infrastructure/         # DbContext, Repositories, Migrations
├── API/                    # Controllers, Middlewares, Program.cs
├── Teste.Domain/           # Testes unitários do domínio
├── Teste.Application/      # Testes dos services
└── Teste.Integration/      # Testes de integração
```
