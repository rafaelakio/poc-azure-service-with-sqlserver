# Arquitetura da Aplicação Link Manager

## Visão Geral

A aplicação Link Manager segue uma arquitetura em camadas baseada no padrão MVC (Model-View-Controller) com uma camada adicional de acesso a dados (DAL - Data Access Layer).

## Diagrama de Arquitetura

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                        │
│                   (Blazor Server UI)                         │
│                                                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │ Index.razor  │  │ MainLayout   │  │  NavMenu     │     │
│  │ (View/Page)  │  │   .razor     │  │   .razor     │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ Dependency Injection
                         │
┌────────────────────────▼────────────────────────────────────┐
│                   BUSINESS LOGIC LAYER                       │
│                      (Services)                              │
│                                                              │
│  ┌──────────────────────────────────────────────────┐      │
│  │         IHealthCheckerService                     │      │
│  │                    ▲                              │      │
│  │                    │                              │      │
│  │         HealthCheckerService                      │      │
│  │  - CheckHealthAsync()                             │      │
│  │  - ExtractMetadata()                              │      │
│  │  - DetermineStatus()                              │      │
│  └──────────────────────────────────────────────────┘      │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ Uses
                         │
┌────────────────────────▼────────────────────────────────────┐
│              DATA ACCESS LAYER (DAL)                         │
│                   (Repository Pattern)                       │
│                                                              │
│  ┌──────────────────────────────────────────────────┐      │
│  │         IPageLinkRepository                       │      │
│  │                    ▲                              │      │
│  │                    │                              │      │
│  │         PageLinkRepository                        │      │
│  │  - GetAllAsync()                                  │      │
│  │  - GetByIdAsync()                                 │      │
│  │  - AddAsync()                                     │      │
│  │  - UpdateAsync()                                  │      │
│  │  - DeleteAsync()                                  │      │
│  └──────────────────────────────────────────────────┘      │
│                         │                                    │
│                         │ Uses                               │
│                         ▼                                    │
│  ┌──────────────────────────────────────────────────┐      │
│  │       ApplicationDbContext                        │      │
│  │         (Entity Framework Core)                   │      │
│  │  - DbSet<PageLink>                                │      │
│  │  - OnModelCreating()                              │      │
│  └──────────────────────────────────────────────────┘      │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ SQL Queries
                         │
┌────────────────────────▼────────────────────────────────────┐
│                   DATABASE LAYER                             │
│                    (SQL Server)                              │
│                                                              │
│  ┌──────────────────────────────────────────────────┐      │
│  │              PageLinks Table                      │      │
│  │  - Id (PK)                                        │      │
│  │  - Url (Unique Index)                             │      │
│  │  - Title                                          │      │
│  │  - Description                                    │      │
│  │  - Status (Index)                                 │      │
│  │  - HttpStatusCode                                 │      │
│  │  - ResponseTimeMs                                 │      │
│  │  - CreatedAt (Index)                              │      │
│  │  - LastCheckedAt                                  │      │
│  │  - ErrorMessage                                   │      │
│  │  - Category (Index)                               │      │
│  │  - Notes                                          │      │
│  │  - IsActive                                       │      │
│  └──────────────────────────────────────────────────┘      │
└─────────────────────────────────────────────────────────────┘
```

## Camadas Detalhadas

### 1. Presentation Layer (Blazor Server UI)

**Responsabilidade**: Interface do usuário e interação

**Componentes**:
- **Index.razor**: Página principal com CRUD completo
- **MainLayout.razor**: Layout base da aplicação
- **NavMenu.razor**: Menu de navegação
- **_Host.cshtml**: Página host do Blazor Server

**Características**:
- Renderização no servidor (Blazor Server)
- Comunicação via SignalR
- Componentes reativos
- Validação de formulários

### 2. Business Logic Layer (Services)

**Responsabilidade**: Lógica de negócio e regras

#### HealthCheckerService

```csharp
public class HealthCheckerService : IHealthCheckerService
{
    // Verifica saúde do link
    Task<HealthCheckResult> CheckHealthAsync(string url)
    
