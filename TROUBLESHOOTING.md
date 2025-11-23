# Troubleshooting & FAQ - Link Manager

Guia de solução de problemas comuns e perguntas frequentes.

## 🔧 Problemas Comuns

### 1. Erro: "Cannot open database"

**Sintoma**:
```
Cannot open database "LinkManagerDb" requested by the login.
The login failed.
```

**Causas Possíveis**:
- SQL Server não está rodando
- Banco de dados não foi criado
- Permissões incorretas

**Soluções**:

1. **Verificar se SQL Server está rodando**:
   ```bash
   # Windows
   sqllocaldb info
   sqllocaldb start mssqllocaldb
   
   # SQL Server completo
   services.msc → SQL Server (MSSQLSERVER)
   ```

2. **Aplicar migrations**:
   ```bash
   cd LinkManager.Web
   dotnet ef database update
   ```

3. **Verificar connection string**:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LinkManagerDb;Trusted_Connection=True;TrustServerCertificate=True"
     }
   }
   ```

---

### 2. Erro: "A network-related error occurred"

**Sintoma**:
```
A network-related or instance-specific error occurred while establishing
a connection to SQL Server.
```

**Soluções**:

1. **Adicionar TrustServerCertificate**:
   ```json
   "DefaultConnection": "Server=...;TrustServerCertificate=True"
   ```

2. **Verificar firewall**:
   - Libere porta 1433 (SQL Server)
   - Desabilite temporariamente para teste

3. **Verificar nome do servidor**:
   ```bash
   # Listar instâncias SQL Server
   sqllocaldb info
   
   # Testar conexão
   sqlcmd -S (localdb)\mssqllocaldb -Q "SELECT @@VERSION"
   ```

---

### 3. Erro: "The certificate chain was issued by an authority that is not trusted"

**Sintoma**:
```
The certificate chain was issued by an authority that is not trusted
```

**Solução**:
Adicione `TrustServerCertificate=True` na connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;TrustServerCertificate=True;..."
  }
}
```

---

### 4. Erro: "dotnet ef command not found"

**Sintoma**:
```
'dotnet-ef' is not recognized as an internal or external command
```

**Solução**:
Instale o Entity Framework Tools:

