# Hotel Reservation System

[![CI](https://github.com/Khallarrary/hotel-reservation-system/actions/workflows/ci.yml/badge.svg)](https://github.com/Khallarrary/hotel-reservation-system/actions/workflows/ci.yml)
![.NET](https://img.shields.io/badge/.NET-10-512BD4)
![Angular](https://img.shields.io/badge/Angular-21-DD0031)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-4169E1)
![Azure](https://img.shields.io/badge/Azure-Deploy-0078D4)

Sistema web para operação de pequenas pousadas e hotéis, desenvolvido com **ASP.NET Core**, **Angular** e **PostgreSQL**.

O projeto começou como uma aplicação de estudos e evoluiu para uma base de produto SaaS multi-hotel, reunindo mapa de ocupação, reservas, caixa, usuários, autenticação, isolamento de dados e entrega automatizada na nuvem.

> **Status:** em desenvolvimento, com uma demonstração técnica publicada no Azure.

## Demo

- **Aplicação:** [hotelapp-front-demo-3567.azurewebsites.net](https://hotelapp-front-demo-3567.azurewebsites.net)
- **API base (consumida pelo frontend):** `https://hotelapp-api-demo-8066.azurewebsites.net`

O ambiente online é destinado à validação técnica e pode receber alterações durante o desenvolvimento. A documentação Swagger fica disponível somente no ambiente local.

## Funcionalidades

### Reservas e operação

- mapa de reservas em formato de timeline por quarto e período;
- criação de reservas com validação de datas e conflitos de ocupação;
- criação da reserva e de sua conta financeira na mesma transação;
- fluxo de status `Pendente`, `CheckIn`, `CheckOut` e `Cancelada`;
- reservas canceladas preservadas no histórico, removidas do mapa e desconsideradas em novos conflitos;
- busca com filtros por hóspede, status, número do quarto e identificador da reserva;
- paginação realizada no backend;
- validações de data baseadas no fuso horário configurado para cada hotel.

### Quartos

- cadastro e consulta de quartos;
- numeração única dentro de cada hotel;
- bloqueio de remoção quando existem reservas vinculadas;
- isolamento de quartos por hotel autenticado.

### Caixa da reserva

- conta criada automaticamente junto com a reserva;
- lançamentos de crédito e débito;
- formas de pagamento para créditos;
- cálculo de débitos, créditos e saldo;
- histórico de lançamentos;
- encerramento permitido somente com saldo zerado;
- bloqueio de novos lançamentos em contas encerradas.

### Usuários e segurança

- autenticação com JWT e senhas armazenadas por hash;
- perfis `Master`, `Gestor` e `Operador`;
- autorização de endpoints por perfil;
- gerenciamento de usuários pelo Gestor;
- cadastro, edição, ativação e desativação de usuários;
- proteção contra alteração indevida do próprio perfil ou desativação do próprio usuário;
- Angular Guard para rotas protegidas e interceptor para envio do token;
- chave JWT mantida fora do repositório por User Secrets e configurações seguras do Azure.

## Multi-hotel

A aplicação utiliza uma arquitetura **multi-tenant compartilhada**, na qual os registros possuem um `HotelId` e o hotel atual é identificado por uma claim no JWT.

Esse contexto é aplicado nas consultas de usuários, quartos, reservas e contas para impedir acesso cruzado entre hotéis. O perfil `Master` administra a criação dos hotéis e seus primeiros gestores, enquanto `Gestor` e `Operador` permanecem vinculados a um único hotel.

## Arquitetura

O backend está dividido em projetos com responsabilidades distintas:

```text
HotelApp.Domain          Entidades, enums e regras de negócio
HotelApp.Application     DTOs, contratos e serviços de aplicação
HotelApp.Infrastructure  Entity Framework, PostgreSQL e implementações externas
HotelApp.Api             Controllers, autenticação e pipeline HTTP
HotelApp.Tests           Testes automatizados de domínio e serviços
```

O frontend Angular utiliza componentes standalone, services para integração HTTP, guards, interceptor de autenticação e páginas separadas por fluxo operacional.

Entre as decisões aplicadas estão Repository Pattern, injeção de dependência, DTOs, abstração do contexto do hotel, abstração de relógio por fuso horário e transação explícita na criação de reserva e conta.

## Tecnologias

### Backend

- .NET 10 e C#;
- ASP.NET Core Web API;
- Entity Framework Core;
- PostgreSQL com Npgsql;
- autenticação JWT Bearer;
- Swagger / OpenAPI;
- xUnit e FluentAssertions.

### Frontend

- Angular 21;
- TypeScript;
- RxJS;
- HTML e CSS;
- componentes standalone.

### Infraestrutura

- Docker para empacotamento da API;
- GitHub Actions para CI/CD;
- Azure Container Registry;
- Azure App Service para API e frontend;
- Azure Database for PostgreSQL Flexible Server;
- autenticação do pipeline no Azure por OpenID Connect.

## Qualidade e entrega

O workflow do GitHub Actions é executado em pushes e pull requests para a `main`:

1. restaura, compila e executa os testes do backend em Release;
2. instala as dependências e gera o build de produção do frontend;
3. após merge na `main`, publica uma nova imagem da API no ACR;
4. atualiza a API e o frontend hospedados no Azure.

A suíte atual possui **41 testes automatizados**. A branch principal também utiliza proteção para impedir merge quando as verificações obrigatórias falham.

## Como executar localmente

### Pré-requisitos

- .NET SDK 10;
- Node.js 22 e npm;
- PostgreSQL;
- Entity Framework CLI (`dotnet-ef`).

### Backend

Na raiz do repositório:

```powershell
cd hotel-reservation-system-back

$connection = "Host=localhost;Port=5432;Database=hotelapp;Username=postgres;Password=sua_senha"
$env:HOTELAPP_CONNECTION = $connection

dotnet user-secrets set "ConnectionStrings:DefaultConnection" $connection --project HotelApp.Api
dotnet user-secrets set "Jwt:Key" "uma-chave-local-com-pelo-menos-32-caracteres" --project HotelApp.Api

dotnet ef database update --project HotelApp.Infrastructure --startup-project HotelApp.Api
dotnet run --project HotelApp.Api
```

O Swagger estará disponível em `https://localhost:7265/swagger`.

### Frontend

Em outro terminal, a partir da raiz do repositório:

```powershell
cd hotel-reservation-system-front/hotel-app-front
npm ci
npm start
```

A aplicação estará disponível em `http://localhost:4200`.

### Testes e builds

```powershell
dotnet test hotel-reservation-system-back/HotelApp.slnx --configuration Release

cd hotel-reservation-system-front/hotel-app-front
npm run build
```

### Docker

A imagem da API pode ser criada a partir da pasta do backend:

```powershell
cd hotel-reservation-system-back
docker build -t hotelapp-api:dev .
```

As configurações de banco e JWT devem ser fornecidas ao container por variáveis de ambiente. Nenhuma credencial de execução é versionada no repositório.

## Próximos passos

- garantir idempotência e proteção contra concorrência na criação de reservas;
- bloquear movimentações financeiras em reservas canceladas;
- criar tarifário por categoria de quarto;
- implementar rotina de diárias;
- permitir edição de reservas;
- criar cadastro de hóspedes e vínculo com reservas;
- desenvolver dashboard operacional;
- evoluir o módulo de solicitações de hóspedes.

## Objetivo do projeto

Além de consolidar conhecimentos fullstack, o projeto busca modelar problemas reais da hotelaria e servir como base para uma futura solução comercial voltada a pequenas pousadas.

Projeto desenvolvido para estudo e portfólio.
