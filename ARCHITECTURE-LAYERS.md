# Arquitetura em Camadas - Link Manager

## 📐 Visão Geral da Nova Arquitetura

O projeto Link Manager foi reorganizado seguindo uma arquitetura em **3 camadas** (3-Tier Architecture):

1. **UI (User Interface)** - Camada de Apresentação
2. **BLL (Business Logic Layer)** - Camada de Lógica de Negócio
3. **DAL (Data Access Layer)** - Camada de Acesso a Dados

```
┌─────────────────────────────────────────────────────────────┐
│                      UI LAYER                                │
│                 (Presentation Layer)                         │
│  ┌────────────────────────────────────────────────────┐    │
│  │  Blazor Pages (Index.razor)                        │    │
│  │  Razor Components (NavMenu, MainLayout)            │    │
│  │  - Renderização de UI                              │    │
│  │  - Interação com usuário                           │    │
│  │  - Validação de formulários                        │    │
│  └────────────────────────────────────────────────────┘    │
└────────────────────────┬────────────────────────────────────┘
                         │ Chama
                         ↓
┌─────────────────────────────────────────────────────────────┐
│                      BLL LAYER                               │
│              (Business Logic Layer)                          │
│  ┌────────────────────────────────────────────────────┐    │
│  │  Services (HealthCheckerService)                   │    │
│  │  - Regras de negócio                               │    │
│  │  - Validações complexas                            │    │
│  │  - Processamento de dados                          │    │
│  │  - Integração com APIs externas                    │    │
│  └────────────────────────────────────────────────────┘    │
└────────────────────────┬────────────────────────────────────┘
                         │ Chama
                         ↓
┌─────────────────────────────────────────────────────────────┐
│                      DAL LAYER                               │
│              (Data Access Layer)                             │
│  ┌────────────────────────────────────────────────────┐    │
│  │  Repositories (PageLinkRepository)                 │    │
│  │  DbContext (ApplicationDbContext)                  │    │
│  │  - CRUD operations                                 │    │
│  │  - Queries ao banco                                │    │
│  │  - Entity Framework Core                           │    │
│  └────────────────────────────────────────────────────┘    │
└────────────────────────┬────────────────────────────────────┘
                         │ Acessa
                         ↓
┌─────────────────────────────────────────────────────────────┐
│                      DATABASE                                │
│                    SQL Server                                │
│  - Tabelas                                                   │
│  - Índices                                                   │
│  - Stored Procedures (futuro)                               │
└─────────────────────────────────────────────────────────────┘
```

## 📁 Estrutura de Pastas

```
LinkManager.Web/
├── UI/                          # Camada de Apresentação
│   ├── Pages/                   # Páginas Blazor
│   │   ├── Index.razor          # Página principal
│   │   └── _Host.cshtml         # Host HTML
│   └── Shared/                  # Componentes compartilhados
│       ├── MainLayout.razor     # Layout principal
│       └── NavMenu.razor        # Menu de navegação
│
├── BLL/                         # Camada de Lógica de Negócio
│   ├── IHealthCheckerService.cs # Interface do serviço
│   └── HealthCheckerService.cs  # Implementação do serviço
│
├── DAL/                         # Camada de Acesso a Dados
│   ├── ApplicationDbContext.cs  # Contexto EF Core
│   ├── IPageLinkRepository.cs   # Interface do repositório
│   ├── PageLinkRepository.cs    # Implementação do repositório
│   └── Migrations/              # Migrations do EF Core
│
├── Models/                      # Modelos de Domínio
│   └── PageLink.cs              # Entidade principal
│
├── wwwroot/                     # Arquivos estáticos
│   └── css/                     # Estilos CSS
│
├── Program.cs                   # Configuração da aplicação
├── appsettings.json             # Configurações
└── LinkManager.Web.csproj       # Arquivo do projeto
```

## 🎯 Responsabilidades de Cada Camada

### 1. UI Layer (Camada de Apresentação)

**Localização**: `UI/`

**Responsabilidades**:
- Renderizar interface do usuário
- Capturar entrada do usuário
- Validar dados de formulário (validação básica)
- Exibir mensagens e feedback
- Navegação entre páginas

**Não deve**:
- Acessar diretamente o banco de dados
- Conter lógica de negócio complexa
- Fazer chamadas HTTP diretas (exceto via serviços)

