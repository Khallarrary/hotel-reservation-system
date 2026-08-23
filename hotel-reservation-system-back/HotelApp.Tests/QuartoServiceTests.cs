using FluentAssertions;
using HotelApp.Application.DTOs;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Services;
using HotelApp.Domain;

public class QuartoServiceTests
{
    private static readonly DateOnly DataAtual = new(2030, 4, 1);

    [Fact]
    public async Task ObterTodos_Nao_Deve_Incluir_Reserva_Cancelada_No_Mapa()
    {
        var quarto = new Quarto("101", "Luxo", 1);
        typeof(Quarto)
            .GetProperty(nameof(Quarto.Id))!
            .SetValue(quarto, 10);

        var reservaAtiva = new Reserva(
            new DateTime(2030, 4, 10),
            new DateTime(2030, 4, 12),
            "Maria",
            10,
            1,
            DataAtual
        );

        var reservaCancelada = new Reserva(
            new DateTime(2030, 4, 15),
            new DateTime(2030, 4, 17),
            "Joao",
            10,
            1,
            DataAtual
        );
        reservaCancelada.Cancelar();

        var service = new QuartoService(
            new QuartoRepositoryFake(quarto),
            new ReservaRepositoryFake([reservaAtiva, reservaCancelada]),
            new HotelContextoFake(1)
        );

        var resultado = await service.ObterTodos();

        resultado.Should().ContainSingle();
        resultado[0].ReservaList.Should().ContainSingle();
        resultado[0].ReservaList![0].NomeDoHospede.Should().Be("Maria");
    }

    private sealed class HotelContextoFake : IHotelContexto
    {
        private readonly int? _hotelId;

        public HotelContextoFake(int? hotelId)
        {
            _hotelId = hotelId;
        }

        public int? ObterHotelId() => _hotelId;
    }

    private sealed class QuartoRepositoryFake : IQuartoRepository
    {
        private readonly Quarto _quarto;

        public QuartoRepositoryFake(Quarto quarto)
        {
            _quarto = quarto;
        }

        public Task<List<Quarto>> ObterTodosAsync(int hotelId) =>
            Task.FromResult(new List<Quarto> { _quarto });

        public Task<Quarto?> ObterPorIdAsync(int quartoId, int hotelId) =>
            Task.FromResult<Quarto?>(_quarto);

        public Task<Quarto?> ObterPorNumeroAsync(string numero, int hotelId) =>
            Task.FromResult<Quarto?>(_quarto);

        public Task<List<Quarto>> ObterPorIdsAsync(List<int> ids, int hotelId) =>
            Task.FromResult(new List<Quarto> { _quarto });

        public Task<bool> ExisteNumeroAsync(string numero, int hotelId) =>
            Task.FromResult(false);

        public Task AdicionarAsync(Quarto quarto) => Task.CompletedTask;

        public Task RemoverAsync(int quartoId, int hotelId) => Task.CompletedTask;
    }

    private sealed class ReservaRepositoryFake : IReservaRepository
    {
        private readonly List<Reserva> _reservas;

        public ReservaRepositoryFake(List<Reserva> reservas)
        {
            _reservas = reservas;
        }

        public Task<List<Reserva>> ObterReservasPorQuartoAsync(int quartoId, int hotelId) =>
            Task.FromResult(_reservas);

        public Task AdicionarReservaAsync(Reserva reserva) => Task.CompletedTask;

        public Task<List<Reserva>> ListarReservasAsync(int hotelId) =>
            Task.FromResult(_reservas);

        public Task DeletarReservaAsync(Reserva reserva) => Task.CompletedTask;

        public Task<Reserva?> ObterReservaPorIdAsync(int id, int hotelId) =>
            Task.FromResult<Reserva?>(null);

        public Task AtualizarReservaAsync(Reserva reserva) => Task.CompletedTask;

        public Task<int> ContarReservasAsync(ReservaConsultaDto consulta, int hotelId) =>
            Task.FromResult(_reservas.Count);

        public Task<List<Reserva>> ListarReservasPaginadasAsync(
            ReservaConsultaDto consulta,
            int hotelId) => Task.FromResult(_reservas);
    }
}
