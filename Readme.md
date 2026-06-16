# Hotel Reservation System

Sistema de reservas de hotel desenvolvido como projeto de estudos, com backend em .NET e frontend em Angular.

O objetivo do projeto é praticar a construção de uma aplicação completa, passando por modelagem de domínio, regras de negócio, API REST, banco de dados, integração com frontend e evolução de experiência do usuário.

---

## Status do projeto

Em desenvolvimento.

O sistema já possui um fluxo funcional de reservas, incluindo criação, visualização em mapa, listagem, modal de detalhes, cancelamento, check-in e check-out.

---

## Funcionalidades

### Quartos

- Cadastro de quartos
- Listagem de quartos
- Validação de número e tipo
- Bloqueio de remoção de quarto com reservas vinculadas

### Reservas

- Criação de reserva pelo número do quarto
- Validação de conflito de datas no mesmo quarto
- Validação de datas inválidas ou reservas muito longas
- Exibição de reservas em mapa/timeline
- Listagem de reservas em tabela
- Modal com detalhes da reserva
- Cancelamento de reserva
- Fluxo de check-in
- Fluxo de check-out
- Status da reserva:
  - Pendente
  - CheckIn
  - CheckOut
  - Cancelada

### Frontend

- Navegação entre mapa e lista de reservas
- Toasts de sucesso e erro
- Cores diferentes no mapa conforme o status da reserva
- Botão principal do modal muda conforme o status da reserva
- Formatação de datas sem deslocamento por timezone

---

## Destaque do projeto

O principal diferencial é o mapa de reservas em formato de timeline, inspirado em sistemas de hotelaria.

As reservas aparecem como blocos posicionados dinamicamente com base em:

- data de check-in
- data de check-out
- duração da estadia
- janela visível da timeline

Esse fluxo permite visualizar rapidamente quais quartos estão ocupados em cada período.

---

## Tecnologias utilizadas

### Backend

- .NET
- ASP.NET Core
- C#
- Entity Framework Core
- PostgreSQL
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
POST /api/Reserva
POST /api/Reserva/numero
DELETE /api/Reserva/{id}
PATCH /api/Reserva/{id}/check-in
PATCH /api/Reserva/{id}/check-out
```

---

## Regras de negócio

- Não é permitido criar reserva com check-out menor ou igual ao check-in
- Não é permitido criar reserva com check-in no passado
- Não é permitido criar reserva com mais de 30 dias
- Não é permitido conflito de reservas no mesmo quarto
- Check-in só pode ser realizado em reservas pendentes
- Check-in não pode ser realizado antes da data da reserva
- Check-in não pode ser realizado em reservas já expiradas
- Check-out só pode ser realizado em reservas com status CheckIn
- Check-out não pode ser realizado antes da data final da reserva
- Quarto não pode ser removido se possuir reservas vinculadas

---

## Como rodar o projeto

### Backend

Entre na pasta do backend:

```bash
cd hotel-reservation-system-back
```

Configure a variável de ambiente com a connection string do PostgreSQL:

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

Instale as dependências:

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

- separação de responsabilidades entre backend e frontend
- criação de entidades com regras de negócio
- uso de DTOs
- padrão Repository e Service
- migrations com Entity Framework
- integração Angular com API REST
- comunicação entre componentes com Input e Output
- tratamento de datas e timezone
- controle de estado visual no frontend
- testes unitários de regras de domínio

---

## Próximos passos

- Criar fluxo de caixa da reserva
- Adicionar paginação e filtros na listagem de reservas
- Criar usuários e níveis de autenticação
- Melhorar responsividade
- Separar melhor responsabilidades em componentes menores no frontend
- Melhorar tratamento global de erros no backend

---

## Licença

Projeto desenvolvido para fins de estudo e portfólio.
