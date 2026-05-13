# GUIA RÁPIDO - RODAR TESTES DE INTEGRAÇÃO

## MÉTODO MAIS RÁPIDO (Terminal)

### 1. Abrir Terminal:
```
# Windows PowerShell ou CMD
Win + R → cmd → Enter
```

### 2. Navegar para a pasta:
```
cd c:\PROEJTO PASC CAUAVAZ\API\backend
```

### 3. Adicionar projeto à solution (PRIMEIRA VEZ APENAS):
```
dotnet sln add Paschoalotto.IntegrationTests/Paschoalotto.IntegrationTests.csproj
```

### 4. Restaurar pacotes:
```
dotnet restore
```

### 5. Rodar os testes:
```
dotnet test Paschoalotto.IntegrationTests --verbosity normal
```

---

## RESULTADO ESPERADO

```
Passou!  - Falhou:     0, Passou:    14, Ignorado:     0, Total:    14
Tempo de Execução Total: 00:00:02.8765

Resumo dos Testes:
- TituloServiceIntegrationTests: 7 testes
- TituloRepositoryIntegrationTests: 8 testes
```

---

## COMANDOS ÚTEIS

### Rodar apenas Service tests:
```
dotnet test Paschoalotto.IntegrationTests --filter "FullyQualifiedName~TituloServiceIntegrationTests"
```

### Rodar apenas Repository tests:
```
dotnet test Paschoalotto.IntegrationTests --filter "FullyQualifiedName~TituloRepositoryIntegrationTests"
```

### Rodar TODOS os testes do projeto (Unitários + Integração):
```
dotnet test --verbosity normal
```

### Ver mais detalhes:
```
dotnet test Paschoalotto.IntegrationTests --verbosity detailed
```

---

## VIA VISUAL STUDIO

### Passo 1: Abrir Solution
- Abrir `Paschoalotto.sln`

### Passo 2: Build
- `Ctrl + Shift + B`

### Passo 3: Test Explorer
- `Ctrl + E, T` ou
- Menu: `Test` → `Test Explorer`

### Passo 4: Rodar
- Clicar com direito em `Paschoalotto.IntegrationTests`
- Selecionar "Run"

---

## PROBLEMAS COMUNS

### Erro: "Cannot find project"
```
# Solução: Adicionar à solution
dotnet sln add Paschoalotto.IntegrationTests/Paschoalotto.IntegrationTests.csproj
```

### Erro: "Package not found"
```
# Solução: Restaurar pacotes
dotnet restore
```

### Erro: "Build failed"
```
# Solução: Limpar e rebuildar
dotnet clean
dotnet build
```

---

## COMPARAÇÃO DOS TESTES

| Tipo | Comando | Tempo | Testes |
|------|---------|-------|--------|
| Unitários | `dotnet test Paschoalotto.Tests` | ~1s | 26 |
| Integração | `dotnet test Paschoalotto.IntegrationTests` | ~3s | 14 |
| Todos | `dotnet test` | ~4s | 40 |

---

## CHECKLIST

- [ ] Naveguei para `c:\PROEJTO PASC CAUAVAZ\API\backend`
- [ ] Adicionei projeto à solution (primeira vez)
- [ ] Restaurei pacotes com `dotnet restore`
- [ ] Rodei `dotnet test Paschoalotto.IntegrationTests`
- [ ] Todos os 14 testes passaram

---

## PRONTO!

Agora você tem 40 testes no backend:
- 26 testes unitários
- 14 testes de integração

Total do projeto: 89 testes (40 backend + 49 frontend)
