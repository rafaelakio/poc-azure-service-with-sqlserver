# 👋 Bem-vindo ao Link Manager!

<div align="center">

```
╔═══════════════════════════════════════════════════════════╗
║                                                           ║
║              🔗 LINK MANAGER                              ║
║                                                           ║
║     Gerenciador Inteligente de Links com Health Check    ║
║                                                           ║
╚═══════════════════════════════════════════════════════════╝
```

</div>

## 🎯 O que é o Link Manager?

O **Link Manager** é uma aplicação web completa que permite:

- ✅ **Gerenciar** seus links favoritos
- ✅ **Verificar** automaticamente se estão online
- ✅ **Extrair** metadados (título e descrição)
- ✅ **Organizar** por categorias
- ✅ **Monitorar** tempo de resposta
- ✅ **Visualizar** estatísticas em tempo real

## 🚀 Comece Agora!

### Opção 1: Execução Rápida (5 minutos)

```bash
# 1. Clone o repositório
git clone <url-do-repositorio>
cd poc-azure-service-with-sqlserver

# 2. Execute o script de setup
# Windows:
.\setup.ps1

# Linux/Mac:
chmod +x setup.sh && ./setup.sh

# 3. Execute a aplicação
cd LinkManager.Web
dotnet run

# 4. Acesse no navegador
# https://localhost:5001
```

### Opção 2: Passo a Passo Detalhado

📖 Leia o [QUICKSTART.md](QUICKSTART.md) para instruções detalhadas.

## 📚 Documentação

Escolha seu caminho de aprendizado:

### 🎓 Para Iniciantes

```
1. README.md          → Visão geral do projeto
2. QUICKSTART.md      → Execute em 5 minutos
3. TROUBLESHOOTING.md → Resolva problemas comuns
```

### 👨‍💻 Para Desenvolvedores

```
1. ARCHITECTURE.md    → Entenda a arquitetura
2. API.md             → Explore a API
3. DATABASE.md        → Conheça o banco de dados
4. CONTRIBUTING.md    → Contribua com o projeto
```

### 🚀 Para DevOps

```
1. DEPLOYMENT.md      → Deploy na Azure
2. terraform/         → Infraestrutura como código
3. .github/workflows/ → CI/CD pipelines
```

### 📊 Para Gestores

```
1. PROJECT-SUMMARY.md → Resumo executivo
2. README.md          → Funcionalidades
3. DEPLOYMENT.md      → Custos e infraestrutura
```

## 🎨 Interface

A aplicação possui uma interface moderna e intuitiva:

```
┌─────────────────────────────────────────────────────────┐
│  🔗 Link Manager                              [Usuário] │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ➕ Adicionar Novo Link                                │
│  ┌─────────────────────────────────────────────────┐  │
│  │ URL: [https://exemplo.com              ]        │  │
│  │ Categoria: [Tecnologia        ]                 │  │
│  │ Notas: [                      ]                 │  │
│  │                              [Adicionar Link]   │  │
│  └─────────────────────────────────────────────────┘  │
│                                                         │
│  📊 Estatísticas                                        │
│  ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐                 │
│  │  42  │ │  38  │ │   3  │ │   1  │                 │
│  │Total │ │Online│ │Offline│ │Pend. │                 │
│  └──────┘ └──────┘ └──────┘ └──────┘                 │
│                                                         │
│  📋 Links Cadastrados                                   │
│  ┌─────────────────────────────────────────────────┐  │
│  │ ✅ https://google.com        [❤️] [👁️] [🗑️]    │  │
│  │    Google - Mecanismo de busca                  │  │
│  │    Última verificação: há 2 horas               │  │
│  ├─────────────────────────────────────────────────┤  │
│  │ ✅ https://github.com        [❤️] [👁️] [🗑️]    │  │
│  │    GitHub - Plataforma de desenvolvimento       │  │
│  │    Última verificação: há 1 hora                │  │
│  └─────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

## 🛠️ Tecnologias

O projeto utiliza tecnologias modernas e robustas:

```
Frontend:  Blazor Server + Bootstrap 5
Backend:   C# .NET 8
Database:  SQL Server 2019+
ORM:       Entity Framework Core 8.0
Cloud:     Microsoft Azure
IaC:       Terraform
CI/CD:     GitHub Actions
```

## 📖 Índice Completo da Documentação

| Arquivo | Descrição | Quando Usar |
|---------|-----------|-------------|
| [README.md](README.md) | Visão geral completa | Primeira leitura |
| [QUICKSTART.md](QUICKSTART.md) | Guia de início rápido | Executar pela primeira vez |
| [ARCHITECTURE.md](ARCHITECTURE.md) | Arquitetura detalhada | Entender o sistema |
| [API.md](API.md) | Documentação da API | Integrar ou desenvolver |
| [DATABASE.md](DATABASE.md) | Estrutura do banco | Trabalhar com dados |
| [DEPLOYMENT.md](DEPLOYMENT.md) | Guia de deploy Azure | Fazer deploy |
| [TROUBLESHOOTING.md](TROUBLESHOOTING.md) | Solução de problemas | Resolver erros |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Guia de contribuição | Contribuir |
| [PROJECT-SUMMARY.md](PROJECT-SUMMARY.md) | Resumo executivo | Visão rápida |
| [INDEX.md](INDEX.md) | Índice navegável | Encontrar informação |

## 🎯 Casos de Uso

### 1. Gerenciar Links Pessoais
Organize seus links favoritos com categorias e notas.

### 2. Monitorar Sites
Verifique automaticamente se seus sites estão online.

### 3. Catalogar Recursos
Crie um catálogo de recursos com metadados extraídos automaticamente.

### 4. Dashboard de Status
Visualize o status de múltiplos sites em tempo real.

## 💡 Exemplos de Uso

### Adicionar um Link

1. Acesse a aplicação
2. Preencha a URL: `https://www.example.com`
3. Adicione uma categoria: `Tecnologia`
4. Clique em "Adicionar Link"
5. O sistema automaticamente:
   - Verifica se o site está online
   - Extrai o título da página
   - Extrai a descrição
   - Mede o tempo de resposta
   - Salva tudo no banco de dados

