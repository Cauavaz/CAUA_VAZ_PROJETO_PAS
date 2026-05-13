# Sistema de Gestao de Titulos Financeiros

Sistema de gestao de titulos financeiros com calculo automatico de juros e multas.

## Nota Importante sobre Docker

Embora os arquivos `Dockerfile` e `docker-compose.yml` estejam presentes no projeto, a implementacao Docker nao esta completamente funcional. A containerizacao do backend foi implementada com sucesso, incluindo a comunicacao com o banco de dados. Porem, durante os testes, identificou-se que a comunicacao entre o frontend e a API nao funcionava corretamente quando executados via Docker Compose.

Apesar de nao atender aos requisitos minimos do projeto, decidi manter os arquivos Docker no codigo como demonstracao do esforco adicional investido para implementar um diferencial tecnico. Esta foi a parte que demandou maior tempo de desenvolvimento e troubleshooting, representando uma tentativa de entregar uma solucao que vai alem do padrao esperado.

Recomenda-se executar o projeto localmente conforme as instrucoes de configuracao abaixo.

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
