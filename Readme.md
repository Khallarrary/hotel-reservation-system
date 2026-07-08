# Hotel Reservation System

Sistema de reservas de hotel desenvolvido como projeto de estudos, com backend em .NET e frontend em Angular.

O objetivo do projeto e praticar a construcao de uma aplicacao fullstack com regras de negocio reais, integrando API REST, banco de dados, frontend, fluxo operacional de reservas e controle financeiro basico da hospedagem.

---

## Status do projeto

Em desenvolvimento.

O sistema ja possui um fluxo funcional de reservas, mapa/timeline, listagem paginada, modal de detalhes, status da reserva, check-in, check-out, cancelamento e modulo de caixa da reserva.

---

## Funcionalidades

### Quartos

- Cadastro de quartos
- Listagem de quartos
- Busca por id e numero
- Validacao de numero e tipo
- Bloqueio de remocao de quarto com reservas vinculadas

### Reservas

- Criacao de reserva pelo numero do quarto
- Validacao de conflito de datas no mesmo quarto
- Validacao de datas invalidas
- Validacao de limite de permanencia
- Exibicao de reservas em mapa/timeline
- Listagem de reservas em tabela
- Listagem paginada no backend
- Modal com detalhes da reserva
- Cancelamento de reserva
- Fluxo de check-in
- Fluxo de check-out
- Acesso ao caixa da reserva pelo modal
- Status da reserva:
  - Pendente
  - CheckIn
  - CheckOut
  - Cancelada

### Caixa da reserva

- Criacao automatica de conta ao criar uma reserva
- Resumo do caixa por reserva
- Lancamento de creditos
- Lancamento de debitos
- Formas de pagamento para credito:
  - Dinheiro
  - Pix
  - Deposito
  - Visa
  - Master
  - Amex
- Calculo de total de debitos
- Calculo de total de creditos
- Calculo de saldo
- Listagem de lancamentos
- Encerramento de conta
- Bloqueio de encerramento quando o saldo e diferente de zero
- Bloqueio de novos lancamentos em conta encerrada

### Frontend

- Navegacao entre mapa e lista de reservas
- Mapa de reservas em formato de timeline
- Cards de reserva com cores por status
- Formulario de criacao de reserva
- Modal de detalhes da reserva
- Acoes de check-in, check-out, cancelamento e caixa
- Tela de lista de reservas
- Paginacao visual consumindo endpoint paginado
- Tela de caixa da reserva
- Formulario para lancar credito e debito
- Toasts e mensagens de sucesso/erro
- Formatacao de datas sem deslocamento por timezone

---

## Destaque do projeto

O principal diferencial do projeto e o mapa de reservas em formato de timeline, inspirado em sistemas reais de hotelaria.

As reservas aparecem como blocos posicionados dinamicamente com base em:

- data de check-in
- data de check-out
- duracao da estadia
- janela visivel da timeline
- quarto vinculado
- status da reserva

Esse fluxo permite visualizar rapidamente a ocupacao dos quartos por periodo.

Outro ponto importante e o modulo de caixa da reserva, que aproxima o projeto de um fluxo operacional real: a reserva possui uma conta, recebe lancamentos financeiros e pode ser encerrada apenas quando o saldo esta zerado.

---

## Tecnologias utilizadas

### Backend

- .NET
- ASP.NET Core
- C#
- Entity Framework Core
- PostgreSQL
- Swagger / Swashbuckle
- Arquitetura em camadas:
  - Domain
  - Application
  - Infrastructure
  - API

### Frontend

- Angular
- TypeScript
- Standalone Components
- HTML
- CSS
- Consumo de API REST

---

## Estrutura da API

### Quartos

```http
GET /api/Quarto
GET /api/Quarto/{id}
GET /api/Quarto/numero/{numero}
POST /api/Quarto
DELETE /api/Quarto/{id}
DELETE /api/Quarto/numero/{numero}
```

### Reservas

```http
GET /api/Reserva
GET /api/Reserva/paginadas?pagina=1&tamanhoPagina=10
POST /api/Reserva
POST /api/Reserva/numero
DELETE /api/Reserva/{id}
PATCH /api/Reserva/{id}/check-in
PATCH /api/Reserva/{id}/check-out
```

### Caixa

```http
GET /reserva/{reservaId}/caixa
GET /reserva/{reservaId}/lancamentos
POST /reserva/{reservaId}/credito
POST /reserva/{reservaId}/debito
PATCH /reserva/{reservaId}/caixa/encerrar
```

---

## Regras de negocio

### Reservas

- Nao e permitido criar reserva com check-out menor ou igual ao check-in
- Nao e permitido criar reserva com check-in no passado
- Nao e permitido criar reserva acima do limite de permanencia definido
- Nao e permitido conflito de reservas no mesmo quarto
- Check-in so pode ser realizado em reservas pendentes
- Check-in nao pode ser realizado antes da data da reserva
- Check-in nao pode ser realizado em reservas expiradas
- Check-out so pode ser realizado em reservas com status CheckIn
- Check-out nao pode ser realizado antes da data final da reserva
- Quarto nao pode ser removido se possuir reservas vinculadas

### Caixa

- Toda reserva criada deve possuir uma conta vinculada
- Creditos precisam ter forma de pagamento
- Debitos nao possuem forma de pagamento
- Nao e permitido lancar credito em conta encerrada
- Nao e permitido lancar debito em conta encerrada
- Conta so pode ser encerrada com saldo igual a zero

---

## Como rodar o projeto

### Backend

Entre na pasta do backend:

```bash
cd hotel-reservation-system-back
```

Configure a variavel de ambiente com a connection string do PostgreSQL:

```powershell
$env:HOTELAPP_CONNECTION="Host=localhost;Port=5432;Database=hotelapp;Username=postgres;Password=sua_senha"
```

Rode as migrations:

```bash
dotnet ef database update --project HotelApp.Infrastructure --startup-project HotelApp.Api
```

Inicie a API:

```bash
dotnet run --project HotelApp.Api
```

Acesse o Swagger:

```text
https://localhost:7265/swagger
```

### Frontend

Entre na pasta do frontend:

```bash
cd hotel-reservation-system-front/hotel-app-front
```

Instale as dependencias:

```bash
npm install
```

Inicie o Angular:

```bash
npm start
```

Acesse:

```text
http://localhost:4200
```

---

## Testes

Para rodar os testes do backend:

```bash
cd hotel-reservation-system-back
dotnet test
```

---

## Aprendizados

Durante o desenvolvimento foram praticados:

- separacao de responsabilidades entre backend e frontend
- modelagem de entidades de dominio
- criacao de regras de negocio no dominio
- uso de DTOs
- padrao Repository e Service
- migrations com Entity Framework
- integracao Angular com API REST
- comunicacao entre componentes com Input e Output
- tratamento de datas e timezone
- controle de estado visual no frontend
- paginacao real no backend com Skip e Take
- consumo de resposta paginada no Angular
- criacao de fluxos financeiros simples
- testes unitarios de regras de dominio e servicos
- investigacao e correcao de conflito de pacotes Swagger/OpenAPI

---

## Proximos passos

- Adicionar busca com filtros na listagem de reservas
- Criar usuarios e niveis de autenticacao
- Criar tarifario por categoria de quarto
- Criar rotina de diarias
- Criar edicao de reserva
- Criar cadastro de hospede e vincular a reserva
- Avaliar cache para dados de cadastro, como quartos e futuro tarifario
- Melhorar warnings de nullable nos DTOs
- Preparar deploy/demo do projeto

---

## Licenca

Projeto desenvolvido para fins de estudo e portfolio.