**Exemplo**:
```csharp
// UI/Pages/Index.razor
@inject IPageLinkRepository Repository
@inject IHealthCheckerService HealthChecker

private async Task HandleAddLink()
{
    // 1. Validação básica (UI)
    if (string.IsNullOrWhiteSpace(newLink.Url))
    {
        ShowAlert("URL é obrigatória", "danger");
        return;
    }

    // 2. Chama DAL para salvar
    var addedLink = await Repository.AddAsync(newLink);

    // 3. Chama BLL para processar
    await HealthChecker.CheckAndUpdateAsync(addedLink);
    await Repository.UpdateAsync(addedLink);

    // 4. Atualiza UI
    ShowAlert("Link adicionado com sucesso!", "success");
    await LoadLinks();
}
```

### 2. BLL Layer (Camada de Lógica de Negócio)

**Localização**: `BLL/`

**Responsabilidades**:
- Implementar regras de negócio
- Validações complexas
- Processamento de dados
- Integração com APIs externas
- Orquestração de operações

**Não deve**:
- Acessar diretamente o banco de dados (usa DAL)
- Renderizar UI
- Conter código específico de apresentação

**Exemplo**:
```csharp
// BLL/HealthCheckerService.cs
namespace LinkManager.Web.BLL;

public class HealthCheckerService : IHealthCheckerService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HealthCheckerService> _logger;

    public async Task<HealthCheckResult> CheckHealthAsync(string url)
    {
        // Lógica de negócio: verificar saúde do link
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(30);

        var response = await httpClient.GetAsync(url);
        
        // Processar resposta
        var result = new HealthCheckResult
        {
            IsHealthy = response.IsSuccessStatusCode,
            StatusCode = (int)response.StatusCode,
            // ... extrair metadados
        };

        return result;
    }
}
```

### 3. DAL Layer (Camada de Acesso a Dados)

**Localização**: `DAL/`

**Responsabilidades**:
- CRUD operations
- Queries ao banco de dados
- Gerenciamento de transações
- Mapeamento objeto-relacional (ORM)
- Otimização de queries

**Não deve**:
- Conter lógica de negócio
- Fazer validações de negócio
- Acessar APIs externas

**Exemplo**:
```csharp
// DAL/PageLinkRepository.cs
namespace LinkManager.Web.DAL;

public class PageLinkRepository : IPageLinkRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PageLinkRepository> _logger;

    public async Task<List<PageLink>> GetAllAsync()
    {
        // Apenas acesso a dados, sem lógica de negócio
        return await _context.PageLinks
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<PageLink> AddAsync(PageLink pageLink)
    {
        // Validação de duplicata (regra de dados)
        var existing = await GetByUrlAsync(pageLink.Url);
        if (existing != null)
        {
            throw new InvalidOperationException("URL já existe");
        }

        _context.PageLinks.Add(pageLink);
        await _context.SaveChangesAsync();
        
        return pageLink;
    }
}
```

## 🔄 Fluxo de Dados Completo

### Exemplo: Adicionar um Novo Link

```
1. USUÁRIO
   └─> Preenche formulário na UI
       └─> Clica em "Adicionar Link"

2. UI LAYER (Index.razor)
   └─> HandleAddLink()
       ├─> Validação básica (campo obrigatório)
       └─> Chama DAL: Repository.AddAsync(newLink)

3. DAL LAYER (PageLinkRepository)
   └─> AddAsync(pageLink)
       ├─> Verifica duplicata no banco
       ├─> Insere no banco de dados
       └─> Retorna link com ID gerado

4. UI LAYER (Index.razor)
   └─> Recebe link salvo
       └─> Chama BLL: HealthChecker.CheckAndUpdateAsync(link)

5. BLL LAYER (HealthCheckerService)
   └─> CheckAndUpdateAsync(link)
       ├─> Faz requisição HTTP
       ├─> Extrai metadados HTML
       ├─> Determina status
       └─> Retorna link atualizado

6. UI LAYER (Index.razor)
   └─> Chama DAL: Repository.UpdateAsync(link)

7. DAL LAYER (PageLinkRepository)
   └─> UpdateAsync(link)
       ├─> Atualiza no banco
       └─> Retorna link atualizado

8. UI LAYER (Index.razor)
   └─> Exibe mensagem de sucesso
       └─> Recarrega lista de links
```

