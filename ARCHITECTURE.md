# Arquitetura do Link Manager

## Visão Geral

O Link Manager é uma aplicação web construída com **Blazor Server** seguindo o padrão arquitetural **MVC (Model-View-Controller)** com separação clara de responsabilidades através de camadas.

## Diagrama de Arquitetura

```
┌─────────────────────────────────────────────────────────────┐
│                        PRESENTATION                          │
│                     (Blazor Server Pages)                    │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Index.razor - Página Principal                       │  │
│  │  - Formulário de cadastro                             │  │
│  │  - Lista de links                                     │  │
│  │  - Dashboard com estatísticas                         │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ↓ ↑
┌─────────────────────────────────────────────────────────────┐
│                      BUSINESS LOGIC                          │
│                        (Services)                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  HealthCheckerService                                 │  │
│  │  - CheckHealthAsync()                                 │  │
│  │  - CheckAndUpdateAsync()                              │  │
│  │  - ExtractMetadata()                                  │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ↓ ↑
┌─────────────────────────────────────────────────────────────┐
│                    DATA ACCESS LAYER                         │
│                      (Repository)                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  PageLinkRepository                                   │  │
│  │  - GetAllAsync()                                      │  │
│  │  - GetByIdAsync()                                     │  │
│  │  - AddAsync()                                         │  │
│  │  - UpdateAsync()                                      │  │
│  │  - DeleteAsync()                                      │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ↓ ↑
┌─────────────────────────────────────────────────────────────┐
│                      DATA PERSISTENCE                        │
│                   (Entity Framework Core)                    │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  ApplicationDbContext                                 │  │
│  │  - DbSet<PageLink>                                    │  │
│  │  - OnModelCreating()                                  │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ↓ ↑
┌─────────────────────────────────────────────────────────────┐
│                         DATABASE                             │
│                       SQL Server                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Tabela: PageLinks                                    │  │
│  │  - Índices otimizados                                 │  │
│  │  - Constraints e validações                           │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## Camadas da Aplicação

### 1. Presentation Layer (Camada de Apresentação)

**Responsabilidade**: Interface do usuário e interação com o usuário.

**Componentes**:
- `Pages/Index.razor`: Página principal com CRUD de links
- `Shared/MainLayout.razor`: Layout base da aplicação
- `Shared/NavMenu.razor`: Menu de navegação
- `Pages/_Host.cshtml`: Host HTML para Blazor Server

**Tecnologias**:
- Blazor Server (renderização no servidor)
- Bootstrap 5 (UI framework)
- Bootstrap Icons

### 2. Business Logic Layer (Camada de Lógica de Negócio)

**Responsabilidade**: Regras de negócio e processamento de dados.

**Componentes**:
- `Services/HealthCheckerService.cs`: Serviço de verificação de saúde
  - Realiza requisições HTTP
  - Mede tempo de resposta
  - Extrai metadados HTML (título e descrição)
  - Determina status do link

**Padrões Utilizados**:
- **Dependency Injection**: Injeção de dependências via interfaces
- **Service Pattern**: Encapsulamento de lógica de negócio

### 3. Data Access Layer (Camada de Acesso a Dados)

**Responsabilidade**: Abstração do acesso ao banco de dados.

**Componentes**:
- `Data/PageLinkRepository.cs`: Implementação do repositório
- `Data/IPageLinkRepository.cs`: Interface do repositório
- `Data/ApplicationDbContext.cs`: Contexto do Entity Framework

**Padrões Utilizados**:
- **Repository Pattern**: Abstração do acesso a dados
- **Unit of Work**: Gerenciamento de transações via DbContext

**Operações Disponíveis**:
```csharp
// CRUD Básico
GetAllAsync()           // Lista todos os links ativos
GetByIdAsync(id)        // Busca por ID
GetByUrlAsync(url)      // Busca por URL (evita duplicatas)
AddAsync(pageLink)      // Adiciona novo link
UpdateAsync(pageLink)   // Atualiza link existente
DeleteAsync(id)         // Remove link (soft delete)

// Consultas Especializadas
GetByCategoryAsync(category)              // Filtra por categoria
GetByStatusAsync(status)                  // Filtra por status
GetLinksNeedingCheckAsync(hoursThreshold) // Links que precisam verificação
```

### 4. Data Model Layer (Camada de Modelo de Dados)

**Responsabilidade**: Definição das entidades de domínio.

**Componentes**:
- `Models/PageLink.cs`: Entidade principal do sistema

**Propriedades da Entidade PageLink**:
```csharp
public class PageLink
{
    public int Id { get; set; }                    // PK
    public string Url { get; set; }                // URL única
    public string? Title { get; set; }             // Título da página
    public string? Description { get; set; }       // Descrição
    public string Status { get; set; }             // Status atual
    public int? HttpStatusCode { get; set; }       // Código HTTP
    public long? ResponseTimeMs { get; set; }      // Tempo de resposta
    public DateTime CreatedAt { get; set; }        // Data de criação
    public DateTime? LastCheckedAt { get; set; }   // Última verificação
    public string? ErrorMessage { get; set; }      // Mensagem de erro
    public string? Category { get; set; }          // Categoria
    public string? Notes { get; set; }             // Notas
    public bool IsActive { get; set; }             // Ativo/Inativo
}
```

### 5. Database Layer (Camada de Banco de Dados)

**Responsabilidade**: Persistência física dos dados.

**Tecnologia**: SQL Server

**Otimizações**:
- Índice único em `Url` (evita duplicatas)
- Índice em `Status` (filtros rápidos)
- Índice em `CreatedAt` (ordenação)
- Índice em `Category` (agrupamento)
- Soft delete via campo `IsActive`

## Fluxo de Dados

### Fluxo de Adição de Link

```
1. Usuário preenche formulário
   ↓
