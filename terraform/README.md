# Terraform - Link Manager Infrastructure

Infraestrutura como código para provisionar recursos Azure para o Link Manager.

## 📋 Recursos Provisionados

Este módulo Terraform cria:

- ✅ Resource Group
- ✅ App Service Plan (Linux, B1)
- ✅ App Service (Web App)
- ✅ SQL Server
- ✅ SQL Database
- ✅ Application Insights
- ✅ Log Analytics Workspace
- ✅ Firewall Rules
- ✅ (Opcional) Staging Slot

## 🚀 Início Rápido

### 1. Pré-requisitos

```bash
# Terraform instalado
terraform --version

# Azure CLI instalado e autenticado
az login
az account show
```

### 2. Configurar Variáveis

```bash
# Copiar arquivo de exemplo
cp terraform.tfvars.example terraform.tfvars

# Editar com suas configurações
# IMPORTANTE: Mude os nomes para valores únicos!
nano terraform.tfvars
```

### 3. Inicializar Terraform

```bash
terraform init
```

### 4. Validar e Planejar

```bash
# Validar sintaxe
terraform validate

# Ver plano de execução
terraform plan
```

### 5. Aplicar

```bash
# Aplicar mudanças
terraform apply

# Ou aplicar sem confirmação
terraform apply -auto-approve
```

### 6. Ver Outputs

```bash
terraform output
```

## 📝 Variáveis Principais

### Obrigatórias

```hcl
sql_admin_password = "SuaSenhaSegura123!"  # Senha do SQL Server
```

### Importantes (devem ser únicas globalmente)

```hcl
sql_server_name = "sql-linkmanager-unique123"
app_service_name = "app-linkmanager-unique123"
```

### Opcionais

```hcl
resource_group_name = "rg-linkmanager"
location = "brazilsouth"
environment = "Production"
app_service_plan_sku = "B1"
sql_database_sku = "S0"
enable_staging_slot = false
```

## 🗂️ Estrutura de Arquivos

```
terraform/
├── main.tf                    # Recursos principais
├── variables.tf               # Definição de variáveis
├── outputs.tf                 # Outputs úteis
├── terraform.tfvars.example   # Exemplo de configuração
└── README.md                  # Este arquivo
```

## 📊 Outputs Disponíveis

Após aplicar, você terá acesso a:

```bash
# URL da aplicação
terraform output app_service_url

# FQDN do SQL Server
terraform output sql_server_fqdn

# Nome do banco de dados
terraform output sql_database_name

# Connection string (sem senha)
terraform output sql_connection_string

# Instrumentation key do App Insights
terraform output app_insights_instrumentation_key

# Comandos de deploy
terraform output deployment_commands
```

## 💰 Custos Estimados

### Configuração Padrão (B1 + S0)

| Recurso | SKU | Custo Mensal |
|---------|-----|--------------|
| App Service Plan | B1 | ~$13 |
| SQL Database | S0 | ~$15 |
| Application Insights | Pay-as-you-go | ~$2-5 |
| Log Analytics | Pay-as-you-go | ~$0-2 |
| **Total** | | **~$30-35** |

### Configuração Mínima (Free/Basic)

| Recurso | SKU | Custo Mensal |
|---------|-----|--------------|
| App Service Plan | F1 (Free) | $0 |
| SQL Database | Basic | ~$5 |
| Application Insights | Pay-as-you-go | ~$1-2 |
| **Total** | | **~$6-7** |

Para usar configuração mínima:
```hcl
app_service_plan_sku = "F1"
sql_database_sku = "Basic"
```

## 🔧 Comandos Úteis

### Gerenciamento

```bash
# Ver estado atual
terraform show

# Listar recursos
terraform state list

# Ver recurso específico
terraform state show azurerm_linux_web_app.main

# Refresh state
terraform refresh

# Formatar arquivos
terraform fmt

# Validar configuração
terraform validate
```

### Destruição

```bash
# Destruir todos os recursos
terraform destroy

# Destruir sem confirmação
terraform destroy -auto-approve

# Destruir recurso específico
terraform destroy -target=azurerm_linux_web_app.main
```

### Workspaces (Múltiplos Ambientes)

```bash
# Criar workspace para dev
terraform workspace new dev

# Criar workspace para prod
terraform workspace new prod

# Listar workspaces
terraform workspace list

# Selecionar workspace
terraform workspace select dev

# Ver workspace atual
terraform workspace show
```

