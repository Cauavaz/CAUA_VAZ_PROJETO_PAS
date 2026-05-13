# 📁 Estrutura Organizada - Projeto Paschoalotto

## ✅ BACKEND OFICIAL (USAR)

### 📍 Localização
```
c:\PROEJTO_PASC CAUAVAZ\API\backend\
```

### 🏗️ Estrutura Clean Architecture

```
backend/
│
├── 📄 Paschoalotto.sln                 ← ABRA ESTE ARQUIVO
├── 📄 GUIA_RAPIDO.md                   ← Guia de comandos
│
└── src/
    │
    ├── 🔷 Paschoalotto.Domain/
    │   └── Entities/
    │       ├── User.cs                 (Usuários com autenticação)
    │       └── Produto.cs              (Produtos do sistema)
    │
    ├── 🔶 Paschoalotto.Application/
    │   ├── DTOs/
    │   │   └── Auth/                   (RegisterRequest, LoginRequest, etc)
    │   ├── Interfaces/
    │   │   ├── IAuthService.cs
    │   │   ├── IJwtService.cs
    │   │   └── IUserRepository.cs
    │   └── Services/
    │       └── AuthService.cs          (Lógica de autenticação)
    │
    ├── 🔸 Paschoalotto.Infrastructure/
    │   ├── Data/
    │   │   └── AppDbContext.cs         (EF Core - Users + Produtos)
    │   ├── Repositories/
    │   │   └── UserRepository.cs
    │   ├── Services/
    │   │   └── JwtService.cs           (Geração de tokens JWT)
    │   └── DependencyInjection.cs      (Configuração de serviços)
    │
    └── 🔹 Paschoalotto.API/
        ├── Controllers/
        │   └── AuthController.cs       (Endpoints de autenticação)
        ├── Program.cs                  (Configuração principal)
        ├── appsettings.json            (Configurações)
        └── Properties/
```

### ⚡ Como Usar

```powershell
# 1. Navegar até a pasta
cd "c:\PROEJTO_PASC CAUAVAZ\API\backend"

# 2. Executar o backend
dotnet run --project src/Paschoalotto.API

# 3. Acessar
# API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### 🎯 Recursos Implementados

✅ **Autenticação JWT completa**
- Registro de usuários
- Login com token
- Proteção de rotas
- Expiração configurável (24h)

✅ **Clean Architecture**
- Separação de responsabilidades
- Fácil manutenção
- Testável
- Escalável

✅ **Banco de Dados**
- Entity Framework Core
- SQL Server LocalDB
- Migrations configuradas
- Tabelas: Users, Produtos

✅ **Segurança**
- Senhas com BCrypt
- JWT com secret key
- CORS configurado
- Validações de entrada

---

## ⚠️ BACKEND ANTIGO (NÃO USAR)

### 📍 Localização
```
c:\PROEJTO_PASC CAUAVAZ\API\API_PROJETO_PASC\
```

### ❌ Por que não usar?

- Estrutura simples (sem camadas)
- Sem autenticação
- Sem organização de código
- Difícil de manter e escalar

### ✅ O que foi migrado?

- ✅ Entidade `Produto` → Migrada para o novo backend
- ✅ Tabela `Produtos` → Mantida no banco de dados
- ✅ Connection String → Reutilizada no novo backend

---

## 🔗 FRONTEND ANGULAR

### 📍 Localização
```
c:\PROEJTO_PASC CAUAVAZ\FRONT\frontend\
```

### ⚡ Como Usar

```powershell
# 1. Navegar até a pasta
cd "c:\PROEJTO_PASC CAUAVAZ\FRONT\frontend"

# 2. Instalar dependências (primeira vez)
npm install

# 3. Executar o frontend
npm start

# 4. Acessar
# http://localhost:4200
```

### 🎯 Recursos Implementados

✅ **Páginas de Autenticação**
- `/login` - Tela de login profissional
- `/register` - Tela de cadastro com validações

✅ **Funcionalidades**
- Toggle de senha (mostrar/ocultar)
- Indicador de força de senha
- Validações em tempo real
- Mensagens de erro claras
- Loading states
- Animações suaves

✅ **Segurança**
- Guards de autenticação
- Interceptor de token JWT
- Rotas protegidas
- Redirecionamentos automáticos

---

## 🚀 Fluxo de Trabalho Completo

### 1️⃣ Iniciar Backend
```powershell
cd "c:\PROEJTO_PASC CAUAVAZ\API\backend"
dotnet run --project src/Paschoalotto.API
```
**Aguarde:** `Now listening on: http://localhost:5000`

### 2️⃣ Iniciar Frontend
```powershell
cd "c:\PROEJTO_PASC CAUAVAZ\FRONT\frontend"
npm start
```
**Aguarde:** `Compiled successfully`

### 3️⃣ Testar Aplicação
1. Abra http://localhost:4200
2. Será redirecionado para `/login`
3. Clique em "Cadastre-se"
4. Preencha o formulário de registro
5. Após login, acessa área protegida

---

## 📊 Resumo da Organização

| Item | Localização | Status |
|------|-------------|--------|
| **Backend Oficial** | `API/backend/` | ✅ USAR |
| **Backend Antigo** | `API/API_PROJETO_PASC/` | ❌ NÃO USAR |
| **Frontend** | `FRONT/frontend/` | ✅ USAR |
| **Banco de Dados** | LocalDB | ✅ Compartilhado |

---

## 📚 Documentação Adicional

- 📄 `API/README.md` - Documentação completa do backend
- 📄 `API/backend/GUIA_RAPIDO.md` - Comandos essenciais
- 📄 Este arquivo - Visão geral da estrutura

---

## 🎯 Próximos Passos Sugeridos

1. **Criar Controller de Produtos** (se necessário)
   - Endpoint GET /api/produtos
   - Endpoint POST /api/produtos
   - Endpoint PUT /api/produtos/{id}
   - Endpoint DELETE /api/produtos/{id}

2. **Integrar Produtos no Frontend**
   - Criar componente de listagem
   - Criar formulário de cadastro
   - Adicionar rotas

3. **Melhorias Futuras**
   - Refresh token
   - Recuperação de senha
   - Perfis de usuário
   - Upload de imagens

---

**Última atualização:** 11/05/2026  
**Versão:** 1.0
