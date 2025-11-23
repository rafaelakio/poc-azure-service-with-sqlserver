# API Documentation - Link Manager

Este documento descreve as interfaces e métodos disponíveis na aplicação Link Manager.

## 📦 Repositório (Data Access Layer)

### IPageLinkRepository

Interface para operações de dados relacionadas a links.

#### Métodos

##### GetAllAsync()
Obtém todos os links ativos ordenados por data de criação (mais recentes primeiro).

```csharp
Task<List<PageLink>> GetAllAsync()
```

**Retorno**: Lista de todos os links ativos

**Exemplo**:
```csharp
var links = await _repository.GetAllAsync();
```

---

##### GetByIdAsync(int id)
Obtém um link específico por ID.

```csharp
Task<PageLink?> GetByIdAsync(int id)
```

**Parâmetros**:
- `id`: ID do link

**Retorno**: Link encontrado ou `null`

**Exemplo**:
```csharp
var link = await _repository.GetByIdAsync(1);
if (link != null)
{
    Console.WriteLine($"Link: {link.Url}");
}
```

---

##### GetByUrlAsync(string url)
Obtém um link por URL (útil para evitar duplicatas).

```csharp
Task<PageLink?> GetByUrlAsync(string url)
```

**Parâmetros**:
- `url`: URL completa do link

**Retorno**: Link encontrado ou `null`

**Exemplo**:
```csharp
var existing = await _repository.GetByUrlAsync("https://www.google.com");
if (existing != null)
{
    throw new Exception("Link já existe!");
}
```

---

##### AddAsync(PageLink pageLink)
Adiciona um novo link ao banco de dados.

```csharp
Task<PageLink> AddAsync(PageLink pageLink)
```

**Parâmetros**:
- `pageLink`: Objeto PageLink a ser adicionado

**Retorno**: Link adicionado com ID gerado

**Exceções**:
- `InvalidOperationException`: Se já existe um link com a mesma URL

**Exemplo**:
```csharp
var newLink = new PageLink
{
    Url = "https://www.example.com",
    Category = "Example",
    Notes = "Site de exemplo"
};

var added = await _repository.AddAsync(newLink);
Console.WriteLine($"Link adicionado com ID: {added.Id}");
```

---

##### UpdateAsync(PageLink pageLink)
Atualiza um link existente.

```csharp
Task<PageLink> UpdateAsync(PageLink pageLink)
```

**Parâmetros**:
- `pageLink`: Objeto PageLink com dados atualizados

**Retorno**: Link atualizado

**Exceções**:
- `InvalidOperationException`: Se o link não existe

**Exemplo**:
```csharp
var link = await _repository.GetByIdAsync(1);
link.Title = "Novo Título";
link.Status = LinkStatus.Online;

await _repository.UpdateAsync(link);
```

---

##### DeleteAsync(int id)
Remove um link (soft delete - marca como inativo).

```csharp
Task DeleteAsync(int id)
```

**Parâmetros**:
- `id`: ID do link a ser removido

**Exceções**:
- `InvalidOperationException`: Se o link não existe

**Exemplo**:
```csharp
await _repository.DeleteAsync(1);
```

---

##### GetByCategoryAsync(string category)
Busca links por categoria.

```csharp
Task<List<PageLink>> GetByCategoryAsync(string category)
```

**Parâmetros**:
- `category`: Nome da categoria

**Retorno**: Lista de links da categoria

**Exemplo**:
```csharp
var techLinks = await _repository.GetByCategoryAsync("Technology");
```

---

##### GetByStatusAsync(string status)
Busca links por status.

```csharp
Task<List<PageLink>> GetByStatusAsync(string status)
```

**Parâmetros**:
- `status`: Status do link (Online, Offline, Pending, etc)

**Retorno**: Lista de links com o status especificado

**Exemplo**:
```csharp
var offlineLinks = await _repository.GetByStatusAsync(LinkStatus.Offline);
```

---

##### GetLinksNeedingCheckAsync(int hoursThreshold = 24)
Obtém links que precisam de verificação (não verificados há X horas).

```csharp
Task<List<PageLink>> GetLinksNeedingCheckAsync(int hoursThreshold = 24)
```

