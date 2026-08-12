using FluentAssertions;
using HotelApp.Application.DTOs;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Services;
using HotelApp.Domain;

public class ReservaServiceTests
{
    [Fact]
    public async Task Deve_Criar_Reserva_E_Conta_Dentro_Da_Transacao()
    {
        const int hotelId = 1;
        const int quartoId = 10;
        const int reservaIdGerado = 25;

        var reservaRepo = new ReservaRepositoryFake(reservaIdGerado);
        var contaRepo = new ContaReservaRepositoryFake();
        var transacao = new TransacaoFake();
        var service = new ReservaService(
            reservaRepo,
            new QuartoRepositoryFake(new Quarto("101", "Luxo", hotelId), quartoId),
            contaRepo,
            new HotelContextoFake(hotelId),
            transacao);

        await service.CriarReserva(
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(2),
            "Hospede Teste",
            quartoId);

        transacao.QuantidadeExecucoes.Should().Be(1);
        reservaRepo.ReservaAdicionada.Should().NotBeNull();
        contaRepo.ContaAdicionada.Should().NotBeNull();
        contaRepo.ContaAdicionada!.ReservaId.Should().Be(reservaIdGerado);
    }

    private class TransacaoFake : ITransacao
    {
        public int QuantidadeExecucoes { get; private set; }

        public async Task ExecutarAsync(Func<Task> operacao)
        {
            QuantidadeExecucoes++;
            await operacao();
        }
    }

    private class HotelContextoFake : IHotelContexto
    {
        private readonly int _hotelId;

        public HotelContextoFake(int hotelId)
        {
            _hotelId = hotelId;
        }

        public int? ObterHotelId() => _hotelId;
    }

    private class ReservaRepositoryFake : IReservaRepository
    {
        private readonly int _reservaIdGerado;

        public Reserva? ReservaAdicionada { get; private set; }

        public ReservaRepositoryFake(int reservaIdGerado)
        {
            _reservaIdGerado = reservaIdGerado;
        }

        public Task AdicionarReservaAsync(Reserva reserva)
        {
            typeof(Reserva)
                .GetProperty(nameof(Reserva.Id))!
                .SetValue(reserva, _reservaIdGerado);

            ReservaAdicionada = reserva;
            return Task.CompletedTask;
        }

        public Task<List<Reserva>> ObterReservasPorQuartoAsync(int quartoId, int hotelId) =>
            Task.FromResult(new List<Reserva>());

        public Task<List<Reserva>> ListarReservasAsync(int hotelId) =>
            Task.FromResult(new List<Reserva>());

        public Task DeletarReservaAsync(Reserva reserva) => Task.CompletedTask;

        public Task<Reserva?> ObterReservaPorIdAsync(int id, int hotelId) =>
            Task.FromResult<Reserva?>(null);

        public Task AtualizarReservaAsync(Reserva reserva) => Task.CompletedTask;

        public Task<int> ContarReservasAsync(ReservaConsultaDto consulta, int hotelId) =>
            Task.FromResult(0);

        public Task<List<Reserva>> ListarReservasPaginadasAsync(
            ReservaConsultaDto consulta,
            int hotelId) => Task.FromResult(new List<Reserva>());
    }

    private class QuartoRepositoryFake : IQuartoRepository
    {
        private readonly Quarto _quarto;

        public QuartoRepositoryFake(Quarto quarto, int quartoId)
        {
            typeof(Quarto)
                .GetProperty(nameof(Quarto.Id))!
                .SetValue(quarto, quartoId);

            _quarto = quarto;
        }

        public Task<Quarto?> ObterPorIdAsync(int quartoId, int hotelId) =>
            Task.FromResult<Quarto?>(
                _quarto.Id == quartoId && _quarto.HotelId == hotelId ? _quarto : null);

        public Task<List<Quarto>> ObterTodosAsync(int hotelId) =>
            Task.FromResult(new List<Quarto>());

        public Task RemoverAsync(int quartoId, int hotelId) => Task.CompletedTask;

        public Task AdicionarAsync(Quarto quarto) => Task.CompletedTask;

        public Task<bool> ExisteNumeroAsync(string numero, int hotelId) =>
            Task.FromResult(false);

        public Task<Quarto?> ObterPorNumeroAsync(string numero, int hotelId) =>
            Task.FromResult<Quarto?>(null);

        public Task<List<Quarto>> ObterPorIdsAsync(List<int> ids, int hotelId) =>
            Task.FromResult(new List<Quarto>());
    }

    private class ContaReservaRepositoryFake : IContaReservaRepository
    {
        public ContaReserva? ContaAdicionada { get; private set; }

        public Task AdicionarAsync(ContaReserva conta)
        {
            ContaAdicionada = conta;
            return Task.CompletedTask;
        }

        public Task<ContaReserva?> ObterPorReservaIdAsync(int reservaId) =>
            Task.FromResult<ContaReserva?>(null);

        public Task<ContaReserva?> ObterPorIdAsync(int id) =>
            Task.FromResult<ContaReserva?>(null);

        public Task AtualizarAsync(ContaReserva conta) => Task.CompletedTask;
    }
}
