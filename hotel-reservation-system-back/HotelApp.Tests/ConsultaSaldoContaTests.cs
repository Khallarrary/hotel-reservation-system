using FluentAssertions;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Services;
using HotelApp.Domain;

public class ConsultaSaldoContaTests
{
    [Fact]
    public async Task Deve_Calcular_Saldo_Da_Conta_Vinculada_A_Reserva()
    {
        const int reservaId = 25;
        const int contaId = 8;
        var contaRepo = new ContaReservaRepositoryFake(reservaId, contaId);
        var lancamentoRepo = new LancamentoContaRepositoryFake(
            new LancamentoConta(contaId, LancamentoTipo.Debito, "Diaria", 150m),
            new LancamentoConta(
                contaId,
                LancamentoTipo.Credito,
                "Pagamento",
                100m,
                FormaPagamento.Pix));
        var consulta = new ConsultaSaldoConta(contaRepo, lancamentoRepo);

        var saldo = await consulta.ObterSaldoAsync(reservaId);

        saldo.Should().Be(50m);
        contaRepo.ReservaIdConsultado.Should().Be(reservaId);
        lancamentoRepo.ContaIdConsultada.Should().Be(contaId);
    }

    [Fact]
    public async Task Deve_Lancar_Erro_Quando_Conta_Da_Reserva_Nao_Existir()
    {
        var consulta = new ConsultaSaldoConta(
            new ContaReservaRepositoryFake(),
            new LancamentoContaRepositoryFake());

        Func<Task> action = () => consulta.ObterSaldoAsync(25);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    private class ContaReservaRepositoryFake : IContaReservaRepository
    {
        private readonly ContaReserva? _conta;

        public int? ReservaIdConsultado { get; private set; }

        public ContaReservaRepositoryFake(int? reservaId = null, int contaId = 0)
        {
            if (!reservaId.HasValue)
            {
                return;
            }

            _conta = new ContaReserva(reservaId.Value);
            typeof(ContaReserva)
                .GetProperty(nameof(ContaReserva.Id))!
                .SetValue(_conta, contaId);
        }

        public Task<ContaReserva?> ObterPorReservaIdAsync(int reservaId)
        {
            ReservaIdConsultado = reservaId;
            return Task.FromResult(_conta?.ReservaId == reservaId ? _conta : null);
        }

        public Task AdicionarAsync(ContaReserva conta) => Task.CompletedTask;

        public Task<ContaReserva?> ObterPorIdAsync(int id) =>
            Task.FromResult<ContaReserva?>(null);

        public Task AtualizarAsync(ContaReserva conta) => Task.CompletedTask;
    }

    private class LancamentoContaRepositoryFake : ILancamentoContaRepository
    {
        private readonly List<LancamentoConta> _lancamentos;

        public int? ContaIdConsultada { get; private set; }

        public LancamentoContaRepositoryFake(params LancamentoConta[] lancamentos)
        {
            _lancamentos = lancamentos.ToList();
        }

        public Task AdicionarAsync(LancamentoConta lancamento) => Task.CompletedTask;

        public Task<List<LancamentoConta>> ListarPorContaReservaIdAsync(int contaReservaId)
        {
            ContaIdConsultada = contaReservaId;
            return Task.FromResult(
                _lancamentos
                    .Where(l => l.ContaReservaId == contaReservaId)
                    .ToList());
        }
    }
}
