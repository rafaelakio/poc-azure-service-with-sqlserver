# Guia de Contribuição - Link Manager

Obrigado por considerar contribuir com o Link Manager! Este documento fornece diretrizes para contribuir com o projeto.

## 🤝 Como Contribuir

### Reportando Bugs

Antes de reportar um bug:
1. Verifique se já não existe uma issue aberta
2. Verifique [TROUBLESHOOTING.md](TROUBLESHOOTING.md) para soluções conhecidas
3. Teste na versão mais recente

Ao reportar, inclua:
- **Descrição clara** do problema
- **Passos para reproduzir**
- **Comportamento esperado** vs **comportamento atual**
- **Screenshots** (se aplicável)
- **Ambiente**:
  - SO (Windows/Linux/Mac)
  - Versão do .NET
  - Versão do SQL Server
  - Browser (se aplicável)
- **Logs** relevantes

### Sugerindo Melhorias

Para sugerir uma nova funcionalidade:
1. Abra uma issue com tag `enhancement`
2. Descreva claramente a funcionalidade
3. Explique o caso de uso
4. Sugira uma implementação (opcional)

### Pull Requests

1. **Fork** o repositório
2. **Clone** seu fork
3. **Crie uma branch** descritiva
4. **Faça suas alterações**
5. **Teste** suas alterações
6. **Commit** com mensagens claras
7. **Push** para seu fork
8. **Abra um Pull Request**

## 📋 Padrões de Código

### C# / .NET

#### Nomenclatura

```csharp
// Classes: PascalCase
public class PageLinkRepository { }

// Interfaces: I + PascalCase
public interface IPageLinkRepository { }

// Métodos: PascalCase
public async Task<PageLink> GetByIdAsync(int id) { }

// Variáveis locais: camelCase
var pageLink = new PageLink();

// Campos privados: _camelCase
private readonly ApplicationDbContext _context;

// Constantes: PascalCase
public const string Online = "Online";

// Propriedades: PascalCase
public string Url { get; set; }
```

#### Async/Await

```csharp
// ✓ Correto: Use async/await
public async Task<List<PageLink>> GetAllAsync()
{
    return await _context.PageLinks.ToListAsync();
}

// ✗ Evite: Bloqueio síncrono
public List<PageLink> GetAll()
{
    return _context.PageLinks.ToList();
}

// ✗ Evite: .Result ou .Wait()
var result = GetAllAsync().Result;
```

#### Dependency Injection

```csharp
// ✓ Correto: Injeção via construtor
public class PageLinkRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PageLinkRepository> _logger;

    public PageLinkRepository(
        ApplicationDbContext context,
        ILogger<PageLinkRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
}

// ✗ Evite: Instanciação direta
var context = new ApplicationDbContext();
```

#### Tratamento de Erros

```csharp
// ✓ Correto: Trate exceções específicas
try
{
    await _repository.AddAsync(link);
}
catch (InvalidOperationException ex)
{
    _logger.LogWarning(ex, "Link duplicado");
    throw;
}
catch (Exception ex)
{
    _logger.LogError(ex, "Erro inesperado");
    throw;
}

// ✗ Evite: Catch genérico sem logging
try
{
    await _repository.AddAsync(link);
}
catch { }
```

#### Logging

```csharp
// ✓ Correto: Logging estruturado
_logger.LogInformation("Adicionando link: {Url}", link.Url);

// ✗ Evite: String interpolation em logs
_logger.LogInformation($"Adicionando link: {link.Url}");
```

### Blazor / Razor

```razor
@* ✓ Correto: Componentes organizados *@
@page "/"
@using LinkManager.Web.Data
@inject IPageLinkRepository Repository

<PageTitle>Link Manager</PageTitle>

<div class="container">
    @* Conteúdo *@
</div>

@code {
    private List<PageLink> links = new();
    
    protected override async Task OnInitializedAsync()
    {
        links = await Repository.GetAllAsync();
    }
}
```

### SQL / Entity Framework

```csharp
// ✓ Correto: Queries otimizadas
var links = await _context.PageLinks
    .Where(p => p.IsActive)
    .OrderByDescending(p => p.CreatedAt)
    .ToListAsync();

// ✗ Evite: Carregar tudo na memória
var links = _context.PageLinks.ToList()
    .Where(p => p.IsActive)
    .OrderByDescending(p => p.CreatedAt);
```

## 🧪 Testes

### Estrutura de Testes

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    var link = new PageLink { Url = "https://test.com" };
    
    // Act
    var result = await _repository.AddAsync(link);
    
    // Assert
    Assert.NotNull(result);
    Assert.NotEqual(0, result.Id);
}
```

### Executar Testes

```bash
# Todos os testes
dotnet test

# Testes específicos
dotnet test --filter "FullyQualifiedName~PageLinkRepository"

# Com cobertura
dotnet test /p:CollectCoverage=true
```

## 📝 Commits

### Mensagens de Commit

Use o padrão [Conventional Commits](https://www.conventionalcommits.org/):

```
tipo(escopo): descrição curta

Descrição mais detalhada (opcional)

Closes #123
```

#### Tipos

- `feat`: Nova funcionalidade
- `fix`: Correção de bug
- `docs`: Documentação
- `style`: Formatação (não afeta código)
- `refactor`: Refatoração
- `test`: Adiciona/modifica testes
- `chore`: Tarefas de manutenção

#### Exemplos

```bash
# Nova funcionalidade
git commit -m "feat(health-checker): adiciona timeout configurável"

# Correção de bug
git commit -m "fix(repository): corrige duplicação de URLs"

# Documentação
git commit -m "docs(readme): atualiza instruções de instalação"

