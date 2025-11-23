# 📝 Comandos Úteis - Link Manager

Referência rápida de comandos para desenvolvimento, deploy e manutenção.

## 🚀 Desenvolvimento Local

### Setup Inicial

```bash
# Clone o repositório
git clone <url-do-repositorio>
cd poc-azure-service-with-sqlserver

# Restaurar pacotes
cd LinkManager.Web
dotnet restore

# Build
dotnet build

# Executar
dotnet run
```

### Entity Framework

```bash
# Instalar EF Tools (global)
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef

# Verificar versão
dotnet ef --version

# Criar migration
dotnet ef migrations add NomeDaMigration

# Aplicar migrations
dotnet ef database update

# Reverter migration
dotnet ef database update PreviousMigration

# Remover última migration (não aplicada)
dotnet ef migrations remove

# Gerar script SQL
dotnet ef migrations script

# Gerar script incremental
dotnet ef migrations script FromMigration ToMigration

# Script idempotente (para produção)
dotnet ef migrations script --idempotent --output migration.sql

# Listar migrations
dotnet ef migrations list

# Drop database
dotnet ef database drop
```

### Build e Publish

```bash
# Build Debug
dotnet build

# Build Release
dotnet build -c Release

# Publish
dotnet publish -c Release -o ./publish

# Publish com runtime específico
dotnet publish -c Release -r win-x64 --self-contained

# Clean
dotnet clean
```

### Testes

```bash
# Executar todos os testes
dotnet test

# Testes com verbosidade
dotnet test --verbosity detailed

# Testes específicos
dotnet test --filter "FullyQualifiedName~PageLinkRepository"

# Testes com cobertura
dotnet test /p:CollectCoverage=true

# Gerar relatório de cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### NuGet

```bash
# Adicionar pacote
dotnet add package NomeDoPacote

# Adicionar pacote com versão específica
dotnet add package NomeDoPacote --version 1.0.0

# Remover pacote
dotnet remove package NomeDoPacote

# Listar pacotes
dotnet list package

# Atualizar pacotes
dotnet list package --outdated
dotnet add package NomeDoPacote
```

## 🗄️ SQL Server

### LocalDB

```bash
# Listar instâncias
sqllocaldb info

# Criar instância
sqllocaldb create MSSQLLocalDB

# Iniciar instância
sqllocaldb start mssqllocaldb

# Parar instância
sqllocaldb stop mssqllocaldb

# Deletar instância
sqllocaldb delete MSSQLLocalDB

# Informações da instância
sqllocaldb info mssqllocaldb
```

### SQL Server (via sqlcmd)

```bash
# Conectar
sqlcmd -S localhost -U sa -P SuaSenha

# Executar query
sqlcmd -S localhost -U sa -P SuaSenha -Q "SELECT @@VERSION"

# Executar script
sqlcmd -S localhost -U sa -P SuaSenha -i script.sql

# Backup
sqlcmd -S localhost -U sa -P SuaSenha -Q "BACKUP DATABASE LinkManagerDb TO DISK='C:\Backup\LinkManagerDb.bak'"

# Restore
sqlcmd -S localhost -U sa -P SuaSenha -Q "RESTORE DATABASE LinkManagerDb FROM DISK='C:\Backup\LinkManagerDb.bak'"
```

### Queries Úteis

```sql
-- Ver todas as tabelas
SELECT * FROM INFORMATION_SCHEMA.TABLES;

-- Ver estrutura da tabela
EXEC sp_help 'PageLinks';

-- Ver índices
EXEC sp_helpindex 'PageLinks';

-- Ver tamanho do banco
EXEC sp_spaceused;

-- Ver conexões ativas
SELECT * FROM sys.dm_exec_sessions WHERE is_user_process = 1;

-- Matar conexão
KILL <session_id>;

-- Atualizar estatísticas
UPDATE STATISTICS PageLinks WITH FULLSCAN;

-- Reorganizar índice
ALTER INDEX IX_PageLinks_Url ON PageLinks REORGANIZE;

-- Reconstruir índice
ALTER INDEX IX_PageLinks_Url ON PageLinks REBUILD;
```

## 🌐 Azure CLI

### Login e Configuração

```bash
# Login
az login

# Listar subscriptions
az account list --output table

