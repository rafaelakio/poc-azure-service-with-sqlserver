# Documentação do Banco de Dados - Link Manager

Este documento descreve a estrutura do banco de dados SQL Server utilizado pela aplicação Link Manager.

## 📊 Visão Geral

- **SGBD**: SQL Server 2019+
- **ORM**: Entity Framework Core 8.0
- **Estratégia de Migration**: Code-First
- **Collation**: SQL_Latin1_General_CP1_CI_AS

## 🗂️ Estrutura do Banco

### Tabela: PageLinks

Tabela principal que armazena informações sobre os links gerenciados.

```sql
CREATE TABLE [dbo].[PageLinks] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Url] NVARCHAR(2000) NOT NULL,
    [Title] NVARCHAR(500) NULL,
    [Description] NVARCHAR(1000) NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    [HttpStatusCode] INT NULL,
    [ResponseTimeMs] BIGINT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [LastCheckedAt] DATETIME2 NULL,
    [ErrorMessage] NVARCHAR(1000) NULL,
    [Category] NVARCHAR(100) NULL,
    [Notes] NVARCHAR(2000) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT [PK_PageLinks] PRIMARY KEY CLUSTERED ([Id] ASC)
);
```

#### Colunas

| Coluna | Tipo | Nulo | Padrão | Descrição |
|--------|------|------|--------|-----------|
| **Id** | INT | Não | IDENTITY | Chave primária auto-incremento |
| **Url** | NVARCHAR(2000) | Não | - | URL completa do link |
| **Title** | NVARCHAR(500) | Sim | NULL | Título extraído do HTML |
| **Description** | NVARCHAR(1000) | Sim | NULL | Descrição extraída dos meta tags |
| **Status** | NVARCHAR(50) | Não | 'Pending' | Status atual (Online, Offline, etc) |
| **HttpStatusCode** | INT | Sim | NULL | Código HTTP da última verificação |
| **ResponseTimeMs** | BIGINT | Sim | NULL | Tempo de resposta em milissegundos |
| **CreatedAt** | DATETIME2 | Não | GETUTCDATE() | Data/hora de criação do registro |
| **LastCheckedAt** | DATETIME2 | Sim | NULL | Data/hora da última verificação |
| **ErrorMessage** | NVARCHAR(1000) | Sim | NULL | Mensagem de erro (se houver) |
| **Category** | NVARCHAR(100) | Sim | NULL | Categoria do link |
| **Notes** | NVARCHAR(2000) | Sim | NULL | Notas adicionais |
| **IsActive** | BIT | Não | 1 | Indica se o link está ativo (soft delete) |

#### Índices

```sql
-- Índice único na URL (evita duplicatas)
CREATE UNIQUE NONCLUSTERED INDEX [IX_PageLinks_Url]
    ON [dbo].[PageLinks]([Url] ASC);

-- Índice no Status (para filtros rápidos)
CREATE NONCLUSTERED INDEX [IX_PageLinks_Status]
    ON [dbo].[PageLinks]([Status] ASC);

-- Índice na data de criação (para ordenação)
CREATE NONCLUSTERED INDEX [IX_PageLinks_CreatedAt]
    ON [dbo].[PageLinks]([CreatedAt] DESC);

-- Índice na categoria (para agrupamento)
CREATE NONCLUSTERED INDEX [IX_PageLinks_Category]
    ON [dbo].[PageLinks]([Category] ASC)
    WHERE [Category] IS NOT NULL;
```

## 🔍 Queries Comuns

### Buscar todos os links ativos

```sql
SELECT *
FROM PageLinks
WHERE IsActive = 1
ORDER BY CreatedAt DESC;
```

### Buscar links por status

```sql
SELECT *
FROM PageLinks
WHERE IsActive = 1
  AND Status = 'Online'
ORDER BY LastCheckedAt DESC;
```

### Buscar links que precisam verificação

```sql
-- Links não verificados nas últimas 24 horas
SELECT *
FROM PageLinks
WHERE IsActive = 1
  AND (LastCheckedAt IS NULL 
       OR LastCheckedAt < DATEADD(HOUR, -24, GETUTCDATE()))
ORDER BY LastCheckedAt ASC;
```

### Estatísticas gerais

```sql
SELECT 
    COUNT(*) AS Total,
    SUM(CASE WHEN Status = 'Online' THEN 1 ELSE 0 END) AS Online,
    SUM(CASE WHEN Status = 'Offline' THEN 1 ELSE 0 END) AS Offline,
    SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END) AS Pending,
    AVG(CAST(ResponseTimeMs AS FLOAT)) AS AvgResponseTime
FROM PageLinks
WHERE IsActive = 1;
```

### Links por categoria

