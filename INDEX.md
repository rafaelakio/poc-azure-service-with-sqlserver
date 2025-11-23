# 📚 Índice de Documentação - Link Manager

Bem-vindo à documentação completa do Link Manager! Este índice te ajudará a encontrar rapidamente a informação que você precisa.

## 🚀 Começando

### Para Iniciantes
1. **[README.md](README.md)** - Comece aqui! Visão geral do projeto
2. **[QUICKSTART.md](QUICKSTART.md)** - Execute a aplicação em 5 minutos
3. **[TROUBLESHOOTING.md](TROUBLESHOOTING.md)** - Problemas comuns e soluções

### Para Desenvolvedores
1. **[ARCHITECTURE.md](ARCHITECTURE.md)** - Entenda a arquitetura do sistema
2. **[API.md](API.md)** - Documentação completa da API
3. **[DATABASE.md](DATABASE.md)** - Estrutura e queries do banco de dados

### Para DevOps
1. **[DEPLOYMENT.md](DEPLOYMENT.md)** - Deploy na Azure com Terraform
2. **[terraform/](terraform/)** - Infraestrutura como código

---

## 📖 Documentação por Tópico

### 🏗️ Arquitetura e Design

#### [ARCHITECTURE.md](ARCHITECTURE.md)
- Visão geral da arquitetura
- Diagrama de camadas
- Padrões de design utilizados
- Fluxo de dados
- Tecnologias e versões

**Quando usar**: Para entender como o sistema funciona internamente.

---

### 🚀 Início Rápido

#### [QUICKSTART.md](QUICKSTART.md)
- Pré-requisitos
- Instalação em 3 passos
- Testando a aplicação
- Funcionalidades principais
- Configuração básica

**Quando usar**: Primeira vez executando o projeto.

---

### 📋 Visão Geral

#### [README.md](README.md)
- Descrição do projeto
- Funcionalidades
- Tecnologias utilizadas
- Como executar
- Estrutura do projeto
- Deploy na Azure

**Quando usar**: Para ter uma visão geral completa do projeto.

---

### 🔌 API e Interfaces

#### [API.md](API.md)
- IPageLinkRepository
  - GetAllAsync()
  - GetByIdAsync()
  - AddAsync()
  - UpdateAsync()
  - DeleteAsync()
  - GetByCategoryAsync()
  - GetByStatusAsync()
  - GetLinksNeedingCheckAsync()
- IHealthCheckerService
  - CheckHealthAsync()
  - CheckAndUpdateAsync()
- Modelos de dados
- Exemplos de uso
- Boas práticas

**Quando usar**: Para integrar com a aplicação ou entender os métodos disponíveis.

---

### 🗄️ Banco de Dados

#### [DATABASE.md](DATABASE.md)
- Estrutura das tabelas
- Índices e otimizações
- Queries comuns
- Migrations
- Segurança
- Monitoramento
- Manutenção
- Backup e restore

**Quando usar**: Para trabalhar diretamente com o banco de dados.

---

### 🌐 Deploy e Infraestrutura

#### [DEPLOYMENT.md](DEPLOYMENT.md)
- Pré-requisitos Azure
- Infraestrutura provisionada
- Passo a passo do deploy
- Configuração de secrets
- Monitoramento
- CI/CD com GitHub Actions
- Custos estimados
- Troubleshooting de deploy

**Quando usar**: Para fazer deploy na Azure.

#### [terraform/](terraform/)
- `main.tf` - Recursos Azure
- `variables.tf` - Variáveis configuráveis
- `outputs.tf` - Outputs do Terraform
- `terraform.tfvars.example` - Exemplo de configuração

**Quando usar**: Para provisionar infraestrutura na Azure.

---

### 🔧 Troubleshooting

#### [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
- Problemas comuns
  - Erros de conexão
  - Erros de migration
  - Problemas de performance
  - Erros de porta