# Definir subscription padrão
az account set --subscription "Nome ou ID"

# Ver subscription atual
az account show

# Listar locations
az account list-locations --output table
```

### Resource Groups

```bash
# Criar resource group
az group create --name rg-linkmanager --location brazilsouth

# Listar resource groups
az group list --output table

# Ver detalhes
az group show --name rg-linkmanager

# Deletar resource group
az group delete --name rg-linkmanager --yes --no-wait
```

### App Service

```bash
# Listar app services
az webapp list --output table

# Ver detalhes
az webapp show --name app-linkmanager --resource-group rg-linkmanager

# Iniciar
az webapp start --name app-linkmanager --resource-group rg-linkmanager

# Parar
az webapp stop --name app-linkmanager --resource-group rg-linkmanager

# Reiniciar
az webapp restart --name app-linkmanager --resource-group rg-linkmanager

# Ver logs em tempo real
az webapp log tail --name app-linkmanager --resource-group rg-linkmanager

# Download de logs
az webapp log download --name app-linkmanager --resource-group rg-linkmanager

# Deploy via ZIP
az webapp deployment source config-zip \
  --resource-group rg-linkmanager \
  --name app-linkmanager \
  --src app.zip

# Ver configurações
az webapp config appsettings list \
  --name app-linkmanager \
  --resource-group rg-linkmanager

# Definir configuração
az webapp config appsettings set \
  --name app-linkmanager \
  --resource-group rg-linkmanager \
  --settings KEY=VALUE
```

### SQL Database

```bash
# Listar servidores SQL
az sql server list --output table

# Criar firewall rule
az sql server firewall-rule create \
  --resource-group rg-linkmanager \
  --server sql-linkmanager \
  --name AllowMyIP \
  --start-ip-address 203.0.113.0 \
  --end-ip-address 203.0.113.0

# Listar databases
az sql db list \
  --resource-group rg-linkmanager \
  --server sql-linkmanager \
  --output table

# Ver detalhes do database
az sql db show \
  --resource-group rg-linkmanager \
  --server sql-linkmanager \
  --name LinkManagerDb

# Backup
az sql db export \
  --resource-group rg-linkmanager \
  --server sql-linkmanager \
  --name LinkManagerDb \
  --admin-user sqladmin \
  --admin-password <password> \
  --storage-key <storage-key> \
  --storage-key-type StorageAccessKey \
  --storage-uri https://mystorageaccount.blob.core.windows.net/backups/LinkManagerDb.bacpac
```

## 🏗️ Terraform

### Comandos Básicos

```bash
# Navegar para pasta terraform
cd terraform

# Inicializar
terraform init

# Validar configuração
terraform validate

# Formatar arquivos
terraform fmt

# Ver plano de execução
terraform plan

# Ver plano com arquivo de variáveis
terraform plan -var-file="prod.tfvars"

# Aplicar mudanças
terraform apply

# Aplicar sem confirmação
terraform apply -auto-approve

# Aplicar com arquivo de variáveis
terraform apply -var-file="prod.tfvars"

# Destruir recursos
terraform destroy

# Destruir sem confirmação
terraform destroy -auto-approve

# Ver outputs
terraform output

# Ver output específico
terraform output app_service_url

# Ver estado
terraform show

# Listar recursos
terraform state list

# Ver recurso específico
terraform state show azurerm_linux_web_app.main

# Refresh state
terraform refresh

# Import recurso existente
terraform import azurerm_resource_group.main /subscriptions/.../resourceGroups/rg-name
```

### Workspace (Ambientes)

```bash
# Listar workspaces
terraform workspace list

# Criar workspace
terraform workspace new dev

# Selecionar workspace
terraform workspace select dev

# Ver workspace atual
terraform workspace show

# Deletar workspace
terraform workspace delete dev
```

## 🐳 Docker

### Build e Run

```bash
# Build imagem
docker build -t linkmanager:latest .

# Run container
docker run -d -p 8080:80 --name linkmanager linkmanager:latest

# Ver containers rodando
docker ps

# Ver todos os containers
docker ps -a

# Ver logs
docker logs linkmanager

# Logs em tempo real
docker logs -f linkmanager

# Parar container
docker stop linkmanager

# Iniciar container
docker start linkmanager

# Remover container
docker rm linkmanager

