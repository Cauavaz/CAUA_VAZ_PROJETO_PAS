# Estrutura do Backend

## Backend Oficial

**Localização:** `c:\PROEJTO_PASC CAUAVAZ\API\backend\`

### Estrutura Clean Architecture

```
backend/
├── Paschoalotto.sln                    # Solution principal
└── src/
    ├── Paschoalotto.Domain/            # Camada de Domínio
    │   └── Entities/
    │       ├── User.cs                 # Entidade de usuário
    │       └── Produto.cs              # Entidade de produto
    │
    ├── Paschoalotto.Application/       # Camada de Aplicação
    │   ├── DTOs/
    │   │   └── Auth/                   # DTOs de autenticação
    │   ├── Interfaces/
    │   │   ├── IAuthService.cs
    │   │   ├── IJwtService.cs
    │   │   └── IUserRepository.cs
    │   └── Services/
    │       └── AuthService.cs          # Lógica de autenticação
    │
    ├── Paschoalotto.Infrastructure/    # Camada de Infraestrutura
    │   ├── Data/
    │   │   └── AppDbContext.cs         # Contexto EF Core
    │   ├── Repositories/
    │   │   └── UserRepository.cs
    │   ├── Services/
    │   │   └── JwtService.cs           # Geração de tokens JWT
    │   └── DependencyInjection.cs      # Configuração de DI
    │
    └── Paschoalotto.API/               # Camada de Apresentação
        ├── Controllers/
        │   └── AuthController.cs       # Endpoints de autenticação
        ├── Program.cs                  # Configuração da aplicação
        └── appsettings.json            # Configurações

```

### Como Executar

```powershell
# Navegar até a pasta do backend
cd "c:\PROEJTO_PASC CAUAVAZ\API\backend"

# Restaurar dependências
dotnet restore

# Criar/Atualizar banco de dados
dotnet ef database update --project src/Paschoalotto.API

# Executar aplicação
dotnet run --project src/Paschoalotto.API
```

### Endpoints Disponíveis

**Base URL:** `http://localhost:5000`

#### Autenticação
- `POST /api/auth/register` - Cadastro de usuário
- `POST /api/auth/login` - Login
- `GET /api/auth/me` - Dados do usuário autenticado (requer token)

#### Swagger
- `GET /swagger` - Documentação interativa da API

### Configurações

**appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PaschoalottoDb;Trusted_Connection=True;"
  },
  "Jwt": {
    "Secret": "sua-chave-secreta-super-segura-com-no-minimo-32-caracteres",
    "Issuer": "PaschoalottoAPI",
    "Audience": "PaschoalottoClient",
    "ExpirationInHours": 24
  }
}
```

### Banco de Dados

**Tabelas:**
- `Users` - Usuários do sistema (com autenticação JWT)
- `Produtos` - Produtos cadastrados

---

## Backend Antigo (NÃO USAR - APENAS REFERÊNCIA)

**Localização:** `c:\PROEJTO_PASC CAUAVAZ\API\API_PROJETO_PASC\`

Este é o backend antigo com estrutura simples. **Não utilize mais este projeto.**

### Migração Concluída

✅ Entidade `Produto` migrada para o novo backend  
✅ Estrutura do banco de dados mantida  
✅ Clean Architecture implementada  
✅ Autenticação JWT adicionada  

---

## Próximos Passos

1. **Criar Migration para Produtos:**
   ```powershell
   cd "c:\PROEJTO_PASC CAUAVAZ\API\backend"
   dotnet ef migrations add AddProdutos --project src/Paschoalotto.Infrastructure --startup-project src/Paschoalotto.API
   dotnet ef database update --project src/Paschoalotto.API
   ```

2. **Criar Controller de Produtos** (se necessário)

3. **Integrar com Frontend Angular** em `c:\PROEJTO_PASC CAUAVAZ\FRONT\frontend`

---

## Segurança

- ✅ Senhas criptografadas com BCrypt
- ✅ JWT com expiração configurável
- ✅ CORS configurado para `http://localhost:4200`
- ✅ Validações de entrada
- ✅ Email único por usuário

---

## Tecnologias

- .NET 8
- Entity Framework Core
- SQL Server LocalDB
- JWT Authentication
- BCrypt
- Swagger/OpenAPI

---

**Última atualização:** 11/05/2026