**Parâmetros**:
- `hoursThreshold`: Número de horas desde a última verificação (padrão: 24)

**Retorno**: Lista de links que precisam verificação

**Exemplo**:
```csharp
// Links não verificados nas últimas 12 horas
var needsCheck = await _repository.GetLinksNeedingCheckAsync(12);

foreach (var link in needsCheck)
{
    await _healthChecker.CheckAndUpdateAsync(link);
    await _repository.UpdateAsync(link);
}
```

---

## 🏥 Health Checker Service

### IHealthCheckerService

Interface para verificação de saúde de links.

#### Métodos

##### CheckHealthAsync(string url)
Verifica a saúde de uma URL e extrai metadados.

```csharp
Task<HealthCheckResult> CheckHealthAsync(string url)
```

**Parâmetros**:
- `url`: URL a ser verificada

**Retorno**: Objeto `HealthCheckResult` com informações da verificação

**Exemplo**:
```csharp
var result = await _healthChecker.CheckHealthAsync("https://www.google.com");

Console.WriteLine($"Status: {result.IsHealthy}");
Console.WriteLine($"HTTP Code: {result.StatusCode}");
Console.WriteLine($"Response Time: {result.ResponseTimeMs}ms");
Console.WriteLine($"Title: {result.Title}");
Console.WriteLine($"Description: {result.Description}");
```

---

##### CheckAndUpdateAsync(PageLink pageLink)
Verifica a saúde de um PageLink e atualiza seus dados.

```csharp
Task<PageLink> CheckAndUpdateAsync(PageLink pageLink)
```

**Parâmetros**:
- `pageLink`: Link a ser verificado

**Retorno**: PageLink atualizado com dados da verificação

**Exemplo**:
```csharp
var link = await _repository.GetByIdAsync(1);
await _healthChecker.CheckAndUpdateAsync(link);
await _repository.UpdateAsync(link);

Console.WriteLine($"Status atualizado: {link.Status}");
```

---

## 📊 Modelos de Dados

### PageLink

Entidade principal que representa um link.

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

**Validações**:
- `Url`: Obrigatório, máximo 2000 caracteres, formato URL válido
- `Title`: Opcional, máximo 500 caracteres
- `Description`: Opcional, máximo 1000 caracteres
- `Status`: Obrigatório, máximo 50 caracteres
- `Category`: Opcional, máximo 100 caracteres
- `Notes`: Opcional, máximo 2000 caracteres

---

### HealthCheckResult

Resultado da verificação de saúde de um link.

```csharp
public class HealthCheckResult
{
    public bool IsHealthy { get; set; }
    public int? StatusCode { get; set; }
    public long ResponseTimeMs { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CheckedAt { get; set; }
}
```

---

### LinkStatus

Constantes para status de links.

```csharp
public static class LinkStatus
{
    public const string Pending = "Pending";   // Ainda não verificado
    public const string Online = "Online";     // HTTP 2xx
    public const string Offline = "Offline";   // HTTP 4xx/5xx
    public const string Error = "Error";       // Erro de rede
    public const string Timeout = "Timeout";   // Timeout na requisição
}
```

---

## 🔄 Fluxos de Uso Comuns

### Adicionar e Verificar Link

```csharp
// 1. Criar novo link
var newLink = new PageLink
{
    Url = "https://www.example.com",
    Category = "Example"
};

// 2. Adicionar ao banco
var added = await _repository.AddAsync(newLink);

// 3. Verificar saúde
await _healthChecker.CheckAndUpdateAsync(added);

// 4. Atualizar com dados da verificação
await _repository.UpdateAsync(added);
```

---

### Verificar Links Pendentes

```csharp
// Buscar links que precisam verificação
var needsCheck = await _repository.GetLinksNeedingCheckAsync(24);

foreach (var link in needsCheck)
{
    try
    {
        // Verificar saúde
        await _healthChecker.CheckAndUpdateAsync(link);
        
        // Atualizar no banco
        await _repository.UpdateAsync(link);
        
        _logger.LogInformation($"Link verificado: {link.Url} - Status: {link.Status}");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Erro ao verificar link: {link.Url}");
    }
}
```

---

### Obter Estatísticas

