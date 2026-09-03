using FluentAssertions;
using HotelApp.Application.DTOs;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Services;
using HotelApp.Domain;

public class ReservaServiceTests
{
    private static readonly DateOnly DataAtual = new(2030, 4, 1);

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
            transacao,
            new ConsultaSaldoContaFake(0m),
            new RelogioHotelFake(DataAtual),
            new HotelRepositoryFake(hotelId));

        await service.CriarReserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 3),
            "Hospede Teste",
            quartoId,
            Guid.NewGuid());

        transacao.QuantidadeExecucoes.Should().Be(1);
        reservaRepo.ReservaAdicionada.Should().NotBeNull();
        contaRepo.ContaAdicionada.Should().NotBeNull();
        contaRepo.ContaAdicionada!.ReservaId.Should().Be(reservaIdGerado);
    }

    [Fact]
    public async Task Nao_Deve_Criar_Novamente_Ao_Repetir_Mesma_Chave_E_Dados()
    {
        const int hotelId = 1;
        const int quartoId = 10;
        var chaveIdempotencia = Guid.NewGuid();
        var reservaRepo = new ReservaRepositoryFake(25);
        var contaRepo = new ContaReservaRepositoryFake();
        var transacao = new TransacaoFake();
        var service = CriarServiceParaCriacao(
            reservaRepo,
            contaRepo,
            transacao,
            hotelId,
            quartoId);

        await service.CriarReserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 3),
            "Hospede Teste",
            quartoId,
            chaveIdempotencia);

        await service.CriarReserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 3),
            "Hospede Teste",
            quartoId,
            chaveIdempotencia);

        transacao.QuantidadeExecucoes.Should().Be(1);
        contaRepo.QuantidadeAdicoes.Should().Be(1);
    }

    [Fact]
    public async Task Nao_Deve_Reutilizar_Mesma_Chave_Com_Dados_Diferentes()
    {
        const int hotelId = 1;
        const int quartoId = 10;
        var chaveIdempotencia = Guid.NewGuid();
        var reservaRepo = new ReservaRepositoryFake(25);
        var contaRepo = new ContaReservaRepositoryFake();
        var transacao = new TransacaoFake();
        var service = CriarServiceParaCriacao(
            reservaRepo,
            contaRepo,
            transacao,
            hotelId,
            quartoId);

        await service.CriarReserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 3),
            "Hospede Teste",
            quartoId,
            chaveIdempotencia);

        Func<Task> action = () => service.CriarReserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 3),
            "Outro Hospede",
            quartoId,
            chaveIdempotencia);

        await action.Should().ThrowAsync<ConflictException>();
        transacao.QuantidadeExecucoes.Should().Be(1);
        contaRepo.QuantidadeAdicoes.Should().Be(1);
    }

    [Fact]
    public async Task Nao_Deve_Criar_Reserva_Com_Chave_Idempotencia_Vazia()
    {
        const int hotelId = 1;
        const int quartoId = 10;
        var reservaRepo = new ReservaRepositoryFake(25);
        var contaRepo = new ContaReservaRepositoryFake();
        var transacao = new TransacaoFake();
        var service = CriarServiceParaCriacao(
            reservaRepo,
            contaRepo,
            transacao,
            hotelId,
            quartoId);

        Func<Task> action = () => service.CriarReserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 3),
            "Hospede Teste",
            quartoId,
            Guid.Empty);

        await action.Should().ThrowAsync<ArgumentException>();
        transacao.QuantidadeExecucoes.Should().Be(0);
        contaRepo.QuantidadeAdicoes.Should().Be(0);
    }

    [Fact]
    public async Task Deve_Tratar_Disputa_Concorrente_Da_Mesma_Chave_Como_Sucesso()
    {
        const int hotelId = 1;
        const int quartoId = 10;
        var reservaRepo = new ReservaRepositoryFake(25)
        {
            SimularChaveIdempotenciaDuplicada = true
        };
        var contaRepo = new ContaReservaRepositoryFake();
        var transacao = new TransacaoFake();
        var service = CriarServiceParaCriacao(
            reservaRepo,
            contaRepo,
            transacao,
            hotelId,
            quartoId);

        Func<Task> action = () => service.CriarReserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 3),
            "Hospede Teste",
            quartoId,
            Guid.NewGuid());

        await action.Should().NotThrowAsync();
        transacao.QuantidadeExecucoes.Should().Be(1);
        contaRepo.QuantidadeAdicoes.Should().Be(0);
    }

    [Fact]
    public async Task Deve_Lancar_Conflito_Quando_Periodo_For_Reservado_Por_Outra_Solicitacao()
    {
        const int hotelId = 1;
        const int quartoId = 10;
        var reservaRepo = new ReservaRepositoryFake(25)
        {
            SimularConflitoPeriodo = true
        };
        var contaRepo = new ContaReservaRepositoryFake();
        var transacao = new TransacaoFake();
        var service = CriarServiceParaCriacao(
            reservaRepo,
            contaRepo,
            transacao,
            hotelId,
            quartoId);

        Func<Task> action = () => service.CriarReserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 3),
            "Hospede Teste",
            quartoId,
            Guid.NewGuid());

        await action.Should().ThrowAsync<ConflictException>();
        transacao.QuantidadeExecucoes.Should().Be(1);
        contaRepo.QuantidadeAdicoes.Should().Be(0);
    }

    [Fact]
    public async Task Deve_Tratar_Conflito_Periodo_Da_Mesma_Chave_Como_Sucesso_Idempotente()
    {
        const int hotelId = 1;
        const int quartoId = 10;
        var reservaRepo = new ReservaRepositoryFake(25)
        {
            SimularConflitoPeriodo = true,
            RegistrarReservaAntesDoConflitoPeriodo = true
        };
        var contaRepo = new ContaReservaRepositoryFake();
        var transacao = new TransacaoFake();
        var service = CriarServiceParaCriacao(
            reservaRepo,
            contaRepo,
            transacao,
            hotelId,
            quartoId);

        Func<Task> action = () => service.CriarReserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 3),
            "Hospede Teste",
            quartoId,
            Guid.NewGuid());

        await action.Should().NotThrowAsync();
        transacao.QuantidadeExecucoes.Should().Be(1);
        contaRepo.QuantidadeAdicoes.Should().Be(0);
    }

    [Fact]
    public async Task Deve_Cancelar_Reserva_E_Encerrar_Conta_Quando_Saldo_Estiver_Zerado()
    {
        const int hotelId = 1;
        const int reservaId = 25;
        var reserva = CriarReserva(reservaId, hotelId);
        var conta = new ContaReserva(reservaId);
        var reservaRepo = new ReservaRepositoryFake(reservaId, reserva);
        var contaRepo = new ContaReservaRepositoryFake(conta);
        var transacao = new TransacaoFake();
        var service = CriarService(
            reservaRepo,
            hotelId,
            new ConsultaSaldoContaFake(0m),
            contaRepo,
            transacao);

        await service.CancelarReserva(reservaId);

        reserva.Status.Should().Be(ReservaStatus.Cancelada);
        conta.Status.Should().Be(ContaStatus.Encerrada);
        reservaRepo.ReservaAtualizada.Should().BeSameAs(reserva);
        contaRepo.ContaAtualizada.Should().BeSameAs(conta);
        transacao.QuantidadeExecucoes.Should().Be(1);
    }

    [Fact]
    public async Task Nao_Deve_Cancelar_Reserva_Quando_Saldo_For_Diferente_De_Zero()
    {
        const int hotelId = 1;
        const int reservaId = 25;
        var reserva = CriarReserva(reservaId, hotelId);
        var conta = new ContaReserva(reservaId);
        var reservaRepo = new ReservaRepositoryFake(reservaId, reserva);
        var contaRepo = new ContaReservaRepositoryFake(conta);
        var transacao = new TransacaoFake();
        var service = CriarService(
            reservaRepo,
            hotelId,
            new ConsultaSaldoContaFake(50m),
            contaRepo,
            transacao);

        Func<Task> action = () => service.CancelarReserva(reservaId);

        await action.Should().ThrowAsync<ConflictException>();
        reserva.Status.Should().Be(ReservaStatus.Confirmada);
        conta.Status.Should().Be(ContaStatus.Aberta);
        reservaRepo.ReservaAtualizada.Should().BeNull();
        contaRepo.ContaAtualizada.Should().BeNull();
        transacao.QuantidadeExecucoes.Should().Be(0);
    }

    [Fact]
    public async Task Nao_Deve_Cancelar_Reserva_Quando_Conta_Nao_Existir()
    {
        const int hotelId = 1;
        const int reservaId = 25;
        var reserva = CriarReserva(reservaId, hotelId);
        var reservaRepo = new ReservaRepositoryFake(reservaId, reserva);
        var consultaSaldo = new ConsultaSaldoContaFake(0m);
        var transacao = new TransacaoFake();
        var service = CriarService(
            reservaRepo,
            hotelId,
            consultaSaldo,
            new ContaReservaRepositoryFake(),
            transacao);

        Func<Task> action = () => service.CancelarReserva(reservaId);

        await action.Should().ThrowAsync<NotFoundException>();
        reserva.Status.Should().Be(ReservaStatus.Confirmada);
        consultaSaldo.QuantidadeConsultas.Should().Be(0);
        reservaRepo.ReservaAtualizada.Should().BeNull();
        transacao.QuantidadeExecucoes.Should().Be(0);
    }

    [Fact]
    public async Task Nao_Deve_Cancelar_Reserva_De_Outro_Hotel()
    {
        const int hotelIdAutenticado = 1;
        const int reservaId = 25;
        var reservaOutroHotel = CriarReserva(reservaId, hotelId: 2);
        var reservaRepo = new ReservaRepositoryFake(reservaId, reservaOutroHotel);
        var consultaSaldo = new ConsultaSaldoContaFake(0m);
        var service = CriarService(reservaRepo, hotelIdAutenticado, consultaSaldo);

        Func<Task> action = () => service.CancelarReserva(reservaId);

        await action.Should().ThrowAsync<NotFoundException>();
        consultaSaldo.QuantidadeConsultas.Should().Be(0);
        reservaRepo.ReservaAtualizada.Should().BeNull();
    }

    private static ReservaService CriarService(
        ReservaRepositoryFake reservaRepo,
        int hotelId,
        IConsultaSaldoConta consultaSaldo,
        ContaReservaRepositoryFake? contaRepo = null,
        TransacaoFake? transacao = null)
    {
        return new ReservaService(
            reservaRepo,
            new QuartoRepositoryFake(new Quarto("101", "Luxo", hotelId), 10),
            contaRepo ?? new ContaReservaRepositoryFake(),
            new HotelContextoFake(hotelId),
            transacao ?? new TransacaoFake(),
            consultaSaldo,
            new RelogioHotelFake(DataAtual),
            new HotelRepositoryFake(hotelId));
    }

    private static ReservaService CriarServiceParaCriacao(
        ReservaRepositoryFake reservaRepo,
        ContaReservaRepositoryFake contaRepo,
        TransacaoFake transacao,
        int hotelId,
        int quartoId)
    {
        return new ReservaService(
            reservaRepo,
            new QuartoRepositoryFake(new Quarto("101", "Luxo", hotelId), quartoId),
            contaRepo,
            new HotelContextoFake(hotelId),
            transacao,
            new ConsultaSaldoContaFake(0m),
            new RelogioHotelFake(DataAtual),
            new HotelRepositoryFake(hotelId));
    }

    private static Reserva CriarReserva(int reservaId, int hotelId)
    {
        var reserva = new Reserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 3),
            "Hospede Teste",
            10,
            hotelId,
            DataAtual);

        typeof(Reserva)
            .GetProperty(nameof(Reserva.Id))!
            .SetValue(reserva, reservaId);

        return reserva;
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

    private class RelogioHotelFake : IRelogioHotel
    {
        private readonly DateOnly _dataAtual;

        public RelogioHotelFake(DateOnly dataAtual)
        {
            _dataAtual = dataAtual;
        }

        public DateOnly ObterDataAtual(string fusoHorario) => _dataAtual;
    }

    private class HotelRepositoryFake : IHotelRepository
    {
        private readonly int _hotelId;
        private readonly Hotel _hotel = new(
            "Hotel Teste",
            "12.345.678/0001-90",
            "America/Sao_Paulo");

        public HotelRepositoryFake(int hotelId)
        {
            _hotelId = hotelId;
        }

        public Task<Hotel?> ObterPorIdAsync(int id) =>
            Task.FromResult<Hotel?>(id == _hotelId ? _hotel : null);

        public Task AdicionarAsync(Hotel hotel) => Task.CompletedTask;

        public Task<Hotel?> ObterPorDocumentoAsync(string documento) =>
            Task.FromResult<Hotel?>(null);

        public Task<List<Hotel>> ListarAsync() =>
            Task.FromResult(new List<Hotel>());

        public Task AtualizarAsync(Hotel hotel) => Task.CompletedTask;
    }

    private class ConsultaSaldoContaFake : IConsultaSaldoConta
    {
        private readonly decimal _saldo;

        public int QuantidadeConsultas { get; private set; }

        public ConsultaSaldoContaFake(decimal saldo)
        {
            _saldo = saldo;
        }

        public Task<decimal> ObterSaldoAsync(int reservaId)
        {
            QuantidadeConsultas++;
            return Task.FromResult(_saldo);
        }
    }

    private class ReservaRepositoryFake : IReservaRepository
    {
        private readonly int _reservaIdGerado;

        private Reserva? _reservaExistente;

        public Reserva? ReservaAdicionada { get; private set; }
        public Reserva? ReservaAtualizada { get; private set; }
        public bool SimularChaveIdempotenciaDuplicada { get; set; }
        public bool SimularConflitoPeriodo { get; set; }
        public bool RegistrarReservaAntesDoConflitoPeriodo { get; set; }

        public ReservaRepositoryFake(int reservaIdGerado, Reserva? reservaExistente = null)
        {
            _reservaIdGerado = reservaIdGerado;
            _reservaExistente = reservaExistente;
        }

        public Task AdicionarReservaAsync(Reserva reserva)
        {
            typeof(Reserva)
                .GetProperty(nameof(Reserva.Id))!
                .SetValue(reserva, _reservaIdGerado);

            if (SimularChaveIdempotenciaDuplicada)
            {
                SimularChaveIdempotenciaDuplicada = false;
                _reservaExistente = reserva;

                throw new ChaveIdempotenciaDuplicadaException(
                    "A tentativa de criação da reserva já foi processada.",
                    new InvalidOperationException());
            }

            if (SimularConflitoPeriodo)
            {
                if (RegistrarReservaAntesDoConflitoPeriodo)
                {
                    _reservaExistente = reserva;
                }

                throw new ConflitoPeriodoReservaException(
                    "O quarto já possui uma reserva no período informado.",
                    new InvalidOperationException());
            }

            ReservaAdicionada = reserva;
            _reservaExistente = reserva;
            return Task.CompletedTask;
        }

        public Task<List<Reserva>> ObterReservasPorQuartoAsync(int quartoId, int hotelId) =>
            Task.FromResult(new List<Reserva>());

        public Task<List<Reserva>> ListarReservasAsync(int hotelId) =>
            Task.FromResult(new List<Reserva>());

        public Task DeletarReservaAsync(Reserva reserva) => Task.CompletedTask;

        public Task<Reserva?> ObterReservaPorIdAsync(int id, int hotelId) =>
            Task.FromResult<Reserva?>(
                _reservaExistente?.Id == id && _reservaExistente.HotelId == hotelId
                    ? _reservaExistente
                    : null);

        public Task<Reserva?> ObterPorChaveIdempotenciaAsync(
            Guid chaveIdempotencia,
            int hotelId) =>
            Task.FromResult<Reserva?>(
                _reservaExistente?.ChaveIdempotencia == chaveIdempotencia &&
                _reservaExistente.HotelId == hotelId
                    ? _reservaExistente
                    : null);

        public Task AtualizarReservaAsync(Reserva reserva)
        {
            ReservaAtualizada = reserva;
            return Task.CompletedTask;
        }

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
        private readonly ContaReserva? _contaExistente;

        public ContaReserva? ContaAdicionada { get; private set; }
        public ContaReserva? ContaAtualizada { get; private set; }
        public int QuantidadeAdicoes { get; private set; }

        public ContaReservaRepositoryFake(ContaReserva? contaExistente = null)
        {
            _contaExistente = contaExistente;
        }

        public Task AdicionarAsync(ContaReserva conta)
        {
            QuantidadeAdicoes++;
            ContaAdicionada = conta;
            return Task.CompletedTask;
        }

        public Task<ContaReserva?> ObterPorReservaIdAsync(int reservaId) =>
            Task.FromResult<ContaReserva?>(
                _contaExistente?.ReservaId == reservaId ? _contaExistente : null);

        public Task<ContaReserva?> ObterPorIdAsync(int id) =>
            Task.FromResult<ContaReserva?>(null);

        public Task AtualizarAsync(ContaReserva conta)
        {
            ContaAtualizada = conta;
            return Task.CompletedTask;
        }
    }
}
