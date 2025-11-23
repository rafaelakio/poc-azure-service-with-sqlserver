# 🚀 Guia de Início Rápido - Link Manager

Este guia vai te ajudar a executar a aplicação localmente em menos de 5 minutos.

## ⚡ Início Rápido (Local)

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) (já vem com Visual Studio)

### Passos

1. **Clone o repositório**
   ```bash
   git clone <url-do-repositorio>
   cd poc-azure-service-with-sqlserver
   ```

2. **Restaure os pacotes**
   ```bash
   cd LinkManager.Web
   dotnet restore
   ```

3. **Execute a aplicação**
   ```bash
   dotnet run
   ```

4. **Acesse no navegador**
   ```
   https://localhost:5001
   ```

Pronto! A aplicação criará o banco de dados automaticamente na primeira execução.

## 📝 Testando a Aplicação

### 1. Adicionar um Link

1. Na página inicial, preencha o formulário:
   - **URL**: `https://www.google.com`
   - **Categoria**: `Search Engine`
   - **Notas**: `Mecanismo de busca`

2. Clique em **Adicionar Link**

3. A aplicação irá:
   - Verificar se o link está online
   - Extrair o título da página
   - Extrair a descrição
   - Salvar no banco de dados

### 2. Verificar Saúde de um Link

1. Na lista de links, clique no botão **❤️ (Health Check)**
2. A aplicação verificará o status atual do link
3. Os dados serão atualizados automaticamente

### 3. Ver Detalhes

1. Clique no botão **👁️ (Ver Detalhes)**
2. Uma modal mostrará todas as informações do link:
   - Título e descrição
   - Status HTTP
   - Tempo de resposta
   - Histórico de verificações

### 4. Excluir um Link

1. Clique no botão **🗑️ (Excluir)**
2. O link será removido (soft delete)

## 🎯 Funcionalidades Principais

### Dashboard
- **Total de Links**: Quantidade total cadastrada
- **Online**: Links funcionando (HTTP 2xx)
- **Offline**: Links com erro (HTTP 4xx/5xx)
- **Pendentes**: Links ainda não verificados

### Health Checker
- Verifica disponibilidade do link
- Mede tempo de resposta
- Extrai metadados HTML:
  - Título (`<title>` ou `og:title`)
  - Descrição (`meta description` ou `og:description`)
- Registra histórico de verificações

### Categorização
- Organize links por categoria
- Filtre por categoria (futuro)
- Adicione notas personalizadas

## 🗄️ Banco de Dados

### LocalDB (Padrão)

A aplicação usa SQL Server LocalDB por padrão:

```
Server=(localdb)\mssqllocaldb;Database=LinkManagerDb;Trusted_Connection=True
```

### SQL Server Completo

Para usar SQL Server completo, edite `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LinkManagerDb;User Id=sa;Password=SuaSenha123;TrustServerCertificate=True"
  }
}
```

### Migrations

As migrations são aplicadas automaticamente no startup (modo Development).

Para aplicar manualmente:

```bash
dotnet ef database update
```

Para criar nova migration:

```bash
dotnet ef migrations add NomeDaMigration
```

## 🔧 Configuração

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "sua-connection-string"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Variáveis de Ambiente

Você pode sobrescrever configurações via variáveis de ambiente:

```bash
# Windows (PowerShell)
$env:ConnectionStrings__DefaultConnection="sua-connection-string"
dotnet run

# Linux/Mac
export ConnectionStrings__DefaultConnection="sua-connection-string"
dotnet run
```

## 🐛 Troubleshooting

### Erro: "Cannot open database"

**Solução**: Verifique se o SQL Server LocalDB está instalado:

```bash
sqllocaldb info
```

Se não estiver instalado, baixe o [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads).

### Erro: "A network-related error occurred"

**Solução**: Adicione `TrustServerCertificate=True` na connection string.

### Erro: "The certificate chain was issued by an authority that is not trusted"

**Solução**: Adicione `TrustServerCertificate=True` na connection string.

### Porta já em uso

**Solução**: Mude a porta em `Properties/launchSettings.json`:

```json
{
  "applicationUrl": "https://localhost:5002;http://localhost:5001"
}
```

### Migrations não aplicadas

**Solução**: Execute manualmente:

```bash
dotnet ef database update
```

## 📦 Estrutura do Projeto

```
LinkManager.Web/
├── Data/                    # Camada de dados
│   ├── ApplicationDbContext.cs
│   ├── IPageLinkRepository.cs
│   ├── PageLinkRepository.cs
│   └── Migrations/
├── Models/                  # Modelos de domínio
│   └── PageLink.cs
├── Services/                # Serviços de negócio
│   ├── IHealthCheckerService.cs
│   └── HealthCheckerService.cs
├── Pages/                   # Páginas Blazor
│   ├── Index.razor
│   └── _Host.cshtml
├── Shared/                  # Componentes compartilhados
│   ├── MainLayout.razor
│   └── NavMenu.razor
├── wwwroot/                 # Arquivos estáticos
│   └── css/
├── Program.cs               # Configuração da aplicação
└── appsettings.json         # Configurações
```

## 🚀 Próximos Passos

### Desenvolvimento Local

1. **Adicione mais funcionalidades**:
   - Edição de links
   - Filtros e busca
   - Exportação de dados
   - Agendamento de verificações

2. **Melhore a UI**:
   - Adicione gráficos
   - Implemente paginação
   - Adicione temas

3. **Adicione testes**:
   - Testes unitários
   - Testes de integração
   - Testes E2E

### Deploy na Azure

Quando estiver pronto para produção:

1. Leia o [DEPLOYMENT.md](DEPLOYMENT.md)
2. Configure o Terraform
3. Faça o deploy na Azure

## 📚 Documentação Adicional

- [README.md](README.md) - Visão geral completa
- [ARCHITECTURE.md](ARCHITECTURE.md) - Arquitetura detalhada
- [DEPLOYMENT.md](DEPLOYMENT.md) - Guia de deploy na Azure

## 💡 Dicas

### Performance

- Use índices no banco de dados (já configurados)
- Implemente cache para consultas frequentes
- Use paginação para grandes listas

### Segurança

- Valide todas as entradas do usuário
- Use HTTPS em produção
- Proteja connection strings
- Implemente rate limiting

### Monitoramento

- Use Application Insights em produção
- Configure alertas para links offline
- Monitore tempo de resposta

## 🆘 Precisa de Ajuda?

- **Documentação**: Leia os arquivos .md na raiz do projeto
- **Issues**: Abra uma issue no GitHub
- **Logs**: Verifique os logs da aplicação em `Logs/`

## 🎉 Pronto!

Agora você tem uma aplicação completa de gerenciamento de links rodando localmente. Explore as funcionalidades e personalize conforme necessário!