    // Atualiza PageLink com resultado
    Task<PageLink> CheckAndUpdateAsync(PageLink pageLink)
    
    // Extrai metadados HTML
    void ExtractMetadata(string html, HealthCheckResult result)
    
    // Determina status baseado em resultado
    string DetermineStatus(HealthCheckResult result)
}
```

**Funcionalidades**:
- Requisições HTTP com timeout
- Extração de metadados HTML (título, descrição)
- Medição de tempo de resposta
- Tratamento de erros e timeouts
- Suporte a Open Graph tags

### 3. Data Access Layer (Repository Pattern)

**Responsabilidade**: Acesso e manipulação de dados

#### IPageLinkRepository (Interface)

```csharp
public interface IPageLinkRepository
{
    Task<List<PageLink>> GetAllAsync();
    Task<PageLink?> GetByIdAsync(int id);
    Task<PageLink?> GetByUrlAsync(string url);
    Task<PageLink> AddAsync(PageLink pageLink);
    Task<PageLink> UpdateAsync(PageLink pageLink);
    Task DeleteAsync(int id);
    Task<List<PageLink>> GetByCategoryAsync(string category);
    Task<List<PageLink>> GetByStatusAsync(string status);
    Task<List<PageLink>> GetLinksNeedingCheckAsync(int hoursThreshold);
}
```

#### PageLinkRepository (Implementação)

**Características**:
- Usa Entity Framework Core
- Implementa soft delete (IsActive flag)
- Validação de duplicatas
- Logging de operações
- Queries otimizadas com índices

#### ApplicationDbContext

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<PageLink> PageLinks { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuração de índices
        // Configuração de relacionamentos
        // Seed data
    }
}
```

### 4. Model Layer

#### PageLink (Entidade)

```csharp
public class PageLink
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; }
    public int? HttpStatusCode { get; set; }
    public long? ResponseTimeMs { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastCheckedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Category { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
}
```

## Fluxo de Dados

### Fluxo de Adição de Link

```
1. Usuário preenche formulário (Index.razor)
   ↓
2. HandleAddLink() é chamado
   ↓
3. Repository.AddAsync(newLink)
   ↓
4. Entity Framework insere no banco
   ↓
5. HealthChecker.CheckAndUpdateAsync(link)
   ↓
6. HTTP Request para URL
   ↓
7. Extração de metadados HTML
   ↓
8. Repository.UpdateAsync(link)
   ↓
9. Entity Framework atualiza registro
   ↓
10. UI é atualizada (StateHasChanged)
```

### Fluxo de Health Check

```
1. Usuário clica em "Verificar Saúde"
   ↓
2. CheckHealth(link) é chamado
   ↓
3. HealthChecker.CheckHealthAsync(url)
   ↓
4. HttpClient.GetAsync(url)
   ├─ Sucesso (200-299)
   │  ├─ Extrai HTML
   │  ├─ Parse com HtmlAgilityPack
   │  ├─ Extrai <title>
   │  ├─ Extrai <meta name="description">
   │  └─ Extrai Open Graph tags
   │
   ├─ Erro (400-599)
   │  └─ Armazena código e mensagem
   │
   └─ Timeout/Exception
      └─ Armazena erro
   ↓
5. Atualiza PageLink com resultados
   ↓
6. Repository.UpdateAsync(link)
   ↓
7. UI é atualizada
```

## Padrões de Design Utilizados

### 1. Repository Pattern

**Objetivo**: Abstrair acesso a dados

**Benefícios**:
- Desacoplamento entre lógica de negócio e dados
- Facilita testes unitários (mock do repositório)
- Centraliza queries do banco
- Permite trocar implementação sem afetar outras camadas

### 2. Dependency Injection

**Objetivo**: Inversão de controle

**Implementação**:
```csharp
// Program.cs
builder.Services.AddScoped<IPageLinkRepository, PageLinkRepository>();
builder.Services.AddScoped<IHealthCheckerService, HealthCheckerService>();
```