# Refatoração
git commit -m "refactor(services): extrai lógica de parsing HTML"
```

## 🌿 Branches

### Nomenclatura

```
tipo/descrição-curta
```

#### Tipos de Branch

- `feature/` - Novas funcionalidades
- `fix/` - Correções de bugs
- `docs/` - Documentação
- `refactor/` - Refatorações
- `test/` - Testes

#### Exemplos

```bash
feature/adicionar-paginacao
fix/corrigir-timeout-health-check
docs/atualizar-guia-deploy
refactor/melhorar-repository-pattern
test/adicionar-testes-unitarios
```

## 🔄 Workflow

### 1. Criar Branch

```bash
git checkout -b feature/minha-funcionalidade
```

### 2. Fazer Alterações

```bash
# Edite os arquivos
# Teste localmente
dotnet run
```

### 3. Commit

```bash
git add .
git commit -m "feat: adiciona nova funcionalidade"
```

### 4. Push

```bash
git push origin feature/minha-funcionalidade
```

### 5. Pull Request

1. Vá para o GitHub
2. Clique em "New Pull Request"
3. Preencha o template
4. Aguarde review

## ✅ Checklist do Pull Request

Antes de abrir um PR, verifique:

- [ ] Código compila sem erros
- [ ] Testes passam
- [ ] Documentação atualizada (se necessário)
- [ ] Código segue os padrões do projeto
- [ ] Commits seguem o padrão
- [ ] Branch está atualizada com main
- [ ] Descrição clara do PR

## 📚 Áreas para Contribuir

### Funcionalidades Sugeridas

- [ ] Autenticação e autorização
- [ ] Paginação de resultados
- [ ] Filtros avançados
- [ ] Exportação de dados (CSV, JSON)
- [ ] Importação de links em lote
- [ ] Agendamento de verificações
- [ ] Notificações por email
- [ ] Dashboard com gráficos
- [ ] API REST completa
- [ ] Temas customizáveis
- [ ] Multi-idioma (i18n)
- [ ] Tags/labels para links
- [ ] Histórico de verificações
- [ ] Relatórios de uptime

### Melhorias Técnicas

- [ ] Adicionar testes unitários
- [ ] Adicionar testes de integração
- [ ] Melhorar performance
- [ ] Adicionar cache
- [ ] Implementar rate limiting
- [ ] Adicionar validações
- [ ] Melhorar tratamento de erros
- [ ] Adicionar logging estruturado
- [ ] Implementar retry policies
- [ ] Adicionar health checks

### Documentação

- [ ] Tutoriais em vídeo
- [ ] Exemplos de uso
- [ ] Diagramas de arquitetura
- [ ] Guias de boas práticas
- [ ] Tradução para outros idiomas
- [ ] API reference completa

## 🎨 UI/UX

### Design

- Use Bootstrap 5 para consistência
- Siga o design existente
- Teste em diferentes resoluções
- Garanta acessibilidade (WCAG)

### Acessibilidade

```html
<!-- ✓ Correto: Use labels e ARIA -->
<button aria-label="Verificar saúde do link">
    <i class="bi bi-heart-pulse"></i>
</button>

<!-- ✗ Evite: Botões sem descrição -->
<button>
    <i class="bi bi-heart-pulse"></i>
</button>
```

## 🔒 Segurança

### Boas Práticas

- Nunca commite secrets (senhas, keys)
- Use variáveis de ambiente
- Valide todas as entradas
- Sanitize dados do usuário
- Use HTTPS em produção
- Implemente rate limiting
- Adicione CSRF protection

### Reportar Vulnerabilidades

Para reportar vulnerabilidades de segurança:
1. **NÃO** abra uma issue pública
2. Envie email para: security@example.com
3. Inclua detalhes da vulnerabilidade
4. Aguarde resposta antes de divulgar

## 📄 Licença

Ao contribuir, você concorda que suas contribuições serão licenciadas sob a mesma licença do projeto.

## 💬 Comunicação

### Canais

- **GitHub Issues**: Bugs e features
- **Pull Requests**: Code review
- **Discussions**: Perguntas gerais

### Código de Conduta

- Seja respeitoso
- Seja construtivo
- Seja paciente
- Seja inclusivo

## 🎓 Recursos para Contribuidores

### Documentação do Projeto

- [README.md](README.md) - Visão geral
- [ARCHITECTURE.md](ARCHITECTURE.md) - Arquitetura
- [API.md](API.md) - API reference
- [DATABASE.md](DATABASE.md) - Banco de dados

### Recursos Externos

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [Git Best Practices](https://git-scm.com/book/en/v2)

## 🙏 Agradecimentos

Obrigado por contribuir com o Link Manager! Sua ajuda é muito apreciada.

### Principais Contribuidores

<!-- Lista será atualizada automaticamente -->

---

**Dúvidas?** Abra uma issue ou discussion no GitHub!

## 🛡️ Branch Protection

Para garantir a qualidade do código, o branch `main` deve ter proteção configurada. No GitHub, acesse **Settings → Branches → Add rule** para `main` e habilite:

- **Require pull request reviews before merging**: pelo menos 1 approver.
- **Require status checks to pass before merging**: exigir que o workflow CI passe.
- **Require branches to be up to date before merging**: garantir que a branch esteja atualizada antes do merge.
- **Include administrators**: aplicar regras também a administradores.

## 🧑‍💻 Processo de Code Review

- Todo PR deve ter **pelo menos 1 aprovação** antes do merge.
- Revisores devem verificar:
  - Aderência aos padrões de código (style guide e Conventional Commits).
  - Cobertura de testes para novas funcionalidades e correções.
  - Documentação atualizada quando aplicável.
- Comentários de revisão devem ser resolvidos antes do merge.