## 🎨 Benefícios da Arquitetura em Camadas

### 1. Separação de Responsabilidades (SoC)
Cada camada tem uma responsabilidade clara e bem definida.

### 2. Manutenibilidade
Mudanças em uma camada não afetam as outras (baixo acoplamento).

### 3. Testabilidade
Cada camada pode ser testada independentemente:
- **UI**: Testes de componente
- **BLL**: Testes unitários de lógica de negócio
- **DAL**: Testes de integração com banco

### 4. Reutilização
Serviços (BLL) e repositórios (DAL) podem ser reutilizados em diferentes UIs.

### 5. Escalabilidade
Camadas podem ser escaladas independentemente (ex: múltiplas instâncias de BLL).

## 📝 Convenções de Nomenclatura

### Namespaces

```csharp
// UI Layer
namespace LinkManager.Web.UI.Pages;
namespace LinkManager.Web.UI.Shared;

// BLL Layer
namespace LinkManager.Web.BLL;

// DAL Layer
namespace LinkManager.Web.DAL;

// Models (compartilhado)
namespace LinkManager.Web.Models;
```

### Interfaces

```csharp
// BLL
public interface IHealthCheckerService { }
public class HealthCheckerService : IHealthCheckerService { }

// DAL
public interface IPageLinkRepository { }
public class PageLinkRepository : IPageLinkRepository { }
```

## 🔧 Configuração no Program.cs

```csharp
using LinkManager.Web.DAL;
using LinkManager.Web.BLL;

var builder = WebApplication.CreateBuilder(args);

// UI
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// DAL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IPageLinkRepository, PageLinkRepository>();

// BLL
builder.Services.AddScoped<IHealthCheckerService, HealthCheckerService>();
builder.Services.AddHttpClient();
```

## 🧪 Testando Cada Camada

### Testes de UI (Componentes Blazor)

```csharp
[Fact]
public void Index_ShouldRenderCorrectly()
{
    // Arrange
    using var ctx = new TestContext();
    var mockRepo = new Mock<IPageLinkRepository>();
    ctx.Services.AddSingleton(mockRepo.Object);

    // Act
    var cut = ctx.RenderComponent<Index>();

    // Assert
    cut.Find("h1").TextContent.Should().Contain("Link Manager");
}
```

### Testes de BLL (Lógica de Negócio)

```csharp
[Fact]
public async Task CheckHealthAsync_ShouldReturnOnline_ForValidUrl()
{
    // Arrange
    var service = new HealthCheckerService(httpClientFactory, logger);

    // Act
    var result = await service.CheckHealthAsync("https://www.google.com");

    // Assert
    result.IsHealthy.Should().BeTrue();
    result.StatusCode.Should().Be(200);
}
```

### Testes de DAL (Acesso a Dados)

```csharp
[Fact]
public async Task AddAsync_ShouldAddLink_ToDatabase()
{
    // Arrange
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase("TestDb")
        .Options;
    
    using var context = new ApplicationDbContext(options);
    var repository = new PageLinkRepository(context, logger);
    
    var link = new PageLink { Url = "https://test.com" };

    // Act
    var result = await repository.AddAsync(link);

    // Assert
    result.Id.Should().BeGreaterThan(0);
    context.PageLinks.Count().Should().Be(1);
}
```

## 🚀 Próximos Passos

### Melhorias Futuras

1. **DTOs (Data Transfer Objects)**
   - Criar DTOs para transferência entre camadas
   - Evitar expor entidades de domínio na UI

2. **AutoMapper**
   - Mapear automaticamente entre entidades e DTOs

3. **CQRS (Command Query Responsibility Segregation)**
   - Separar operações de leitura e escrita

4. **Mediator Pattern**
   - Usar MediatR para desacoplar ainda mais as camadas

5. **Unit of Work Pattern**
   - Gerenciar transações complexas

## 📚 Referências

- [Microsoft - Layered Architecture](https://docs.microsoft.com/architecture/guide/architecture-styles/n-tier)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [Repository Pattern](https://docs.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)

---

**Nota**: Esta arquitetura em camadas proporciona uma base sólida para crescimento e manutenção do projeto a longo prazo.