- FAQ
  - Como adicionar autenticação?
  - Como adicionar paginação?
  - Como agendar verificações?
  - Como exportar dados?
  - Como usar Docker?
- Debug tips
- Recursos de suporte

**Quando usar**: Quando algo não está funcionando.

---

## 🎯 Guias por Cenário

### Cenário 1: "Quero executar o projeto pela primeira vez"

1. Leia [QUICKSTART.md](QUICKSTART.md)
2. Execute `setup.ps1` (Windows) ou `setup.sh` (Linux/Mac)
3. Execute `dotnet run`
4. Acesse `https://localhost:5001`

---

### Cenário 2: "Quero entender como o código funciona"

1. Leia [ARCHITECTURE.md](ARCHITECTURE.md) - Entenda a estrutura
2. Leia [API.md](API.md) - Veja os métodos disponíveis
3. Explore o código em `LinkManager.Web/`

---

### Cenário 3: "Quero fazer deploy na Azure"

1. Leia [DEPLOYMENT.md](DEPLOYMENT.md)
2. Configure `terraform/terraform.tfvars`
3. Execute `terraform apply`
4. Faça deploy da aplicação

---

### Cenário 4: "Estou com um erro"

1. Verifique [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
2. Procure o erro específico
3. Siga as soluções sugeridas
4. Se não resolver, abra uma issue

---

### Cenário 5: "Quero adicionar uma funcionalidade"

1. Entenda a arquitetura em [ARCHITECTURE.md](ARCHITECTURE.md)
2. Veja exemplos em [API.md](API.md)
3. Siga os padrões existentes
4. Adicione testes (se aplicável)

---

### Cenário 6: "Quero trabalhar com o banco de dados"

1. Leia [DATABASE.md](DATABASE.md)
2. Veja a estrutura das tabelas
3. Use as queries de exemplo
4. Crie migrations se necessário

---

## 📁 Estrutura de Arquivos

```
poc-azure-service-with-sqlserver/
├── 📄 README.md                    # Visão geral do projeto
├── 📄 QUICKSTART.md                # Guia de início rápido
├── 📄 ARCHITECTURE.md              # Documentação da arquitetura
├── 📄 API.md                       # Documentação da API
├── 📄 DATABASE.md                  # Documentação do banco
├── 📄 DEPLOYMENT.md                # Guia de deploy Azure
├── 📄 TROUBLESHOOTING.md           # Solução de problemas
├── 📄 INDEX.md                     # Este arquivo
├── 📄 .gitignore                   # Arquivos ignorados pelo Git
├── 📄 LinkManager.sln              # Solution do Visual Studio
├── 📄 setup.ps1                    # Script de setup (Windows)
├── 📄 setup.sh                     # Script de setup (Linux/Mac)
│
├── 📁 LinkManager.Web/             # Aplicação principal
│   ├── 📁 Data/                    # Camada de dados (DAL)
│   │   ├── ApplicationDbContext.cs
│   │   ├── IPageLinkRepository.cs
│   │   ├── PageLinkRepository.cs
│   │   └── 📁 Migrations/
│   ├── 📁 Models/                  # Modelos de domínio
│   │   └── PageLink.cs
│   ├── 📁 Services/                # Serviços de negócio
│   │   ├── IHealthCheckerService.cs
│   │   └── HealthCheckerService.cs
│   ├── 📁 Pages/                   # Páginas Blazor
│   │   ├── Index.razor
│   │   └── _Host.cshtml
│   ├── 📁 Shared/                  # Componentes compartilhados
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── 📁 wwwroot/                 # Arquivos estáticos
│   │   └── 📁 css/
│   ├── Program.cs                  # Configuração da aplicação
│   ├── appsettings.json            # Configurações
│   └── LinkManager.Web.csproj      # Arquivo do projeto
│
├── 📁 terraform/                   # Infraestrutura como código
│   ├── main.tf                     # Recursos Azure
│   ├── variables.tf                # Variáveis
│   ├── outputs.tf                  # Outputs
│   └── terraform.tfvars.example    # Exemplo de configuração
│
└── 📁 .github/                     # GitHub Actions
    └── 📁 workflows/
        └── deploy-azure.yml        # CI/CD pipeline
```

---

## 🔍 Busca Rápida

### Comandos Úteis

```bash
# Executar aplicação
dotnet run

# Aplicar migrations
dotnet ef database update

# Criar migration
dotnet ef migrations add NomeDaMigration

# Build
dotnet build

# Publicar
dotnet publish -c Release

# Testes
dotnet test

# Terraform
terraform init
terraform plan
terraform apply
terraform destroy
```

---

### Conceitos Importantes

- **Repository Pattern**: [ARCHITECTURE.md](ARCHITECTURE.md#padrões-de-design-utilizados)
- **Health Checker**: [README.md](README.md#-health-checker)
- **Migrations**: [DATABASE.md](DATABASE.md#-migrations)
- **Soft Delete**: [ARCHITECTURE.md](ARCHITECTURE.md#5-soft-delete)
- **Dependency Injection**: [ARCHITECTURE.md](ARCHITECTURE.md#configuração-e-injeção-de-dependências)

---

### Tecnologias

- **.NET 8**: Framework principal
- **Blazor Server**: UI framework
- **Entity Framework Core**: ORM
- **SQL Server**: Banco de dados
- **Terraform**: Infraestrutura como código
- **Azure**: Cloud provider
- **HtmlAgilityPack**: Parsing HTML

---

## 📞 Precisa de Ajuda?

### Documentação
- Leia os arquivos .md relevantes
- Verifique [TROUBLESHOOTING.md](TROUBLESHOOTING.md)

### Comunidade
- Stack Overflow: Tag `blazor`, `entity-framework-core`
- GitHub Issues: Reporte bugs ou peça features

### Recursos Externos
- [Documentação .NET](https://docs.microsoft.com/dotnet/)
- [Blazor Docs](https://docs.microsoft.com/aspnet/core/blazor)
- [EF Core Docs](https://docs.microsoft.com/ef/core/)
- [Azure Docs](https://docs.microsoft.com/azure/)
- [Terraform Docs](https://www.terraform.io/docs/)

---

## 🎓 Aprendizado

### Para Iniciantes em .NET
1. [QUICKSTART.md](QUICKSTART.md) - Execute o projeto
2. [README.md](README.md) - Entenda o que faz
3. [ARCHITECTURE.md](ARCHITECTURE.md) - Veja como funciona
4. Explore o código

### Para Desenvolvedores Experientes
1. [ARCHITECTURE.md](ARCHITECTURE.md) - Arquitetura
2. [API.md](API.md) - Interfaces e métodos
3. [DATABASE.md](DATABASE.md) - Estrutura de dados
4. Contribua com melhorias

### Para DevOps
1. [DEPLOYMENT.md](DEPLOYMENT.md) - Deploy Azure
2. `terraform/` - Infraestrutura
3. `.github/workflows/` - CI/CD
4. Configure monitoramento

---

## 🚀 Próximos Passos

Depois de explorar a documentação:

1. ✅ Execute o projeto localmente
2. ✅ Entenda a arquitetura
3. ✅ Faça modificações
4. ✅ Adicione testes
5. ✅ Faça deploy na Azure
6. ✅ Configure CI/CD
7. ✅ Monitore em produção

---

## 📝 Contribuindo

Quer melhorar a documentação?

1. Fork o repositório
2. Crie uma branch (`git checkout -b docs/melhoria`)
3. Faça suas alterações
4. Commit (`git commit -m 'Melhora documentação X'`)
5. Push (`git push origin docs/melhoria`)
6. Abra um Pull Request

---

**Última atualização**: Novembro 2024

**Versão**: 1.0.0

**Licença**: MIT (ou conforme definido no projeto)
