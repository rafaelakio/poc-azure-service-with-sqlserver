#!/bin/bash

# Script de Setup para Link Manager (Linux/Mac)
# Execute: chmod +x setup.sh && ./setup.sh

echo "========================================"
echo "  Link Manager - Setup Script"
echo "========================================"
echo ""

# Cores
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Verifica .NET SDK
echo -e "${YELLOW}Verificando .NET SDK...${NC}"
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✓ .NET SDK encontrado: $DOTNET_VERSION${NC}"
else
    echo -e "${RED}✗ .NET SDK não encontrado!${NC}"
    echo -e "${YELLOW}  Baixe em: https://dotnet.microsoft.com/download/dotnet/8.0${NC}"
    exit 1
fi

# Restaura pacotes NuGet
echo ""
echo -e "${YELLOW}Restaurando pacotes NuGet...${NC}"
cd LinkManager.Web
dotnet restore
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓ Pacotes restaurados com sucesso${NC}"
else
    echo -e "${RED}✗ Erro ao restaurar pacotes${NC}"
    exit 1
fi

# Verifica Entity Framework Tools
echo ""
echo -e "${YELLOW}Verificando Entity Framework Tools...${NC}"
if dotnet tool list -g | grep -q "dotnet-ef"; then
    echo -e "${GREEN}✓ EF Tools encontrado${NC}"
else
    echo -e "${YELLOW}! EF Tools não encontrado. Instalando...${NC}"
    dotnet tool install --global dotnet-ef
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓ EF Tools instalado com sucesso${NC}"
    else
        echo -e "${RED}✗ Erro ao instalar EF Tools${NC}"
    fi
fi

# Aplica migrations
echo ""
echo -e "${YELLOW}Aplicando migrations do banco de dados...${NC}"
dotnet ef database update
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓ Migrations aplicadas com sucesso${NC}"
else
    echo -e "${YELLOW}! Erro ao aplicar migrations (será aplicado automaticamente no primeiro run)${NC}"
fi

# Build da aplicação
echo ""
echo -e "${YELLOW}Compilando aplicação...${NC}"
dotnet build --configuration Debug
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓ Aplicação compilada com sucesso${NC}"
else
    echo -e "${RED}✗ Erro ao compilar aplicação${NC}"
    exit 1
fi

# Finalização
echo ""
echo -e "${CYAN}========================================${NC}"
echo -e "${GREEN}  Setup concluído com sucesso!${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""
echo -e "${YELLOW}Para executar a aplicação:${NC}"
echo -e "  dotnet run"
echo ""
echo -e "${YELLOW}A aplicação estará disponível em:${NC}"
echo -e "  https://localhost:5001"
echo -e "  http://localhost:5000"
echo ""
echo -e "${YELLOW}Documentação:${NC}"
echo -e "  README.md        - Visão geral"
echo -e "  QUICKSTART.md    - Início rápido"
echo -e "  ARCHITECTURE.md  - Arquitetura"
echo -e "  DEPLOYMENT.md    - Deploy Azure"
echo ""

cd ..
