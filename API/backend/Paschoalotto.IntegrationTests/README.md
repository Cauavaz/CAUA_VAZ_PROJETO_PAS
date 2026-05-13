# Testes de Integração - Paschoalotto

## O que são estes testes?

Testes de integração validam a comunicação entre diferentes partes do sistema:
- Service + Repository + Database
- Repository + EF Core
- Fluxos completos de negócio

---

## Arquitetura dos Testes

```
Paschoalotto.IntegrationTests/
├── Infrastructure/
│   ├── DatabaseFixture.cs          # Classe base para criar banco de testes
│   └── TituloRepositoryIntegrationTests.cs  # Testes de Repository + EF Core
├── Application/
│   └── TituloServiceIntegrationTests.cs     # Testes de Service + Repository
├── Usings.cs                        # Imports globais
└── Paschoalotto.IntegrationTests.csproj
```

---

## Como Rodar os Testes

### **Opção 1: Via Visual Studio**

1. **Abrir Solution:**
   ```
   Paschoalotto.sln
   ```

2. **Build da Solution:**
   - `Ctrl + Shift + B`

3. **Abrir Test Explorer:**
   - `Ctrl + E, T` ou
   - Menu: `Test` → `Test Explorer`

4. **Rodar Testes:**
   - **Todos os testes:** Botão "Run All"
   - **Apenas integração:** Clicar com direito em `Paschoalotto.IntegrationTests` → "Run"

---

### **Opção 2: Via Terminal (Recomendado)**

```bash
# Navegar para a pasta do backend
cd c:\PROEJTO_PASC CAUAVAZ\API\backend

# Rodar APENAS testes de integração
dotnet test Paschoalotto.IntegrationTests --verbosity normal

# Rodar com mais detalhes
dotnet test Paschoalotto.IntegrationTests --verbosity detailed

# Rodar testes específicos
dotnet test Paschoalotto.IntegrationTests --filter "FullyQualifiedName~TituloServiceIntegrationTests"
```

---

### **Opção 3: Rodar TODOS os testes (Unitários + Integração)**

```bash
# Rodar toda a solution
dotnet test --verbosity normal

# Com cobertura de código
dotnet test --collect:"XPlat Code Coverage"
```

---

## Resultado Esperado

```
Passou!  - Falhou:     0, Passou:    14, Ignorado:     0, Total:    14
Tempo: ~2-3 segundos
```

### **Testes Incluídos:**

#### **TituloServiceIntegrationTests (7 testes):**
- `CriarAsync_ComDadosValidos_DevePersistirTituloNoBanco`
- `CriarAsync_ComNumeroDuplicado_DeveLancarExcecao`
- `ListarAsync_ComTitulosNoBanco_DeveRetornarListaOrdenada`
- `CriarAsync_ComCpfInvalido_DeveLancarExcecao`
- `CriarAsync_ComParcelasComValoresCorretos_DeveCalcularValorOriginalCorretamente`
- `CriarAsync_ComMultiplasParcelas_DeveManterRelacionamentoCorreto`

#### **TituloRepositoryIntegrationTests (8 testes):**
- `AdicionarAsync_ComTituloValido_DeveSalvarNoBanco`
- `AdicionarAsync_ComParcelas_DeveSalvarRelacionamento`
- `ListarAsync_ComMultiplosTitulos_DeveRetornarOrdenadoPorDataDecrescente`
- `ListarAsync_DeveCarregarParcelasComInclude`
- `ExisteNumeroAsync_ComNumeroExistente_DeveRetornarTrue`
- `ExisteNumeroAsync_ComNumeroInexistente_DeveRetornarFalse`
- `ListarAsync_DeveUsarAsNoTracking`
- `AdicionarAsync_ComConcorrencia_DeveManterIntegridade`

---

## Configuração

### **Banco de Dados:**
- **Tipo:** InMemory Database (EF Core)
- **Isolamento:** Cada teste cria um banco novo
- **Limpeza:** Automática após cada teste

### **Fixtures:**
- **DatabaseFixture:** Gerencia criação e limpeza de bancos
- **IClassFixture:** Padrão xUnit para compartilhar contexto

---

## Boas Práticas Implementadas

### **1. Isolamento de Testes**
```csharp
// Cada teste cria seu próprio banco
using var context = _fixture.CriarContexto();
```

### **2. Padrão AAA (Arrange-Act-Assert)**
```csharp
// Arrange - Preparar dados
var request = new CriarTituloRequest { /* ... */ };

// Act - Executar ação
var resultado = await service.CriarAsync(request);

// Assert - Verificar resultado
resultado.Should().NotBeNull();
```

### **3. Nomenclatura Descritiva**
```csharp
[Fact]
public async Task CriarAsync_ComDadosValidos_DevePersistirTituloNoBanco()
```

### **4. Validação Dupla**
```csharp
// Valida retorno do service
resultado.NumeroTitulo.Should().Be("T-INT-001");

// Valida persistência no banco
var tituloSalvo = await context.Titulos.FindAsync(id);
tituloSalvo.Should().NotBeNull();
```

---

## Comparação: Unitários vs Integração

| Aspecto | Unitários | Integração |
|---------|-----------|------------|
| **Velocidade** | Rápido (~100ms) | Médio (~500ms) |
| **Escopo** | 1 classe | Múltiplas classes |
| **Banco** | Mock | Real (InMemory) |
| **Confiança** | Média | Alta |
| **Quantidade** | 75% dos testes | 20% dos testes |

---

## Troubleshooting

### **Erro: "Cannot find project"**
```bash
# Adicionar projeto à solution
dotnet sln add Paschoalotto.IntegrationTests/Paschoalotto.IntegrationTests.csproj
```

### **Erro: "Package not found"**
```bash
# Restaurar pacotes
dotnet restore Paschoalotto.IntegrationTests
```

### **Testes lentos:**
```bash
# Rodar em paralelo
dotnet test --parallel
```

---

## Próximos Passos

### **Expandir Cobertura:**
- [ ] Testes para AuthService
- [ ] Testes para Controllers (API)
- [ ] Testes E2E completos

### **Melhorias:**
- [ ] Adicionar testes de performance
- [ ] Configurar CI/CD para rodar testes
- [ ] Adicionar relatório de cobertura

---

## Resumo do Projeto

### **Testes Implementados:**
- **Unitários:** 26 testes (Backend)
- **Integração:** 14 testes (Backend)
- **Unitários:** 49 testes (Frontend)
- **TOTAL:** 89 testes

### **Cobertura:**
- Domain Entities
- Application Services
- Infrastructure Repositories
- Validações de negócio
- Persistência de dados

---

## Suporte

Para dúvidas ou problemas:
1. Verificar logs dos testes
2. Consultar documentação do xUnit
3. Revisar código dos testes como exemplo

**Testes são documentação viva do seu código!**
