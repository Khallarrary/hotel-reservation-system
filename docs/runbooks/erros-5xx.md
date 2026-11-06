# Runbook: erros 5xx na HotelApp API

## Objetivo

Orientar o diagnostico e a recuperacao quando a API registrar respostas HTTP
5xx em producao.

## Alerta relacionado

- **Nome:** HotelApp API - Erros 5xx
- **Servico:** `HotelApp.Api`
- **Ambiente:** `Production`
- **Severidade inicial:** warning
- **Condicao planejada:** 3 ou mais respostas 5xx em 5 minutos

## Impacto

Usuarios podem nao conseguir consultar ou alterar quartos, reservas, check-ins,
check-outs ou lancamentos financeiros. O impacto pode estar restrito a uma rota
ou a um hotel.

## Diagnostico

1. Abra o dashboard `HotelApp API - Observabilidade` no Grafana.
2. Selecione `Ambiente = Production` e verifique o periodo do alerta.
3. Identifique as rotas afetadas e, quando presente, o `hotel_id`.
4. Consulte os logs da API no mesmo intervalo.
5. Localize o `traceId` das respostas com falha e analise a excecao associada.
6. Verifique se `/health/ready` responde HTTP 200.
7. Confira deploys recentes e o estado do PostgreSQL no Azure.

## Causas provaveis

- Regressao introduzida por um deploy recente.
- PostgreSQL indisponivel, bloqueado pelo firewall ou com conexao esgotada.
- Variavel de ambiente ausente ou incorreta.
- Excecao nao tratada em uma rota especifica.
- Dependencia externa indisponivel ou com timeout.

## Acoes

### Falha apos deploy

1. Compare o inicio dos erros com o horario do deploy.
2. Consulte os logs da nova versao.
3. Se a regressao estiver confirmada, execute o rollback pelo processo de deploy.

### Falha de banco de dados

1. Verifique o estado do PostgreSQL e a conectividade da aplicacao.
2. Confirme as regras de firewall e a connection string configurada.
3. Evite alterar configuracoes sem registrar o valor anterior.

### Falha em uma rota

1. Use o `traceId` para localizar a excecao e a operacao afetada.
2. Verifique se o problema ocorre em todos os hoteis ou apenas em um `hotel_id`.
3. Preserve os dados necessarios para reproduzir o erro sem expor segredos.

### Operacao financeira

Antes de repetir uma requisicao de caixa, confirme se a operacao anterior foi
persistida. Reutilize a mesma chave de idempotencia quando o fluxo permitir.

## Validacao da recuperacao

- `/health/ready` responde HTTP 200.
- Nao surgem novos erros 5xx durante 10 minutos.
- A rota afetada responde normalmente.
- O fluxo de negocio relacionado funciona do inicio ao fim.
- O alerta retorna ao estado `Normal`.

## Escalonamento

Se a causa nao for identificada, se o erro persistir apos rollback ou se houver
risco de inconsistencia financeira, interrompa a operacao afetada e preserve os
logs, trace IDs, horario e versao do deploy para investigacao.