```sql
SELECT 
    Category,
    COUNT(*) AS Total,
    AVG(CAST(ResponseTimeMs AS FLOAT)) AS AvgResponseTime
FROM PageLinks
WHERE IsActive = 1
  AND Category IS NOT NULL
GROUP BY Category
ORDER BY Total DESC;
```

### Links mais lentos

```sql
SELECT TOP 10
    Url,
    Title,
    ResponseTimeMs,
    LastCheckedAt
FROM PageLinks
WHERE IsActive = 1
  AND ResponseTimeMs IS NOT NULL
ORDER BY ResponseTimeMs DESC;
```

### Links com erro

```sql
SELECT 
    Url,
    Title,
    Status,
    ErrorMessage,
    LastCheckedAt
FROM PageLinks
WHERE IsActive = 1
  AND Status IN ('Offline', 'Error', 'Timeout')
ORDER BY LastCheckedAt DESC;
```

## 📈 Otimizações de Performance

### Índices Implementados

1. **IX_PageLinks_Url (UNIQUE)**
   - Garante unicidade de URLs
   - Acelera buscas por URL
   - Usado em: `GetByUrlAsync()`

2. **IX_PageLinks_Status**
   - Acelera filtros por status
   - Usado em: `GetByStatusAsync()`, dashboard

3. **IX_PageLinks_CreatedAt**
   - Acelera ordenação por data
   - Usado em: `GetAllAsync()`

4. **IX_PageLinks_Category**
   - Acelera filtros por categoria
   - Índice filtrado (apenas quando Category não é NULL)
   - Usado em: `GetByCategoryAsync()`

### Estatísticas de Índice

```sql
-- Verificar uso dos índices
SELECT 
    i.name AS IndexName,
    s.user_seeks AS UserSeeks,
    s.user_scans AS UserScans,
    s.user_lookups AS UserLookups,
    s.user_updates AS UserUpdates
FROM sys.indexes i
INNER JOIN sys.dm_db_index_usage_stats s
    ON i.object_id = s.object_id
    AND i.index_id = s.index_id
WHERE OBJECT_NAME(i.object_id) = 'PageLinks';
```

### Fragmentação de Índices

```sql
-- Verificar fragmentação
SELECT 
    i.name AS IndexName,
    ps.avg_fragmentation_in_percent AS Fragmentation
FROM sys.dm_db_index_physical_stats(
    DB_ID(), 
    OBJECT_ID('PageLinks'), 
    NULL, 
    NULL, 
    'LIMITED'
) ps
INNER JOIN sys.indexes i
    ON ps.object_id = i.object_id
    AND ps.index_id = i.index_id
WHERE ps.avg_fragmentation_in_percent > 10;

-- Reorganizar índice se fragmentação entre 10-30%
ALTER INDEX [IX_PageLinks_Url] ON [PageLinks] REORGANIZE;

-- Reconstruir índice se fragmentação > 30%
ALTER INDEX [IX_PageLinks_Url] ON [PageLinks] REBUILD;
```

## 🔐 Segurança

### Permissões Recomendadas

```sql
-- Criar usuário da aplicação
CREATE LOGIN [LinkManagerApp] WITH PASSWORD = 'SenhaSegura123!';
CREATE USER [LinkManagerApp] FOR LOGIN [LinkManagerApp];

-- Conceder permissões mínimas necessárias
GRANT SELECT, INSERT, UPDATE ON [dbo].[PageLinks] TO [LinkManagerApp];

-- NÃO conceder DELETE (usamos soft delete)
-- NÃO conceder permissões de DDL
```

### Auditoria

```sql
-- Habilitar auditoria de alterações (opcional)
CREATE TABLE [dbo].[PageLinksAudit] (
    [AuditId] INT IDENTITY(1,1) PRIMARY KEY,
    [PageLinkId] INT NOT NULL,
    [Action] NVARCHAR(50) NOT NULL,
    [OldValue] NVARCHAR(MAX) NULL,
    [NewValue] NVARCHAR(MAX) NULL,
    [ChangedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ChangedBy] NVARCHAR(100) NOT NULL
);
```

## 🔄 Migrations

### Aplicar Migrations

```bash
# Via .NET CLI
dotnet ef database update

# Para uma migration específica
dotnet ef database update InitialCreate

# Reverter para migration anterior
dotnet ef database update PreviousMigration
```

### Criar Nova Migration

```bash
dotnet ef migrations add NomeDaMigration
```

### Gerar Script SQL

```bash
# Script completo
dotnet ef migrations script

# Script incremental
dotnet ef migrations script FromMigration ToMigration

# Script para produção
dotnet ef migrations script --idempotent --output migration.sql
```

### Remover Última Migration

