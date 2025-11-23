# Script de Setup para Link Manager
# Execute este script para configurar o ambiente de desenvolvimento

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Link Manager - Setup Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verifica .NET SDK
Write-Host "Verificando .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET SDK encontrado: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ .NET SDK não encontrado!" -ForegroundColor Red
    Write-Host "  Baixe em: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    exit 1
}

# Verifica SQL Server LocalDB
Write-Host ""
Write-Host "Verificando SQL Server LocalDB..." -ForegroundColor Yellow
try {
    $localDbInfo = sqllocaldb info
    if ($localDbInfo) {
        Write-Host "✓ SQL Server LocalDB encontrado" -ForegroundColor Green
    }
} catch {
    Write-Host "✗ SQL Server LocalDB não encontrado!" -ForegroundColor Red
    Write-Host "  Baixe em: https://www.microsoft.com/sql-server/sql-server-downloads" -ForegroundColor Yellow
    Write-Host "  Ou use SQL Server completo e ajuste a connection string" -ForegroundColor Yellow
}

# Restaura pacotes NuGet
Write-Host ""
Write-Host "Restaurando pacotes NuGet..." -ForegroundColor Yellow
Set-Location -Path "LinkManager.Web"
dotnet restore
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Pacotes restaurados com sucesso" -ForegroundColor Green
} else {
    Write-Host "✗ Erro ao restaurar pacotes" -ForegroundColor Red
    exit 1
}

# Verifica Entity Framework Tools
Write-Host ""
Write-Host "Verificando Entity Framework Tools..." -ForegroundColor Yellow
$efTools = dotnet tool list -g | Select-String "dotnet-ef"
if ($efTools) {
    Write-Host "✓ EF Tools encontrado" -ForegroundColor Green
} else {
    Write-Host "! EF Tools não encontrado. Instalando..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ EF Tools instalado com sucesso" -ForegroundColor Green
    } else {
        Write-Host "✗ Erro ao instalar EF Tools" -ForegroundColor Red
    }
}

# Aplica migrations
Write-Host ""
Write-Host "Aplicando migrations do banco de dados..." -ForegroundColor Yellow
dotnet ef database update
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Migrations aplicadas com sucesso" -ForegroundColor Green
} else {
    Write-Host "! Erro ao aplicar migrations (será aplicado automaticamente no primeiro run)" -ForegroundColor Yellow
}

# Build da aplicação
Write-Host ""
Write-Host "Compilando aplicação..." -ForegroundColor Yellow
dotnet build --configuration Debug
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Aplicação compilada com sucesso" -ForegroundColor Green
} else {
    Write-Host "✗ Erro ao compilar aplicação" -ForegroundColor Red
    exit 1
}

# Finalização
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Setup concluído com sucesso!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Para executar a aplicação:" -ForegroundColor Yellow
Write-Host "  dotnet run" -ForegroundColor White
Write-Host ""
Write-Host "A aplicação estará disponível em:" -ForegroundColor Yellow
Write-Host "  https://localhost:5001" -ForegroundColor White
Write-Host "  http://localhost:5000" -ForegroundColor White
Write-Host ""
Write-Host "Documentação:" -ForegroundColor Yellow
Write-Host "  README.md        - Visão geral" -ForegroundColor White
Write-Host "  QUICKSTART.md    - Início rápido" -ForegroundColor White
Write-Host "  ARCHITECTURE.md  - Arquitetura" -ForegroundColor White
Write-Host "  DEPLOYMENT.md    - Deploy Azure" -ForegroundColor White
Write-Host ""

Set-Location -Path ".."