## 🌍 Múltiplos Ambientes

### Opção 1: Workspaces

```bash
# Criar ambientes
terraform workspace new dev
terraform workspace new staging
terraform workspace new prod

# Deploy em dev
terraform workspace select dev
terraform apply -var-file="dev.tfvars"

# Deploy em prod
terraform workspace select prod
terraform apply -var-file="prod.tfvars"
```

### Opção 2: Arquivos de Variáveis

Crie arquivos separados:

```
terraform/
├── dev.tfvars
├── staging.tfvars
└── prod.tfvars
```

Deploy:
```bash
terraform apply -var-file="dev.tfvars"
terraform apply -var-file="prod.tfvars"
```

## 🔒 Segurança

### Secrets

**NUNCA** commite `terraform.tfvars` com senhas reais!

```bash
# Adicione ao .gitignore
echo "*.tfvars" >> .gitignore
echo "!terraform.tfvars.example" >> .gitignore
```

### Boas Práticas

1. **Use Azure Key Vault** para secrets em produção
2. **Habilite Azure AD authentication** quando possível
3. **Configure firewall rules** apropriadamente
4. **Use HTTPS only** (já configurado)
5. **Habilite logging e monitoring**

### Firewall SQL Server

Por padrão, apenas serviços Azure podem acessar. Para adicionar seu IP:

```hcl
# Em terraform.tfvars
allow_my_ip = "203.0.113.0"  # Seu IP público
```

Ou via Azure CLI:
```bash
az sql server firewall-rule create \
  --resource-group rg-linkmanager \
  --server sql-linkmanager \
  --name AllowMyIP \
  --start-ip-address 203.0.113.0 \
  --end-ip-address 203.0.113.0
```

## 🔄 CI/CD

### GitHub Actions

O projeto inclui workflow para deploy automático:

```yaml
# .github/workflows/deploy-azure.yml
- name: Terraform Apply
  run: |
    cd terraform
    terraform init
    terraform apply -auto-approve
```

Configure secrets no GitHub:
- `AZURE_CREDENTIALS`: Service principal JSON
- `TF_VAR_sql_admin_password`: Senha do SQL

## 📚 Recursos Criados

### Resource Group
```hcl
azurerm_resource_group.main
```

### App Service
```hcl
azurerm_service_plan.main              # App Service Plan
azurerm_linux_web_app.main             # Web App
azurerm_linux_web_app_slot.staging     # Staging Slot (opcional)
```

### SQL Server
```hcl
azurerm_mssql_server.main              # SQL Server
azurerm_mssql_database.main            # Database
azurerm_mssql_firewall_rule.*          # Firewall Rules
```

### Monitoring
```hcl
azurerm_application_insights.main      # App Insights
azurerm_log_analytics_workspace.main   # Log Analytics
```

## 🐛 Troubleshooting

### Erro: "Name already exists"

**Causa**: Nomes de SQL Server e App Service devem ser globalmente únicos.

**Solução**: Mude os nomes em `terraform.tfvars`:
```hcl
sql_server_name = "sql-linkmanager-unique-xyz123"
app_service_name = "app-linkmanager-unique-xyz123"
```

### Erro: "Insufficient permissions"

**Causa**: Sua conta Azure não tem permissões suficientes.

**Solução**: 
```bash
# Verificar permissões
az role assignment list --assignee $(az account show --query user.name -o tsv)

# Solicitar permissões de Contributor
```

### Erro: "Backend initialization required"

**Causa**: Terraform não foi inicializado.

**Solução**:
```bash
terraform init
```

### Erro: "Invalid password"

**Causa**: Senha do SQL Server não atende requisitos.

**Solução**: Use senha com:
- Mínimo 8 caracteres
- Letras maiúsculas e minúsculas
- Números
- Símbolos especiais

## 📖 Documentação Adicional

- [Terraform Azure Provider](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs)
- [Azure App Service](https://docs.microsoft.com/azure/app-service/)
- [Azure SQL Database](https://docs.microsoft.com/azure/azure-sql/)
- [Guia Completo de Deploy](../DEPLOYMENT.md)

## 🆘 Suporte

Para problemas específicos do Terraform:
1. Verifique [TROUBLESHOOTING.md](../TROUBLESHOOTING.md)
2. Consulte [Terraform Docs](https://www.terraform.io/docs/)
3. Abra uma issue no GitHub

---

**Dica**: Execute `terraform plan` antes de `apply` para revisar mudanças!