2. Index.razor → HandleAddLink()
   ↓
3. Repository.AddAsync(newLink)
   ↓
4. HealthChecker.CheckAndUpdateAsync(link)
   ├─ Faz requisição HTTP
   ├─ Extrai metadados HTML
   └─ Determina status
   ↓
5. Repository.UpdateAsync(link)
   ↓
6. EF Core persiste no SQL Server
   ↓
7. UI atualiza lista de links
```

### Fluxo de Health Check

```
1. Usuário clica em "Verificar Saúde"
   ↓
2. Index.razor → CheckHealth(link)
   ↓
3. HealthChecker.CheckHealthAsync(url)
   ├─ HttpClient.GetAsync(url)
   ├─ Mede tempo de resposta
   ├─ Verifica status HTTP
   └─ Extrai metadados com HtmlAgilityPack
   ↓
4. Atualiza propriedades do PageLink
   ↓
5. Repository.UpdateAsync(link)
   ↓
6. Retorna resultado para UI
```

## Padrões de Design Utilizados

### 1. Repository Pattern
Abstrai o acesso a dados, permitindo trocar a implementação sem afetar outras camadas.

```csharp
public interface IPageLinkRepository
{
    Task<List<PageLink>> GetAllAsync();
    Task<PageLink> AddAsync(PageLink pageLink);
    // ... outros métodos
}
```

### 2. Dependency Injection
Todas as dependências são injetadas via construtor, facilitando testes e manutenção.

```csharp
public class PageLinkRepository : IPageLinkRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PageLinkRepository> _logger;

    public PageLinkRepository(
        ApplicationDbContext context,
        ILogger<PageLinkRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
}
```

### 3. Service Pattern
Encapsula lógica de negócio complexa em serviços dedicados.

```csharp
public interface IHealthCheckerService
{
    Task<HealthCheckResult> CheckHealthAsync(string url);
    Task<PageLink> CheckAndUpdateAsync(PageLink pageLink);
}
```

### 4. Unit of Work
O `DbContext` do Entity Framework atua como Unit of Work, gerenciando transações.

### 5. Soft Delete
Links não são removidos fisicamente, apenas marcados como inativos.

```csharp
public async Task DeleteAsync(int id)
{
    var pageLink = await _context.PageLinks.FindAsync(id);
    pageLink.IsActive = false;
    await _context.SaveChangesAsync();
}
```

## Configuração e Injeção de Dependências

No `Program.cs`:

```csharp
// Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositórios
builder.Services.AddScoped<IPageLinkRepository, PageLinkRepository>();

// Serviços
builder.Services.AddScoped<IHealthCheckerService, HealthCheckerService>();

// HttpClient para requisições
builder.Services.AddHttpClient();
```

## Segurança

### Validações
- Data Annotations no modelo (`[Required]`, `[Url]`, `[MaxLength]`)
- Validação de duplicatas no repositório
- Timeout de 30 segundos em requisições HTTP

### Boas Práticas
- Uso de `async/await` para operações I/O
- Logging estruturado em todas as camadas
- Tratamento de exceções
- Connection string em configuração (não hardcoded)

## Performance

### Otimizações Implementadas
1. **Índices no banco**: Consultas rápidas
2. **Async/Await**: Operações não-bloqueantes
3. **Connection Pooling**: Reutilização de conexões
4. **Retry Policy**: Resiliência em falhas temporárias
5. **Soft Delete**: Evita perda de dados históricos

### Métricas Monitoradas
- Tempo de resposta dos links
- Código HTTP retornado
- Timestamp de cada verificação

## Escalabilidade

### Horizontal
- Stateless: Pode rodar múltiplas instâncias
- Session state no servidor (Blazor Server)

### Vertical
- Queries otimizadas com índices
- Paginação pode ser adicionada facilmente

## Extensibilidade

### Pontos de Extensão
1. **Novos Serviços**: Adicionar via DI
2. **Novos Repositórios**: Implementar interface
3. **Novos Checks**: Estender `HealthCheckerService`
4. **Novos Metadados**: Adicionar propriedades ao modelo

### Exemplos de Extensões Futuras
- Agendamento automático de verificações
- Notificações quando link fica offline
- Exportação de relatórios
- API REST para integração externa
- Autenticação e autorização
- Multi-tenancy

## Tecnologias e Versões

- **.NET**: 8.0
- **Entity Framework Core**: 8.0.0
- **SQL Server**: 2019+
- **HtmlAgilityPack**: 1.11.57
- **Bootstrap**: 5.3.0
- **Bootstrap Icons**: 1.11.0

## Referências

- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Repository Pattern](https://docs.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [Dependency Injection](https://docs.microsoft.com/aspnet/core/fundamentals/dependency-injection)
