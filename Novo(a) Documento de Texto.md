# Hotel Reservation System

API backend para gerenciamento de reservas de hotel, desenvolvida com .NET seguindo princípios de Clean Architecture.

## Tecnologias utilizadas

- .NET / ASP.NET Core  
- Entity Framework Core  
- PostgreSQL  
- Swagger (documentação da API)

## Arquitetura

O projeto foi estruturado utilizando Clean Architecture, separando responsabilidades em camadas:

- Domain → Entidades e regras de negócio  
- Application → Serviços e interfaces  
- Infrastructure → Acesso a dados (EF Core, repositórios)  
- API → Controllers e exposição dos endpoints  

## Funcionalidades

- Cadastro de reservas  
- Validação de conflito de datas (não permite reservas sobrepostas)  
- Integração com banco de dados PostgreSQL  
- Estrutura preparada para CRUD de quartos  
- CRUD completo de quartos (em desenvolvimento)

## Como executar o projeto

### 1. Clonar o repositório

```bash
git clone https://github.com/Khallarrary/hotel-reservation-system.git
```

### 2. Configurar a conexão com o banco

No arquivo `appsettings.json`, configure sua connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=hotelapp;Username=seu_usuario;Password=sua_senha"
}
```

### 3. Aplicar as migrations

```bash
dotnet ef database update
```

### 4. Executar a aplicação

```bash
dotnet run
```

### 5. Acessar o Swagger

A API será aberta automaticamente em:

https://localhost:{porta}/swagger

## Exemplos de uso

### Criar uma reserva

```json
{
  "quartoId": 1,
  "checkIn": "2026-03-25",
  "checkOut": "2026-03-28",
  "nome": "João"
}
```

## Objetivo do projeto

Este projeto foi desenvolvido como parte do meu processo de transição de carreira para desenvolvimento de software, com foco em:

- boas práticas de backend  
- organização de código  
- arquitetura escalável  
- integração com banco de dados real  

## Próximos passos

- Implementar CRUD completo de quartos  
- Melhorar validações de negócio  
- Adicionar autenticação  
- Criar front-end (React) com mapa de reservas  

## Autor

Khallarrary  
https://github.com/Khallarrary

## Licença

Este projeto é para fins de estudo e portfólio.