# Guia de Deploy na Azure

Este guia descreve como fazer o deploy da aplicação Link Manager na Azure usando Terraform.

## 📋 Pré-requisitos

1. **Azure CLI** instalado e configurado
   ```bash
   az --version
   az login
   ```

2. **Terraform** instalado (versão 1.0+)
   ```bash
   terraform --version
   ```

3. **Subscription Azure** ativa
   ```bash
   az account show
   ```

4. **.NET 8 SDK** para build da aplicação
   ```bash
   dotnet --version
   ```

## 🏗️ Infraestrutura Provisionada

O Terraform criará os seguintes recursos na Azure:

### Recursos Principais

1. **Resource Group**: Grupo de recursos para organização
2. **App Service Plan**: Plano de hospedagem (B1 - Basic)
3. **App Service**: Aplicação web Blazor
4. **SQL Server**: Servidor de banco de dados
5. **SQL Database**: Banco de dados (Basic tier)
6. **Application Insights**: Monitoramento e telemetria
7. **Key Vault**: Armazenamento seguro de secrets

### Diagrama de Infraestrutura

```
┌─────────────────────────────────────────────────────────┐
│                    Resource Group                        │
│                                                          │
│  ┌────────────────────────────────────────────────┐    │
│  │           App Service Plan (B1)                 │    │
│  │  ┌──────────────────────────────────────────┐  │    │
│  │  │      App Service (Blazor Web App)        │  │    │
│  │  │  - .NET 8                                │  │    │
│  │  │  - Always On                             │  │    │
│  │  │  - HTTPS Only                            │  │    │
│  │  └──────────────────────────────────────────┘  │    │
│  └────────────────────────────────────────────────┘    │
│                                                          │
│  ┌────────────────────────────────────────────────┐    │
│  │              SQL Server                         │    │
│  │  ┌──────────────────────────────────────────┐  │    │
│  │  │      SQL Database (LinkManagerDb)        │  │    │
│  │  │  - Basic Tier (2GB)                      │  │    │
│  │  │  - Firewall: Allow Azure Services       │  │    │
│  │  └──────────────────────────────────────────┘  │    │
│  └────────────────────────────────────────────────┘    │
│                                                          │
│  ┌────────────────────────────────────────────────┐    │
│  │         Application Insights                    │    │
│  │  - Logs e métricas                             │    │
│  │  - Performance monitoring                       │    │
│  └────────────────────────────────────────────────┘    │
│                                                          │
│  ┌────────────────────────────────────────────────┐    │
│  │              Key Vault                          │    │
│  │  - Connection strings                          │    │
│  │  - SQL credentials                             │    │
│  └────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────┘
```

## 🚀 Passo a Passo do Deploy

### 1. Configurar Variáveis do Terraform

Copie o arquivo de exemplo e configure suas variáveis:

```bash
cd terraform
cp terraform.tfvars.example terraform.tfvars
```

Edite `terraform.tfvars`:

```hcl
# terraform.tfvars
project_name = "linkmanager"
environment  = "prod"
location     = "East US"

# SQL Server
sql_admin_username = "sqladmin"
sql_admin_password = "SuaSenhaSegura123!"  # Mude para uma senha forte

# Tags
tags = {
  Project     = "Link Manager"
  Environment = "Production"
  ManagedBy   = "Terraform"
}
```

### 2. Inicializar Terraform

```bash
cd terraform
terraform init
```

### 3. Validar Configuração

```bash
terraform validate
terraform plan
```

Revise o plano de execução para garantir que os recursos corretos serão criados.

### 4. Aplicar Infraestrutura

```bash
terraform apply
```

Digite `yes` quando solicitado. O processo levará cerca de 5-10 minutos.

### 5. Capturar Outputs

Após a conclusão, o Terraform exibirá informações importantes:

```bash
terraform output
```

Outputs disponíveis:
- `app_service_url`: URL da aplicação
- `sql_server_fqdn`: FQDN do SQL Server
- `sql_database_name`: Nome do banco de dados
- `app_insights_instrumentation_key`: Chave do Application Insights

### 6. Build da Aplicação

Na raiz do projeto:

```bash
cd ../LinkManager.Web
dotnet publish -c Release -o ./publish
```

### 7. Deploy da Aplicação

#### Opção A: Via Azure CLI

```bash
cd publish
zip -r ../app.zip .
cd ..

az webapp deployment source config-zip \
  --resource-group <resource-group-name> \
  --name <app-service-name> \
  --src app.zip
```

#### Opção B: Via Visual Studio

1. Clique com botão direito no projeto `LinkManager.Web`
2. Selecione "Publish"
3. Escolha "Azure"
4. Selecione o App Service criado
5. Clique em "Publish"

#### Opção C: Via GitHub Actions (CI/CD)