# Remover imagem
docker rmi linkmanager:latest

# Entrar no container
docker exec -it linkmanager /bin/bash

# Ver uso de recursos
docker stats linkmanager
```

### Docker Compose

```bash
# Iniciar serviços
docker-compose up

# Iniciar em background
docker-compose up -d

# Ver logs
docker-compose logs

# Logs em tempo real
docker-compose logs -f

# Parar serviços
docker-compose stop

# Parar e remover
docker-compose down

# Rebuild
docker-compose build

# Rebuild e iniciar
docker-compose up --build
```

## 🔧 Git

### Comandos Básicos

```bash
# Clone
git clone <url>

# Status
git status

# Add
git add .
git add arquivo.cs

# Commit
git commit -m "mensagem"

# Push
git push origin main

# Pull
git pull origin main

# Ver histórico
git log
git log --oneline
git log --graph --oneline --all

# Ver diferenças
git diff
git diff arquivo.cs
```

### Branches

```bash
# Listar branches
git branch

# Criar branch
git branch feature/nova-funcionalidade

# Mudar de branch
git checkout feature/nova-funcionalidade

# Criar e mudar
git checkout -b feature/nova-funcionalidade

# Deletar branch local
git branch -d feature/nova-funcionalidade

# Deletar branch remota
git push origin --delete feature/nova-funcionalidade

# Merge
git checkout main
git merge feature/nova-funcionalidade
```

### Stash

```bash
# Salvar mudanças
git stash

# Salvar com mensagem
git stash save "mensagem"

# Listar stashes
git stash list

# Aplicar último stash
git stash apply

# Aplicar e remover
git stash pop

# Aplicar stash específico
git stash apply stash@{0}

# Remover stash
git stash drop stash@{0}

# Limpar todos os stashes
git stash clear
```

## 📦 Utilitários

### PowerShell (Windows)

```powershell
# Ver processos na porta
netstat -ano | findstr :5001

# Matar processo
taskkill /PID <PID> /F

# Ver variáveis de ambiente
Get-ChildItem Env:

# Definir variável de ambiente
$env:ASPNETCORE_ENVIRONMENT="Development"

# Ver versão do .NET
dotnet --version

# Limpar cache NuGet
dotnet nuget locals all --clear
```

### Bash (Linux/Mac)

```bash
# Ver processos na porta
lsof -ti:5001

# Matar processo
kill -9 $(lsof -ti:5001)

# Ver variáveis de ambiente
printenv

# Definir variável de ambiente
export ASPNETCORE_ENVIRONMENT="Development"

# Ver versão do .NET
dotnet --version

# Limpar cache NuGet
dotnet nuget locals all --clear

# Dar permissão de execução
chmod +x setup.sh

# Ver uso de disco
df -h

# Ver uso de memória
free -h
```

## 🔍 Debugging

### Logs

```bash
# Ver logs da aplicação (Development)
# Os logs aparecem no console durante dotnet run

# Habilitar logs detalhados do EF Core
# Em appsettings.Development.json:
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### Performance

```bash
# Profiling com dotnet-trace
dotnet tool install --global dotnet-trace
dotnet trace collect --process-id <PID>

# Memory dump
dotnet tool install --global dotnet-dump
dotnet dump collect --process-id <PID>

# Counters
dotnet tool install --global dotnet-counters
dotnet counters monitor --process-id <PID>
```

## 📚 Referências Rápidas

### Connection Strings

```
# LocalDB
Server=(localdb)\\mssqllocaldb;Database=LinkManagerDb;Trusted_Connection=True;TrustServerCertificate=True

# SQL Server local
Server=localhost;Database=LinkManagerDb;User Id=sa;Password=SuaSenha123;TrustServerCertificate=True

# Azure SQL
Server=tcp:sql-linkmanager.database.windows.net,1433;Database=LinkManagerDb;User ID=sqladmin;Password=SuaSenha123!;Encrypt=True;
```

### URLs Úteis

```
# Aplicação local
https://localhost:5001
http://localhost:5000

# Swagger (se habilitado)
https://localhost:5001/swagger

# Health check (se habilitado)
https://localhost:5001/health

# Azure App Service
https://<app-name>.azurewebsites.net
```

---

**Dica**: Adicione este arquivo aos seus favoritos para referência rápida!
