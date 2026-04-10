# Hotel Reservation System

Sistema de gerenciamento de reservas de hotel com visualização em timeline (estilo PMS), desenvolvido com .NET e Angular.

---

## Funcionalidades

* Cadastro de quartos
* Criação de reservas com validação de conflitos
* Visualização de reservas em formato de mapa (timeline)
* Identificação visual de períodos ocupados por quarto
* API com tratamento global de exceções
* Integração completa entre backend e frontend

---

## Destaque do Projeto

O principal diferencial deste projeto é a implementação de um mapa de reservas com blocos contínuos, inspirado em sistemas profissionais de hotelaria (PMS).

As reservas são exibidas como blocos posicionados dinamicamente com base em:

* Data de check-in (offset)
* Duração da estadia (width)

---

## Tecnologias Utilizadas

### Backend

* .NET (ASP.NET Core)
* Entity Framework Core
* PostgreSQL
* Arquitetura em camadas (Domain, Application, Infrastructure)
* Middleware global para tratamento de exceções

### Frontend

* Angular (Standalone Components)
* TypeScript
* HTML e CSS (Grid e posicionamento absoluto)
* Consumo de API REST

---

## Estrutura da API

### Quartos

* GET /api/Quarto → Lista quartos com reservas
* POST /api/Quarto → Cria novo quarto
* DELETE /api/Quarto/{id} → Remove quarto

### Reservas

* GET /api/Reserva
* POST /api/Reserva

---

## Como rodar o projeto

### Backend

```bash
cd hotel-reservation-system
dotnet run
```

Acesse:

```
https://localhost:7265/swagger
```

---

### Frontend

```bash
cd hotel-app-front
ng serve
```

Acesse:

```
http://localhost:4200
```

---

## Regras de Negócio

* Não é permitido criar reservas com datas inválidas
* Não é permitido conflito de reservas no mesmo quarto
* Quartos devem possuir número e tipo válidos
* Tratamento de erros padronizado via middleware

---

## Aprendizados

Durante o desenvolvimento foram abordados:

* Integração frontend e backend
* Modelagem de domínio
* Validação de regras de negócio
* Manipulação de datas e intervalos
* Renderização dinâmica baseada em tempo (timeline)
* Debug de problemas reais (binding, validação e change detection)

---

## Próximos passos

* Criar reserva diretamente pelo mapa
* Melhorias de experiência do usuário
* Separação de DTOs por responsabilidade
* Implementação de autenticação

---

## Preview

Adicionar imagem do sistema

---

## Licença

Projeto desenvolvido para fins de estudo e portfólio.
