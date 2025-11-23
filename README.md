# Link Manager - Gerenciador de Links com Health Checker

Aplicação web desenvolvida em Blazor Server (.NET 8) com SQL Server para gerenciar links e verificar sua disponibilidade.

## 📋 Índice

- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Funcionalidades](#funcionalidades)
- [Tecnologias](#tecnologias)
- [Pré-requisitos](#pré-requisitos)
- [Instalação Local](#instalação-local)
- [Deploy na Azure](#deploy-na-azure)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Uso](#uso)

## 🎯 Visão Geral

O Link Manager é uma aplicação web que permite:
- Cadastrar e gerenciar links de páginas web
- Verificar automaticamente a disponibilidade dos links (Health Check)
- Extrair metadados HTML (título e descrição)
- Monitorar status e tempo de resposta
- Categorizar e organizar links

## 🏗️ Arquitetura

A aplicação segue o padrão **MVC (Model-View-Controller)** com camada **DAL (Data Access Layer)**:

```
┌─────────────────────────────────────────────┐
│              Blazor Server UI               │
│         (Views - Razor Components)          │
└────────────────┬────────────────────────────┘
                 │
┌────────────────▼────────────────────────────┐
│            Services Layer                   │
│  - HealthCheckerService                     │
│  - Business Logic                           │
└────────────────┬────────────────────────────┘
                 │
┌────────────────▼────────────────────────────┐
│         Data Access Layer (DAL)             │
│  - PageLinkRepository (Interface)           │
│  - Entity Framework Core                    │
└────────────────┬────────────────────────────┘
                 │
┌────────────────▼────────────────────────────┐
│            SQL Server Database              │
│  - PageLinks Table                          │
└─────────────────────────────────────────────┘
```

### Camadas

#### 1. **Models** (Entidades)
- `PageLink.cs`: Modelo de dados do link

#### 2. **Data Access Layer (DAL)**
- `ApplicationDbContext.cs`: Contexto do Entity Framework
- `IPageLinkRepository.cs`: Interface do repositório
- `PageLinkRepository.cs`: Implementação do repositório

#### 3. **Services** (Lógica de Negócio)
- `IHealthCheckerService.cs`: Interface do serviço de health check
- `HealthCheckerService.cs`: Implementação do health checker

#### 4. **Views** (Blazor Components)
- `Index.razor`: Página principal com CRUD e listagem

## ✨ Funcionalidades

### 1. CRUD de Links
- ✅ **Create**: Adicionar novos links
- ✅ **Read**: Listar e visualizar links
- ✅ **Update**: Atualizar informações (via health check)
- ✅ **Delete**: Remover links (soft delete)

### 2. Health Checker
- ✅ Verificação HTTP da disponibilidade do link
- ✅ Medição de tempo de resposta
- ✅ Extração de metadados HTML:
  - Título da página (`<title>`)
  - Descrição (`<meta name="description">`)
  - Open Graph tags (fallback)
- ✅ Detecção de status:
  - Online (200-299)
  - Offline (400-599)
  - Timeout
  - Error

### 3. Dashboard
- ✅ Estatísticas em tempo real
- ✅ Filtros por status e categoria
- ✅ Visualização detalhada de cada link

## 🛠️ Tecnologias

### Backend
- **.NET 8**: Framework principal
- **Blazor Server**: UI interativa
- **Entity Framework Core 8**: ORM
- **SQL Server**: Banco de dados

### Frontend
- **Blazor Components**: Componentes reativos
- **Bootstrap 5**: Framework CSS
- **Bootstrap Icons**: Ícones

### Bibliotecas
- **HtmlAgilityPack**: Parser HTML para extração de metadados
- **HttpClient**: Requisições HTTP

## 📦 Pré-requisitos

### Desenvolvimento Local
- .NET 8 SDK
- SQL Server (LocalDB, Express ou Full)
- Visual Studio 2022 ou VS Code
- Git

### Deploy na Azure
- Conta Azure ativa
- Azure CLI instalado
- Terraform instalado

## 🚀 Instalação Local

### 1. Clone o Repositório

```bash
cd poc-azure-service-with-sqlserver
```

### 2. Configure a Connection String

Edite `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LinkManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### 3. Restaure Pacotes

```bash
cd LinkManager.Web
dotnet restore
```

### 4. Crie o Banco de Dados

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Execute a Aplicação

```bash
dotnet run
```

Acesse: `https://localhost:5001`

## ☁️ Deploy na Azure

### Opção 1: Terraform (Recomendado)

Veja documentação completa em: [TERRAFORM.md](TERRAFORM.md)

```bash
cd terraform
terraform init
terraform plan
terraform apply
```

### Opção 2: Azure CLI

```bash
# Criar Resource Group
az group create --name rg-linkmanager --location brazilsouth

# Criar SQL Server
az sql server create --name sql-linkmanager --resource-group rg-linkmanager --location brazilsouth --admin-user sqladmin --admin-password "SuaSenhaForte123!"

# Criar Database
az sql db create --resource-group rg-linkmanager --server sql-linkmanager --name LinkManagerDb --service-objective S0

# Criar App Service Plan
az appservice plan create --name plan-linkmanager --resource-group rg-linkmanager --sku B1 --is-linux

# Criar Web App
az webapp create --resource-group rg-linkmanager --plan plan-linkmanager --name app-linkmanager --runtime "DOTNET|8.0"

# Deploy
dotnet publish -c Release
cd bin/Release/net8.0/publish
zip -r deploy.zip .
az webapp deployment source config-zip --resource-group rg-linkmanager --name app-linkmanager --src deploy.zip
```

## 📁 Estrutura do Projeto

```
poc-azure-service-with-sqlserver/
├── LinkManager.sln                      # Solution
├── LinkManager.Web/                     # Projeto principal
│   ├── Data/                           # Data Access Layer
│   │   ├── ApplicationDbContext.cs    # EF Context
│   │   ├── IPageLinkRepository.cs     # Interface
│   │   └── PageLinkRepository.cs      # Implementação
│   ├── Models/                         # Entidades
│   │   └── PageLink.cs                # Modelo de Link
│   ├── Services/                       # Serviços
│   │   ├── IHealthCheckerService.cs   # Interface
│   │   └── HealthCheckerService.cs    # Implementação
│   ├── Pages/                          # Blazor Pages
│   │   ├── Index.razor                # Página principal
│   │   └── _Host.cshtml               # Host page
│   ├── Shared/                         # Componentes compartilhados
│   │   ├── MainLayout.razor           # Layout principal
│   │   └── NavMenu.razor              # Menu navegação
│   ├── Program.cs                      # Configuração
│   ├── appsettings.json               # Configurações
│   └── LinkManager.Web.csproj         # Projeto
├── terraform/                          # Infraestrutura
│   ├── main.tf                        # Recursos principais
│   ├── variables.tf                   # Variáveis
│   ├── outputs.tf                     # Outputs
│   └── terraform.tfvars.example       # Exemplo de variáveis
├── README.md                           # Esta documentação
├── ARCHITECTURE.md                     # Arquitetura detalhada
└── TERRAFORM.md                        # Guia Terraform
```

## 💻 Uso

### Adicionar um Link

1. Acesse a página inicial
2. Preencha o campo "URL" com o link completo
3. (Opcional) Adicione categoria e notas
4. Clique em "Adicionar Link"
5. O sistema automaticamente:
   - Verifica a disponibilidade
   - Extrai título e descrição
   - Salva no banco de dados

### Verificar Saúde de um Link

1. Na lista de links, clique no ícone ❤️ (Health Check)
2. O sistema fará uma nova verificação
3. Os dados serão atualizados automaticamente

### Ver Detalhes

1. Clique no ícone 👁️ (Ver Detalhes)
2. Uma modal exibirá todas as informações do link

### Excluir um Link

1. Clique no ícone 🗑️ (Excluir)
2. O link será marcado como inativo (soft delete)

## 🔧 Configuração

### Connection String

Para SQL Server local:
```json
"Server=(localdb)\\mssqllocaldb;Database=LinkManagerDb;Trusted_Connection=True"
```

Para Azure SQL:
```json
"Server=tcp:seu-servidor.database.windows.net,1433;Database=LinkManagerDb;User ID=sqladmin;Password=SuaSenha;Encrypt=True;TrustServerCertificate=False"
```

### Timeouts

Edite `HealthCheckerService.cs`:

```csharp
httpClient.Timeout = TimeSpan.FromSeconds(30); // Ajuste conforme necessário
```

## 📊 Banco de Dados

### Tabela PageLinks

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| Id | int | Chave primária |
| Url | nvarchar(2000) | URL do link |
| Title | nvarchar(500) | Título da página |
| Description | nvarchar(1000) | Descrição |
| Status | nvarchar(50) | Status atual |
| HttpStatusCode | int | Código HTTP |
| ResponseTimeMs | bigint | Tempo de resposta |
| CreatedAt | datetime2 | Data de criação |
| LastCheckedAt | datetime2 | Última verificação |
| ErrorMessage | nvarchar(1000) | Mensagem de erro |
| Category | nvarchar(100) | Categoria |
| Notes | nvarchar(2000) | Notas |
| IsActive | bit | Ativo/Inativo |

### Índices

- `IX_PageLinks_Url` (Unique)
- `IX_PageLinks_Status`
- `IX_PageLinks_CreatedAt`
- `IX_PageLinks_Category`

## 🧪 Testes

### Testar Health Checker

```bash
curl -X GET https://localhost:5001/
```

### Testar Banco de Dados

```sql
SELECT * FROM PageLinks WHERE IsActive = 1;
```

## 🐛 Troubleshooting

### Erro de Conexão com Banco

**Problema**: Cannot connect to SQL Server

**Solução**:
1. Verifique se SQL Server está rodando
2. Confirme a connection string
3. Teste conexão: `sqlcmd -S (localdb)\mssqllocaldb -Q "SELECT @@VERSION"`

### Erro ao Extrair Metadados

**Problema**: Título/Descrição não são extraídos

**Solução**:
- Alguns sites bloqueiam scraping
- Verifique se o site tem os meta tags
- Aumente o timeout

### Timeout em Links

**Problema**: Muitos timeouts

**Solução**:
- Aumente o timeout em `HealthCheckerService.cs`
- Verifique conectividade de rede
- Alguns sites podem estar bloqueando

## 📝 Licença

Este projeto é fornecido como exemplo educacional.

## 🤝 Contribuindo

Veja [CONTRIBUTING.md](CONTRIBUTING.md) para detalhes.

## 📧 Suporte

Para questões e suporte, abra uma issue no repositório.
