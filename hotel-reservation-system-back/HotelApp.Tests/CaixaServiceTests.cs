using FluentAssertions;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Services;
using HotelApp.Domain;

public class CaixaServiceTests
{
    [Fact]
    public async Task Deve_Lancar_Erro_Quando_Conta_Nao_Existir_Ao_Encerrar()
    {
        var contaRepo = new ContaReservaRepositoryFake(null);
        var lancamentoRepo = new LancamentoContaRepositoryFake();
        var service = CriarService(lancamentoRepo, contaRepo);

        Func<Task> action = () => service.EncerrarConta(1);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Deve_Lancar_Erro_Quando_Saldo_For_Diferente_De_Zero_Ao_Encerrar()
    {
        var conta = new ContaReserva(1);
        DefinirId(conta, 1);
        var contaRepo = new ContaReservaRepositoryFake(conta);
        var lancamentoRepo = new LancamentoContaRepositoryFake(new List<LancamentoConta>
        {
            new LancamentoConta(1, LancamentoTipo.Debito, "Diaria", 200),
            new LancamentoConta(1, LancamentoTipo.Credito, "Pagamento", 100, FormaPagamento.Pix)
        });
        var service = CriarService(lancamentoRepo, contaRepo);

        Func<Task> action = () => service.EncerrarConta(1);

        await action.Should().ThrowAsync<ArgumentException>();
        conta.Status.Should().Be(ContaStatus.Aberta);
        contaRepo.Atualizou.Should().BeFalse();
    }

    [Fact]
    public async Task Deve_Encerrar_Conta_Quando_Saldo_For_Zero()
    {
        var conta = new ContaReserva(1);
        DefinirId(conta, 1);
        var contaRepo = new ContaReservaRepositoryFake(conta);
        var lancamentoRepo = new LancamentoContaRepositoryFake(new List<LancamentoConta>
        {
            new LancamentoConta(1, LancamentoTipo.Debito, "Diaria", 200),
            new LancamentoConta(1, LancamentoTipo.Credito, "Pagamento", 200, FormaPagamento.Pix)
        });
        var service = CriarService(lancamentoRepo, contaRepo);

        await service.EncerrarConta(1);

        conta.Status.Should().Be(ContaStatus.Encerrada);
        conta.DataEncerramento.Should().NotBeNull();
        contaRepo.Atualizou.Should().BeTrue();
    }

    private static void DefinirId(ContaReserva conta, int id)
    {
        typeof(ContaReserva)
            .GetProperty(nameof(ContaReserva.Id))!
            .SetValue(conta, id);
    }

    [Fact]
    public async Task Deve_Bloquear_Acesso_Quando_Reserva_For_De_Outro_Hotel()
    {
        var contaRepo = new ContaReservaRepositoryFake(new ContaReserva(1));
        var lancamentoRepo = new LancamentoContaRepositoryFake();
        var reserva = CriarReserva(hotelId: 1);
        var service = CriarService(
            lancamentoRepo,
            contaRepo,
            hotelIdAutenticado: 2,
            reserva);

        Func<Task> action = () => service.ResumoCaixa(1);

        await action.Should().ThrowAsync<NotFoundException>();
        contaRepo.ConsultouPorReserva.Should().BeFalse();
    }

    private static CaixaService CriarService(
        ILancamentoContaRepository lancamentoRepo,
        IContaReservaRepository contaRepo,
        int hotelIdAutenticado = 1,
        Reserva? reserva = null)
    {
        reserva ??= CriarReserva(hotelIdAutenticado);

        return new CaixaService(
            lancamentoRepo,
            contaRepo,
            new HotelContextoFake(hotelIdAutenticado),
            new ReservaRepositoryFake(reserva));
    }

    private static Reserva CriarReserva(int hotelId)
    {
        return new Reserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 3),
            "Hospede Teste",
            quartoId: 1,
            hotelId,
            new DateOnly(2030, 4, 1));
    }

    private class ContaReservaRepositoryFake : IContaReservaRepository
    {
        private readonly ContaReserva? _conta;

        public bool Atualizou { get; private set; }
        public bool ConsultouPorReserva { get; private set; }

        public ContaReservaRepositoryFake(ContaReserva? conta)
        {
            _conta = conta;
        }

        public Task AdicionarAsync(ContaReserva conta)
        {
            return Task.CompletedTask;
        }

        public Task<ContaReserva?> ObterPorReservaIdAsync(int reservaId)
        {
            ConsultouPorReserva = true;
            return Task.FromResult(_conta);
        }

        public Task<ContaReserva?> ObterPorIdAsync(int id)
        {
            return Task.FromResult(_conta);
        }

        public Task AtualizarAsync(ContaReserva conta)
        {
            Atualizou = true;
            return Task.CompletedTask;
        }
    }

    private class HotelContextoFake : IHotelContexto
    {
        private readonly int? _hotelId;

        public HotelContextoFake(int? hotelId)
        {
            _hotelId = hotelId;
        }

        public int? ObterHotelId()
        {
            return _hotelId;
        }
    }

    private class ReservaRepositoryFake : IReservaRepository
    {
        private readonly Reserva? _reserva;

        public ReservaRepositoryFake(Reserva? reserva)
        {
            _reserva = reserva;
        }

        public Task<Reserva?> ObterReservaPorIdAsync(int id, int hotelId)
        {
            var reserva = _reserva?.HotelId == hotelId ? _reserva : null;
            return Task.FromResult(reserva);
        }

        public Task<Reserva?> ObterPorChaveIdempotenciaAsync(
            Guid chaveIdempotencia,
            int hotelId)
        {
            var reserva = _reserva?.ChaveIdempotencia == chaveIdempotencia &&
                          _reserva.HotelId == hotelId
                ? _reserva
                : null;

            return Task.FromResult(reserva);
        }

        public Task<List<Reserva>> ObterReservasPorQuartoAsync(int quartoId, int hotelId)
        {
            return Task.FromResult(new List<Reserva>());
        }

        public Task AdicionarReservaAsync(Reserva reserva)
        {
            return Task.CompletedTask;
        }

        public Task<List<Reserva>> ListarReservasAsync(int hotelId)
        {
            return Task.FromResult(new List<Reserva>());
        }

        public Task DeletarReservaAsync(Reserva reserva)
        {
            return Task.CompletedTask;
        }

        public Task AtualizarReservaAsync(Reserva reserva)
        {
            return Task.CompletedTask;
        }

        public Task<int> ContarReservasAsync(
            HotelApp.Application.DTOs.ReservaConsultaDto consulta,
            int hotelId)
        {
            return Task.FromResult(0);
        }

        public Task<List<Reserva>> ListarReservasPaginadasAsync(
            HotelApp.Application.DTOs.ReservaConsultaDto consulta,
            int hotelId)
        {
            return Task.FromResult(new List<Reserva>());
        }
    }

    private class LancamentoContaRepositoryFake : ILancamentoContaRepository
    {
        private readonly List<LancamentoConta> _lancamentos;

        public LancamentoContaRepositoryFake()
            : this(new List<LancamentoConta>())
        {
        }

        public LancamentoContaRepositoryFake(List<LancamentoConta> lancamentos)
        {
            _lancamentos = lancamentos;
        }

        public Task AdicionarAsync(LancamentoConta lancamento)
        {
            _lancamentos.Add(lancamento);
            return Task.CompletedTask;
        }

        public Task<List<LancamentoConta>> ListarPorContaReservaIdAsync(int contaReservaId)
        {
            return Task.FromResult(_lancamentos
                .Where(l => l.ContaReservaId == contaReservaId)
                .ToList());
        }

    }
}
