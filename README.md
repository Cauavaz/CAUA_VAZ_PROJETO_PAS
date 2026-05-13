# Sistema de Gestao de Titulos Financeiros

Sistema de gestao de titulos financeiros com calculo automatico de juros e multas.

## Tecnologias

Backend: .NET 8, Entity Framework Core, JWT, Swagger  
Frontend: Angular 18, TypeScript, TailwindCSS

## Configuracao

### Backend
```bash
cd API/backend
copy .env.example .env
dotnet run --project src/Paschoalotto.API
```
Acesse: http://localhost:5000/swagger

### Frontend
```bash
cd FRONT/frontend
copy .env.example .env
npm install && npm start
```
Acesse: http://localhost:4200

## Endpoints

Autenticacao:
- POST /api/auth/register - Registrar usuario
- POST /api/auth/login - Login
- GET /api/auth/me - Dados do usuario

Titulos:
- POST /api/titulos - Criar titulo
- GET /api/titulos - Listar titulos

## Testes

```bash
# Backend
cd API/backend
dotnet test

# Frontend
cd FRONT/frontend
npm test
```

## Variaveis de Ambiente

Copie .env.example para .env em ambos os projetos antes de rodar.