```bash
dotnet ef migrations remove
```

## 📊 Monitoramento

### Tamanho do Banco

```sql
SELECT 
    DB_NAME() AS DatabaseName,
    SUM(size * 8 / 1024) AS SizeMB
FROM sys.database_files;
```

### Tamanho da Tabela

```sql
SELECT 
    t.NAME AS TableName,
    p.rows AS RowCounts,
    SUM(a.total_pages) * 8 / 1024 AS TotalSpaceMB,
    SUM(a.used_pages) * 8 / 1024 AS UsedSpaceMB
FROM sys.tables t
INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
WHERE t.NAME = 'PageLinks'
GROUP BY t.Name, p.Rows;
```

### Queries Lentas

```sql
-- Top 10 queries mais lentas
SELECT TOP 10
    qs.execution_count AS ExecutionCount,
    qs.total_elapsed_time / 1000000 AS TotalElapsedTimeSeconds,
    qs.total_elapsed_time / qs.execution_count / 1000 AS AvgElapsedTimeMs,
    SUBSTRING(qt.text, (qs.statement_start_offset/2)+1,
        ((CASE qs.statement_end_offset
            WHEN -1 THEN DATALENGTH(qt.text)
            ELSE qs.statement_end_offset
        END - qs.statement_start_offset)/2) + 1) AS QueryText
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
WHERE qt.text LIKE '%PageLinks%'
ORDER BY qs.total_elapsed_time DESC;
```

## 🧹 Manutenção

### Limpeza de Dados Antigos

```sql
-- Remover links inativos há mais de 90 dias
DELETE FROM PageLinks
WHERE IsActive = 0
  AND CreatedAt < DATEADD(DAY, -90, GETUTCDATE());
```

### Atualizar Estatísticas

```sql
-- Atualizar estatísticas da tabela
UPDATE STATISTICS PageLinks WITH FULLSCAN;

-- Atualizar estatísticas de um índice específico
UPDATE STATISTICS PageLinks IX_PageLinks_Url WITH FULLSCAN;
```

### Backup

```sql
-- Backup completo
BACKUP DATABASE [LinkManagerDb]
TO DISK = 'C:\Backups\LinkManagerDb_Full.bak'
WITH FORMAT, INIT, NAME = 'Full Backup';

-- Backup diferencial
BACKUP DATABASE [LinkManagerDb]
TO DISK = 'C:\Backups\LinkManagerDb_Diff.bak'
WITH DIFFERENTIAL, FORMAT, INIT, NAME = 'Differential Backup';
```

## 🔄 Dados de Exemplo

### Inserir Dados de Teste

```sql
INSERT INTO PageLinks (Url, Title, Description, Status, Category, CreatedAt, IsActive)
VALUES 
    ('https://www.google.com', 'Google', 'Mecanismo de busca', 'Pending', 'Search Engine', GETUTCDATE(), 1),
    ('https://www.github.com', 'GitHub', 'Plataforma de desenvolvimento', 'Pending', 'Development', GETUTCDATE(), 1),
    ('https://www.stackoverflow.com', 'Stack Overflow', 'Comunidade de programadores', 'Pending', 'Development', GETUTCDATE(), 1),
    ('https://www.microsoft.com', 'Microsoft', 'Tecnologia e software', 'Pending', 'Technology', GETUTCDATE(), 1),
    ('https://www.azure.com', 'Azure', 'Cloud computing', 'Pending', 'Cloud', GETUTCDATE(), 1);
```

## 📝 Convenções

### Nomenclatura

- **Tabelas**: PascalCase, plural (ex: `PageLinks`)
- **Colunas**: PascalCase (ex: `CreatedAt`)
- **Índices**: `IX_NomeTabela_NomeColuna`
- **Constraints**: `PK_`, `FK_`, `UQ_`, `CK_` + NomeTabela

### Tipos de Dados

- **IDs**: `INT IDENTITY`
- **Strings curtas**: `NVARCHAR(n)`
- **Strings longas**: `NVARCHAR(MAX)`
- **Datas**: `DATETIME2` (mais preciso que DATETIME)
- **Booleanos**: `BIT`
- **Números grandes**: `BIGINT`

### Valores Padrão

- Sempre use `GETUTCDATE()` para timestamps (UTC)
- Defina valores padrão sensatos (ex: `IsActive = 1`)
- Use `NULL` para campos opcionais

## 🔗 Referências

- [SQL Server Documentation](https://docs.microsoft.com/sql/sql-server/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [SQL Server Best Practices](https://docs.microsoft.com/sql/relational-databases/best-practices-database-engine)
- [Index Design Guidelines](https://docs.microsoft.com/sql/relational-databases/sql-server-index-design-guide)
