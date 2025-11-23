# 🔗 Link Manager - Gerenciador de Links com Health Checker

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4?style=for-the-badge&logo=blazor)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927?style=for-the-badge&logo=microsoft-sql-server)
![Azure](https://img.shields.io/badge/Azure-Ready-0078D4?style=for-the-badge&logo=microsoft-azure)
![Terraform](https://img.shields.io/badge/Terraform-IaC-7B42BC?style=for-the-badge&logo=terraform)

Sistema web desenvolvido em **Blazor Server** e **C# .NET 8** para gerenciamento de links com verificação automática de saúde (health check) e extração de metadados HTML.

[Início Rápido](QUICKSTART.md) • [Documentação](INDEX.md) • [Arquitetura](ARCHITECTURE.md) • [Deploy Azure](DEPLOYMENT.md)

</div>

---

## 📋 Funcionalidades

- **CRUD Completo de Links**: Criar, listar, visualizar detalhes e excluir links
- **Health Checker**: Verificação automática da disponibilidade dos links
- **Extração de Metadados**: Captura automática de título e descrição das páginas
- **Categorização**: Organização de links por categorias
- **Dashboard**: Estatísticas em tempo real (total, online, offline, pendentes)
- **Histórico**: Registro de todas as verificações com timestamps
- **Interface Responsiva**: Design moderno com Bootstrap 5

## 🏗️ Arquitetura

O projeto segue o padrão **MVC (Model-View-Controller)** com separação clara de responsabilidades:

```
LinkManager.Web/
├── Models/              # Entidades de domínio
│   └── PageLink.cs     # Modelo de dados do link
├── Data/               # Camada de Acesso a Dados (DAL)
│   ├── ApplicationDbContext.cs      # Contexto do EF Core
│   ├── IPageLinkRepository.cs       # Interface do repositório
│   ├── PageLinkRepository.cs        # Implementação do repositório
│   └── Migrations/                  # Migrations do banco
├── Services/           # Camada de Serviços (Business Logic)
│   ├── IHealthCheckerService.cs     # Interface do health checker
│   └── HealthCheckerService.cs      # Implementação do health checker
├── Pages/              # Views (Blazor Pages)
│   ├── Index.razor                  # Página principal
│   └── _Host.cshtml                 # Host HTML
└── Shared/             # Componentes compartilhados
    ├── MainLayout.razor             # Layout principal
    └── NavMenu.razor                # Menu de navegação
```

### Camadas da Aplicação

1. **Model (Modelo)**: Define a estrutura de dados (`PageLink`)
2. **DAL (Data Access Layer)**: Gerencia acesso ao banco via Entity Framework Core
3. **Services (Serviços)**: Lógica de negócio (health check, extração de metadados)
4. **View (Blazor Pages)**: Interface do usuário

## 🛠️ Tecnologias Utilizadas

- **.NET 8.0**: Framework principal
- **Blazor Server**: Framework para UI interativa
- **Entity Framework Core 8.0**: ORM para acesso ao banco de dados
- **SQL Server**: Banco de dados relacional
- **HtmlAgilityPack**: Parsing e extração de metadados HTML
- **Bootstrap 5**: Framework CSS para interface responsiva
- **Bootstrap Icons**: Ícones

## 📦 Pré-requisitos

- .NET 8.0 SDK ou superior
- SQL Server 2019 ou superior (ou SQL Server LocalDB para desenvolvimento)
- Visual Studio 2022 ou VS Code (opcional)

## 🚀 Como Executar Localmente

### 1. Clone o repositório

```bash
git clone <url-do-repositorio>
cd poc-azure-service-with-sqlserver
```

### 2. Configure a string de conexão

Edite o arquivo `appsettings.json` ou `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LinkManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### 3. Aplique as migrations

```bash
cd LinkManager.Web
dotnet ef database update
```

### 4. Execute a aplicação

```bash
dotnet run
```

A aplicação estará disponível em `https://localhost:5001` ou `http://localhost:5000`.

## 🗄️ Estrutura do Banco de Dados

### Tabela: PageLinks

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| Id | int | Chave primária (auto-incremento) |
| Url | nvarchar(2000) | URL do link (único) |
| Title | nvarchar(500) | Título extraído do HTML |
| Description | nvarchar(1000) | Descrição extraída dos meta tags |
| Status | nvarchar(50) | Status atual (Online, Offline, Pending, etc) |
| HttpStatusCode | int | Código HTTP da última verificação |
| ResponseTimeMs | bigint | Tempo de resposta em milissegundos |
| CreatedAt | datetime2 | Data de criação do registro |
| LastCheckedAt | datetime2 | Data da última verificação |
| ErrorMessage | nvarchar(1000) | Mensagem de erro (se houver) |
| Category | nvarchar(100) | Categoria do link |
| Notes | nvarchar(2000) | Notas adicionais |
| IsActive | bit | Indica se o link está ativo |

### Índices

- `IX_PageLinks_Url`: Índice único na URL
- `IX_PageLinks_Status`: Índice no status para filtros rápidos
- `IX_PageLinks_CreatedAt`: Índice na data de criação
- `IX_PageLinks_Category`: Índice na categoria

## 🔍 Health Checker

O serviço de Health Checker realiza as seguintes operações:

1. **Requisição HTTP**: Faz uma requisição GET para a URL
2. **Medição de Performance**: Registra o tempo de resposta
3. **Extração de Metadados**:
   - Título: Extrai do tag `<title>` ou `<meta property="og:title">`
   - Descrição: Extrai de `<meta name="description">` ou `<meta property="og:description">`
4. **Determinação de Status**:
   - `Online`: HTTP 2xx
   - `Offline`: HTTP 4xx/5xx ou erro de rede
   - `Timeout`: Requisição excedeu 30 segundos
   - `Error`: Outros erros
   - `Pending`: Ainda não verificado

## 📊 API do Repositório

### IPageLinkRepository

```csharp
Task<List<PageLink>> GetAllAsync()
Task<PageLink?> GetByIdAsync(int id)
Task<PageLink?> GetByUrlAsync(string url)
Task<PageLink> AddAsync(PageLink pageLink)
Task<PageLink> UpdateAsync(PageLink pageLink)
Task DeleteAsync(int id)
Task<List<PageLink>> GetByCategoryAsync(string category)
Task<List<PageLink>> GetByStatusAsync(string status)
Task<List<PageLink>> GetLinksNeedingCheckAsync(int hoursThreshold = 24)
```

## 🌐 Deploy na Azure

Consulte o arquivo [DEPLOYMENT.md](DEPLOYMENT.md) para instruções detalhadas sobre como fazer deploy na Azure usando Terraform.

### Recursos Azure Criados

- **App Service**: Hospedagem da aplicação web
- **SQL Database**: Banco de dados gerenciado
- **Application Insights**: Monitoramento e telemetria
- **Key Vault**: Gerenciamento seguro de secrets

## 📝 Variáveis de Ambiente

Para produção, configure as seguintes variáveis:

```bash
ConnectionStrings__DefaultConnection=<connection-string-sql-server>
ASPNETCORE_ENVIRONMENT=Production
```

## 🧪 Testes

Para executar testes (quando implementados):

```bash
dotnet test
```

## 📄 Licença

Este projeto é um POC (Proof of Concept) para fins educacionais.

## 👥 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📞 Suporte

Para questões e suporte, abra uma issue no repositório.