### Verificar Saúde de um Link

1. Na lista de links, clique no ícone ❤️
2. O sistema verifica o status atual
3. Atualiza as informações automaticamente

## 🌟 Funcionalidades Destacadas

### Health Checker Inteligente
- ✅ Requisição HTTP com timeout configurável
- ✅ Medição precisa de tempo de resposta
- ✅ Detecção automática de status
- ✅ Registro de erros detalhado

### Extração de Metadados
- ✅ Título da página (`<title>` ou `og:title`)
- ✅ Descrição (`meta description` ou `og:description`)
- ✅ Suporte a múltiplos formatos de meta tags

### Dashboard em Tempo Real
- ✅ Total de links cadastrados
- ✅ Links online (HTTP 2xx)
- ✅ Links offline (HTTP 4xx/5xx)
- ✅ Links pendentes de verificação

## 🚀 Deploy na Azure

O projeto inclui infraestrutura completa para Azure:

```
terraform/
├── main.tf              # Recursos Azure
├── variables.tf         # Variáveis configuráveis
├── outputs.tf           # Outputs úteis
└── terraform.tfvars     # Sua configuração
```

**Custo estimado**: ~$30/mês

Leia [DEPLOYMENT.md](DEPLOYMENT.md) para instruções completas.

## 🤝 Contribuindo

Contribuições são bem-vindas! Veja [CONTRIBUTING.md](CONTRIBUTING.md) para:

- 🐛 Reportar bugs
- 💡 Sugerir funcionalidades
- 🔧 Enviar pull requests
- 📖 Melhorar documentação

## 📞 Suporte

### Precisa de Ajuda?

1. **Documentação**: Consulte os arquivos .md
2. **Troubleshooting**: Veja [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
3. **Issues**: Abra uma issue no GitHub
4. **Discussions**: Participe das discussões

### Recursos Úteis

- [Documentação .NET](https://docs.microsoft.com/dotnet/)
- [Blazor Docs](https://docs.microsoft.com/aspnet/core/blazor)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [Azure Docs](https://docs.microsoft.com/azure/)

## 📊 Status do Projeto

```
✅ Funcionalidades principais implementadas
✅ Documentação completa
✅ Infraestrutura Azure pronta
✅ CI/CD configurado
✅ Pronto para produção
```

## 🎓 Aprenda Mais

Este projeto é ideal para aprender:

- 🔷 Desenvolvimento com .NET 8 e Blazor
- 🔷 Arquitetura MVC e padrões de design
- 🔷 Entity Framework Core
- 🔷 SQL Server e otimização de queries
- 🔷 Deploy na Azure
- 🔷 Infraestrutura como código (Terraform)
- 🔷 CI/CD com GitHub Actions

## 🏆 Próximos Passos

Agora que você conhece o projeto:

1. ✅ Execute localmente ([QUICKSTART.md](QUICKSTART.md))
2. ✅ Explore o código
3. ✅ Entenda a arquitetura ([ARCHITECTURE.md](ARCHITECTURE.md))
4. ✅ Faça modificações
5. ✅ Faça deploy na Azure ([DEPLOYMENT.md](DEPLOYMENT.md))
6. ✅ Contribua com melhorias

## 📄 Licença

Este projeto está licenciado sob a licença MIT. Veja [LICENSE](LICENSE) para mais detalhes.

---

<div align="center">

**Desenvolvido com ❤️ usando .NET 8 e Blazor**

[⬆️ Voltar ao topo](#-bem-vindo-ao-link-manager)

</div>
