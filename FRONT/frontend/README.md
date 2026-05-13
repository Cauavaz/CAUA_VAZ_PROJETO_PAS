# Frontend

Aplicacao Angular 18 para gestao de titulos financeiros.

## Tecnologias

- Angular 18
- TypeScript
- TailwindCSS
- RxJS
- Angular Signals

## Configuracao

```bash
# Copiar arquivo de ambiente
copy .env.example .env

# Instalar dependencias
npm install

# Rodar servidor de desenvolvimento
npm start
```

Acesse: `http://localhost:4200`

## Comandos

```bash
# Desenvolvimento
npm start

# Build para producao
npm run build

# Testes
npm test

# Lint
npm run lint
```

## Estrutura

```
src/
├── app/
│   ├── core/          # Servicos, guards, interceptors
│   ├── features/      # Modulos de funcionalidades
│   ├── shared/        # Componentes, pipes, validators compartilhados
│   └── environments/  # Configuracoes de ambiente
```

## Variaveis de Ambiente

Edite `environment.ts` para configurar a URL da API:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000'
};
```