```bash
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

Verifique instalação:
```bash
dotnet ef --version
```

---

### 5. Erro: "Duplicate key error" ao adicionar link

**Sintoma**:
```
Violation of UNIQUE KEY constraint 'IX_PageLinks_Url'.
Cannot insert duplicate key.
```

**Causa**:
Já existe um link com a mesma URL.

**Solução**:
A aplicação já trata isso. Se ocorrer, verifique:

```csharp
var existing = await _repository.GetByUrlAsync(url);
if (existing != null)
{
    // Link já existe
    throw new InvalidOperationException("Link já cadastrado");
}
```

---

### 6. Porta já em uso

**Sintoma**:
```
Failed to bind to address https://localhost:5001: address already in use
```

**Soluções**:

1. **Matar processo na porta**:
   ```bash
   # Windows
   netstat -ano | findstr :5001
   taskkill /PID <PID> /F
   
   # Linux/Mac
   lsof -ti:5001 | xargs kill -9
   ```

2. **Mudar porta**:
   Edite `Properties/launchSettings.json`:
   ```json
   {
     "applicationUrl": "https://localhost:5002;http://localhost:5001"
   }
   ```

---

### 7. Erro: "Timeout expired" no Health Check

**Sintoma**:
Health check demora muito e retorna timeout.

**Causa**:
Site está lento ou inacessível.

**Solução**:
O timeout padrão é 30 segundos. Para ajustar:

```csharp
// Em HealthCheckerService.cs
httpClient.Timeout = TimeSpan.FromSeconds(60); // Aumentar timeout
```

---

### 8. Migrations não aplicadas automaticamente

**Sintoma**:
Aplicação inicia mas tabelas não existem.

**Causa**:
Auto-migration só funciona em Development.

**Solução**:

1. **Aplicar manualmente**:
   ```bash
   dotnet ef database update
   ```

2. **Forçar em produção** (não recomendado):
   ```csharp
   // Em Program.cs, remover verificação de ambiente
   using var scope = app.Services.CreateScope();
   var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
   await dbContext.Database.MigrateAsync();
   ```

---

### 9. Erro: "HtmlAgilityPack not found"

**Sintoma**:
```
The type or namespace name 'HtmlAgilityPack' could not be found
```

**Solução**:
Restaure os pacotes NuGet:

```bash
dotnet restore
dotnet build
```

Se persistir, instale manualmente:
```bash
dotnet add package HtmlAgilityPack
```

---

### 10. Performance lenta

**Sintomas**:
- Listagem de links demora
- Health checks lentos
- Aplicação travando

**Soluções**:

1. **Verificar índices**:
   ```sql
   SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('PageLinks');
   ```

2. **Atualizar estatísticas**:
   ```sql
   UPDATE STATISTICS PageLinks WITH FULLSCAN;
   ```

3. **Verificar queries lentas**:
   ```sql
   SELECT TOP 10 * FROM sys.dm_exec_query_stats
   ORDER BY total_elapsed_time DESC;
   ```

4. **Adicionar paginação**:
   ```csharp
   var links = await _context.PageLinks
       .Where(p => p.IsActive)
       .OrderByDescending(p => p.CreatedAt)
       .Skip(page * pageSize)
       .Take(pageSize)
       .ToListAsync();
   ```

---

## ❓ FAQ (Perguntas Frequentes)

### Como adicionar autenticação?

1. Instale pacotes:
   ```bash
   dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
   ```

2. Configure no `Program.cs`:
   ```csharp
   builder.Services.AddAuthentication();
   builder.Services.AddAuthorization();
   ```

3. Adicione `[Authorize]` nas páginas.

---

### Como adicionar paginação?

```csharp
// No repositório
public async Task<PagedResult<PageLink>> GetPagedAsync(int page, int pageSize)
{
    var total = await _context.PageLinks.CountAsync(p => p.IsActive);
    
    var items = await _context.PageLinks
        .Where(p => p.IsActive)
        .OrderByDescending(p => p.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return new PagedResult<PageLink>
    {
        Items = items,
        TotalCount = total,
        Page = page,
        PageSize = pageSize
    };
}
```

---

### Como agendar verificações automáticas?

Use um Background Service:

```csharp
public class HealthCheckBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Verificar links a cada 1 hora
            await CheckLinksAsync();
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
    
    private async Task CheckLinksAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPageLinkRepository>();
        var healthChecker = scope.ServiceProvider.GetRequiredService<IHealthCheckerService>();
        
        var links = await repository.GetLinksNeedingCheckAsync(24);
        
        foreach (var link in links)
        {
            await healthChecker.CheckAndUpdateAsync(link);
            await repository.UpdateAsync(link);
        }
    }
}

// Registrar no Program.cs
builder.Services.AddHostedService<HealthCheckBackgroundService>();
```

---

### Como exportar dados para CSV?

```csharp
public async Task<string> ExportToCsvAsync()
{
    var links = await _repository.GetAllAsync();
    
    var csv = new StringBuilder();
    csv.AppendLine("URL,Title,Status,Category,LastChecked");
    
    foreach (var link in links)
    {
        csv.AppendLine($"\"{link.Url}\",\"{link.Title}\",{link.Status},{link.Category},{link.LastCheckedAt}");
    }
    
    return csv.ToString();
}
```

---

### Como adicionar notificações por email?

1. Instale pacote:
   ```bash
   dotnet add package MailKit
   ```

2. Configure serviço:
   ```csharp
   public class EmailService
   {
       public async Task SendAlertAsync(PageLink link)
       {
           var message = new MimeMessage();
           message.From.Add(new MailboxAddress("Link Manager", "noreply@linkmanager.com"));
           message.To.Add(new MailboxAddress("Admin", "admin@example.com"));
           message.Subject = $"Link Offline: {link.Url}";
           message.Body = new TextPart("plain")
           {
               Text = $"O link {link.Url} está offline.\nStatus: {link.Status}\nErro: {link.ErrorMessage}"
           };
           
           using var client = new SmtpClient();
           await client.ConnectAsync("smtp.gmail.com", 587, false);
           await client.AuthenticateAsync("user@gmail.com", "password");
           await client.SendAsync(message);
           await client.DisconnectAsync(true);
       }
   }
   ```

---

### Como adicionar API REST?

1. Adicione controllers:
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class LinksController : ControllerBase
   {
       private readonly IPageLinkRepository _repository;
       
       [HttpGet]
       public async Task<ActionResult<List<PageLink>>> GetAll()
       {
           var links = await _repository.GetAllAsync();
           return Ok(links);
       }
       
       [HttpGet("{id}")]
       public async Task<ActionResult<PageLink>> GetById(int id)
       {
           var link = await _repository.GetByIdAsync(id);
           if (link == null) return NotFound();
           return Ok(link);
       }
       
       [HttpPost]
       public async Task<ActionResult<PageLink>> Create(PageLink link)
       {
           var created = await _repository.AddAsync(link);
           return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
       }
   }
   ```