Veja a seção [CI/CD](#cicd-com-github-actions) abaixo.

### 8. Configurar Connection String

A connection string já está configurada automaticamente via Terraform, mas você pode verificar:

```bash
az webapp config connection-string list \
  --resource-group <resource-group-name> \
  --name <app-service-name>
```

### 9. Aplicar Migrations

Após o deploy, as migrations serão aplicadas automaticamente no primeiro acesso (modo Development).

Para produção, aplique manualmente:

```bash
# Configure a connection string localmente
export ConnectionStrings__DefaultConnection="<connection-string-from-terraform>"

# Aplique migrations
dotnet ef database update --project LinkManager.Web
```

### 10. Verificar Deploy

Acesse a URL do App Service (disponível nos outputs do Terraform):

```
https://<app-service-name>.azurewebsites.net
```

## 🔒 Segurança

### Secrets Management

Todos os secrets são armazenados no Azure Key Vault:

```bash
# Listar secrets
az keyvault secret list --vault-name <keyvault-name>

# Obter secret
az keyvault secret show --vault-name <keyvault-name> --name sql-connection-string
```

### Firewall do SQL Server

Por padrão, apenas serviços Azure podem acessar o SQL Server. Para adicionar seu IP:

```bash
az sql server firewall-rule create \
  --resource-group <resource-group-name> \
  --server <sql-server-name> \
  --name AllowMyIP \
  --start-ip-address <seu-ip> \
  --end-ip-address <seu-ip>
```

### HTTPS

A aplicação está configurada para usar apenas HTTPS:
- Certificado SSL gerenciado pela Azure
- Redirecionamento automático HTTP → HTTPS

## 📊 Monitoramento

### Application Insights

Acesse métricas e logs:

```bash
# Via Portal Azure
https://portal.azure.com → Application Insights → <app-insights-name>

# Via CLI
az monitor app-insights metrics show \
  --app <app-insights-name> \
  --resource-group <resource-group-name> \
  --metric requests/count
```

### Logs da Aplicação

```bash
# Stream de logs em tempo real
az webapp log tail \
  --resource-group <resource-group-name> \
  --name <app-service-name>

# Download de logs
az webapp log download \
  --resource-group <resource-group-name> \
  --name <app-service-name>
```

## 🔄 CI/CD com GitHub Actions

### 1. Criar Service Principal

```bash
az ad sp create-for-rbac \
  --name "linkmanager-github-actions" \
  --role contributor \
  --scopes /subscriptions/<subscription-id>/resourceGroups/<resource-group-name> \
  --sdk-auth
```

Copie o JSON retornado.

### 2. Configurar Secrets no GitHub

No repositório GitHub, vá em Settings → Secrets → Actions e adicione:

- `AZURE_CREDENTIALS`: JSON do service principal
- `AZURE_WEBAPP_NAME`: Nome do App Service
- `SQL_CONNECTION_STRING`: Connection string do SQL Server

### 3. Criar Workflow

Crie `.github/workflows/deploy.yml`:

```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Build
      run: |
        cd LinkManager.Web
        dotnet publish -c Release -o ./publish
    
    - name: Azure Login
      uses: azure/login@v1
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}
    
    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: ${{ secrets.AZURE_WEBAPP_NAME }}
        package: ./LinkManager.Web/publish
```

## 💰 Custos Estimados

Custos mensais aproximados (região East US):

| Recurso | Tier | Custo Mensal |
|---------|------|--------------|
| App Service Plan | B1 (Basic) | ~$13 |
| SQL Database | Basic (2GB) | ~$5 |
| Application Insights | Pay-as-you-go | ~$2-5 |
| Key Vault | Standard | ~$0.03 |
| **Total** | | **~$20-23/mês** |

### Otimização de Custos

Para desenvolvimento/teste:
- Use tier Free do App Service (limitações aplicam)
- Use SQL Database tier Basic ou compartilhado
- Desligue recursos fora do horário de trabalho

## 🧹 Limpeza de Recursos

Para remover toda a infraestrutura:

```bash
cd terraform
terraform destroy
```

Ou via Azure CLI:

```bash
az group delete --name <resource-group-name> --yes --no-wait
```

## 🔧 Troubleshooting

### Erro: "Database connection failed"

1. Verifique a connection string:
   ```bash
   az webapp config appsettings list \
     --resource-group <rg> \
     --name <app-name>
   ```

2. Verifique firewall do SQL Server
3. Teste conexão localmente

### Erro: "Application failed to start"

1. Verifique logs:
   ```bash
   az webapp log tail --resource-group <rg> --name <app-name>
   ```

2. Verifique se o runtime está correto (.NET 8)
3. Verifique se as migrations foram aplicadas

### Performance Lenta

1. Considere upgrade do App Service Plan (B1 → S1)
2. Verifique Application Insights para gargalos
3. Otimize queries do banco de dados

## 📚 Referências

- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [Azure SQL Database Documentation](https://docs.microsoft.com/azure/azure-sql/)
- [Terraform Azure Provider](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs)
- [Application Insights](https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview)

## 🆘 Suporte

Para problemas específicos da Azure:
- [Azure Support](https://azure.microsoft.com/support/)
- [Stack Overflow - Azure Tag](https://stackoverflow.com/questions/tagged/azure)

Para problemas da aplicação:
- Abra uma issue no repositório GitHub