**Benefícios**:
- Baixo acoplamento
- Facilita testes
- Gerenciamento automático de ciclo de vida

### 3. Unit of Work (via DbContext)

**Objetivo**: Gerenciar transações

**Implementação**: Entity Framework DbContext

**Benefícios**:
- Transações automáticas
- Change tracking
- Otimização de queries

### 4. Soft Delete

**Objetivo**: Não remover dados fisicamente

**Implementação**:
```csharp
public async Task DeleteAsync(int id)
{
    var pageLink = await _context.PageLinks.FindAsync(id);
    pageLink.IsActive = false;
    await _context.SaveChangesAsync();
}
```

**Benefícios**:
- Auditoria
- Recuperação de dados
- Histórico completo

## Segurança

### 1. SQL Injection

**Proteção**: Entity Framework parametriza queries automaticamente

### 2. XSS (Cross-Site Scripting)

**Proteção**: Blazor escapa HTML automaticamente

### 3. HTTPS

**Implementação**: Configurado em `Program.cs`

```csharp
app.UseHttpsRedirection();
```

### 4. Connection String

**Proteção**: Armazenada em `appsettings.json` (não commitada)

## Performance

### 1. Índices no Banco

```csharp
entity.HasIndex(e => e.Url).IsUnique();
entity.HasIndex(e => e.Status);
entity.HasIndex(e => e.CreatedAt);
entity.HasIndex(e => e.Category);
```

### 2. Async/Await

Todas as operações de I/O são assíncronas:
- Database queries
- HTTP requests
- File operations

### 3. Connection Pooling

Entity Framework gerencia pool de conexões automaticamente

### 4. Retry Logic

```csharp
options.UseSqlServer(
    connectionString,
    sqlOptions => sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: null));
```

## Escalabilidade

### Horizontal Scaling

- Blazor Server suporta múltiplas instâncias
- SignalR pode usar Azure SignalR Service
- SQL Server pode usar réplicas de leitura

### Vertical Scaling

- Aumentar recursos do App Service
- Aumentar tier do SQL Server
- Adicionar cache (Redis)

## Monitoramento

### Logging

```csharp
_logger.LogInformation("Buscando todos os links");
_logger.LogError(ex, "Erro ao carregar links");
```

### Application Insights (Azure)

- Telemetria automática
- Rastreamento de exceções
- Métricas de performance

## Extensibilidade

### Adicionar Novo Serviço

1. Criar interface em `Services/`
2. Implementar serviço
3. Registrar em `Program.cs`
4. Injetar onde necessário

### Adicionar Nova Entidade

1. Criar modelo em `Models/`
2. Adicionar DbSet em `ApplicationDbContext`
3. Criar migration
4. Criar repositório
5. Atualizar UI

## Testes

### Testes Unitários

```csharp
[Fact]
public async Task AddAsync_ShouldAddLink()
{
    // Arrange
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDb")
        .Options;
    
    using var context = new ApplicationDbContext(options);
    var repository = new PageLinkRepository(context, logger);
    
    // Act
    var link = await repository.AddAsync(new PageLink { Url = "https://test.com" });
    
    // Assert
    Assert.NotNull(link);
    Assert.True(link.Id > 0);
}
```

### Testes de Integração

```csharp
[Fact]
public async Task HealthChecker_ShouldExtractMetadata()
{
    // Arrange
    var service = new HealthCheckerService(httpClientFactory, logger);
    
    // Act
    var result = await service.CheckHealthAsync("https://www.google.com");
    
    // Assert
    Assert.True(result.IsHealthy);
    Assert.NotNull(result.Title);
}
```

## Manutenção

### Migrations

```bash
# Criar migration
dotnet ef migrations add NomeDaMigration

# Aplicar migration
dotnet ef database update

# Reverter migration
dotnet ef database update PreviousMigration
```

### Backup

```sql
-- Backup do banco
BACKUP DATABASE LinkManagerDb 
TO DISK = 'C:\Backup\LinkManagerDb.bak'
```

## Referências

- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Repository Pattern](https://docs.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