2. Configure no `Program.cs`:
   ```csharp
   builder.Services.AddControllers();
   app.MapControllers();
   ```

---

### Como usar Docker?

Crie `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LinkManager.Web/LinkManager.Web.csproj", "LinkManager.Web/"]
RUN dotnet restore "LinkManager.Web/LinkManager.Web.csproj"
COPY . .
WORKDIR "/src/LinkManager.Web"
RUN dotnet build "LinkManager.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LinkManager.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LinkManager.Web.dll"]
```

Build e run:
```bash
docker build -t linkmanager .
docker run -p 8080:80 linkmanager
```

---

### Como fazer backup do banco?

```bash
# Via SQL Server Management Studio
# Botão direito no banco → Tasks → Backup

# Via T-SQL
BACKUP DATABASE [LinkManagerDb]
TO DISK = 'C:\Backups\LinkManagerDb.bak'
WITH FORMAT, INIT, NAME = 'Full Backup';

# Via Azure CLI (Azure SQL)
az sql db export \
  --resource-group myResourceGroup \
  --server myServer \
  --name LinkManagerDb \
  --admin-user sqladmin \
  --admin-password <password> \
  --storage-key <storage-key> \
  --storage-key-type StorageAccessKey \
  --storage-uri https://mystorageaccount.blob.core.windows.net/backups/LinkManagerDb.bacpac
```

---

### Como monitorar a aplicação?

1. **Application Insights** (Azure):
   ```csharp
   builder.Services.AddApplicationInsightsTelemetry();
   ```

2. **Serilog** (Logging estruturado):
   ```bash
   dotnet add package Serilog.AspNetCore
   ```
   
   ```csharp
   builder.Host.UseSerilog((context, config) =>
   {
       config.ReadFrom.Configuration(context.Configuration);
   });
   ```

3. **Health Checks**:
   ```csharp
   builder.Services.AddHealthChecks()
       .AddDbContextCheck<ApplicationDbContext>();
   
   app.MapHealthChecks("/health");
   ```

---

## 📞 Suporte

### Logs da Aplicação

Verifique os logs em:
- **Console**: Durante execução
- **Debug Output**: Visual Studio
- **Arquivo**: Configure em `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Ferramentas Úteis

- **SQL Server Management Studio**: Gerenciar banco
- **Azure Data Studio**: Cliente SQL multiplataforma
- **Postman**: Testar APIs
- **Browser DevTools**: Debug frontend

### Recursos Adicionais

- [Documentação .NET](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/blazor)

### Reportar Bugs

Ao reportar bugs, inclua:
1. Descrição do problema
2. Passos para reproduzir
3. Mensagem de erro completa
4. Versão do .NET e SQL Server
5. Sistema operacional
6. Logs relevantes

---

## 🔍 Debug Tips

### Habilitar Logging Detalhado

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### Ver Queries SQL Geradas

```csharp
// Em appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### Breakpoints Condicionais

No Visual Studio:
1. Clique com botão direito no breakpoint
2. Conditions → Conditional Expression
3. Exemplo: `link.Status == "Offline"`

---

Esperamos que este guia ajude a resolver os problemas mais comuns. Se precisar de mais ajuda, consulte a documentação completa ou abra uma issue no repositório!