```csharp
// Buscar todos os links
var allLinks = await _repository.GetAllAsync();

// Calcular estatísticas
var stats = new
{
    Total = allLinks.Count,
    Online = allLinks.Count(l => l.Status == LinkStatus.Online),
    Offline = allLinks.Count(l => l.Status == LinkStatus.Offline),
    Pending = allLinks.Count(l => l.Status == LinkStatus.Pending),
    AverageResponseTime = allLinks
        .Where(l => l.ResponseTimeMs.HasValue)
        .Average(l => l.ResponseTimeMs.Value)
};

Console.WriteLine($"Total: {stats.Total}");
Console.WriteLine($"Online: {stats.Online}");
Console.WriteLine($"Offline: {stats.Offline}");
Console.WriteLine($"Pending: {stats.Pending}");
Console.WriteLine($"Avg Response Time: {stats.AverageResponseTime}ms");
```

---

### Filtrar por Categoria

```csharp
// Buscar links de uma categoria
var techLinks = await _repository.GetByCategoryAsync("Technology");

// Agrupar por status
var grouped = techLinks.GroupBy(l => l.Status);

foreach (var group in grouped)
{
    Console.WriteLine($"{group.Key}: {group.Count()} links");
}
```

---

## 🔒 Tratamento de Erros

### Exceções Comuns

#### InvalidOperationException
Lançada quando:
- Tentativa de adicionar link com URL duplicada
- Tentativa de atualizar/deletar link inexistente

```csharp
try
{
    await _repository.AddAsync(newLink);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Erro: {ex.Message}");
    // Tratar duplicata
}
```

#### HttpRequestException
Lançada quando há erro de rede no health check.

```csharp
try
{
    await _healthChecker.CheckHealthAsync(url);
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Erro de rede: {ex.Message}");
}
```

#### TaskCanceledException
Lançada quando a requisição HTTP excede o timeout (30 segundos).

```csharp
try
{
    await _healthChecker.CheckHealthAsync(url);
}
catch (TaskCanceledException)
{
    Console.WriteLine("Timeout: Requisição excedeu 30 segundos");
}
```

---

## 📝 Boas Práticas

### 1. Sempre use async/await
```csharp
// ✓ Correto
var links = await _repository.GetAllAsync();

// ✗ Evite
var links = _repository.GetAllAsync().Result;
```

### 2. Trate exceções apropriadamente
```csharp
try
{
    await _repository.AddAsync(link);
}
catch (InvalidOperationException ex)
{
    _logger.LogWarning(ex, "Link duplicado");
    // Tratar erro
}
catch (Exception ex)
{
    _logger.LogError(ex, "Erro inesperado");
    throw;
}
```

### 3. Use logging
```csharp
_logger.LogInformation("Adicionando link: {Url}", link.Url);
await _repository.AddAsync(link);
_logger.LogInformation("Link adicionado com ID: {Id}", link.Id);
```

### 4. Valide dados antes de salvar
```csharp
if (string.IsNullOrWhiteSpace(link.Url))
{
    throw new ArgumentException("URL é obrigatória");
}

if (!Uri.TryCreate(link.Url, UriKind.Absolute, out _))
{
    throw new ArgumentException("URL inválida");
}
```

---

## 🧪 Exemplos de Testes

### Teste de Repositório

```csharp
[Fact]
public async Task AddAsync_ShouldAddLink()
{
    // Arrange
    var link = new PageLink
    {
        Url = "https://test.com",
        Category = "Test"
    };

    // Act
    var result = await _repository.AddAsync(link);

    // Assert
    Assert.NotEqual(0, result.Id);
    Assert.Equal("https://test.com", result.Url);
}
```

### Teste de Health Checker

```csharp
[Fact]
public async Task CheckHealthAsync_ShouldReturnOnlineForValidUrl()
{
    // Arrange
    var url = "https://www.google.com";

    // Act
    var result = await _healthChecker.CheckHealthAsync(url);

    // Assert
    Assert.True(result.IsHealthy);
    Assert.Equal(200, result.StatusCode);
    Assert.NotNull(result.Title);
}
```

---

## 📚 Referências

- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Repository Pattern](https://docs.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [Async/Await Best Practices](https://docs.microsoft.com/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)
