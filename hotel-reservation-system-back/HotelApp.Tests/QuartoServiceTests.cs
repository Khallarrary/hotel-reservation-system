using FluentAssertions;
using HotelApp.Application.DTOs;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Services;
using HotelApp.Domain;

public class QuartoServiceTests
{
    private const int HotelId = 1;
    private const int CategoriaId = 5;
    private static readonly DateOnly DataAtual = new(2030, 4, 1);

    [Fact]
    public async Task ObterTodos_Nao_Deve_Incluir_Reserva_Cancelada_No_Mapa()
    {
        var quarto = CriarQuarto(10, "101");

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

        var service = CriarService(
            new QuartoRepositoryFake(quarto),
            CriarCategoria(),
            [reservaAtiva, reservaCancelada]
        );

        var resultado = await service.ObterTodos();

        resultado.Should().ContainSingle();
        resultado[0].ReservaList.Should().ContainSingle();
        resultado[0].ReservaList![0].NomeDoHospede.Should().Be("Maria");
    }

    [Fact]
    public async Task ObterTodos_Deve_Exibir_Nome_Da_Categoria_Inativa_Do_Quarto()
    {
        var quarto = new Quarto("101", CategoriaId, HotelId);
        var categoria = CriarCategoria();
        categoria.Desativar();
        var service = CriarService(new QuartoRepositoryFake(quarto), categoria);

        var resultado = await service.ObterTodos();

        resultado.Should().ContainSingle().Which.tipo.Should().Be("Luxo");
    }

    [Fact]
    public async Task Criar_Deve_Salvar_Quarto_Com_Numero_Normalizado_E_Categoria()
    {
        var repository = new QuartoRepositoryFake();
        var service = CriarService(repository, CriarCategoria());

        await service.Criar(" 101 ", CategoriaId);

        var quarto = (await repository.ObterTodosAsync(HotelId))
            .Should().ContainSingle().Which;
        quarto.Numero.Should().Be("101");
        quarto.CategoriaQuartoId.Should().Be(CategoriaId);
        quarto.HotelId.Should().Be(HotelId);
    }

    [Fact]
    public async Task Criar_Com_Numero_Duplicado_Apos_Normalizar_Nao_Deve_Salvar()
    {
        var existente = new Quarto("101", CategoriaId, HotelId);
        var repository = new QuartoRepositoryFake(existente);
        var service = CriarService(repository, CriarCategoria());

        Func<Task> acao = () => service.Criar(" 101 ", CategoriaId);

        await acao.Should().ThrowAsync<ConflictException>();
        (await repository.ObterTodosAsync(HotelId))
            .Should().ContainSingle().Which.Should().BeSameAs(existente);
    }

    [Theory]
    [InlineData(HotelId, false)]
    [InlineData(2, true)]
    public async Task Criar_Com_Categoria_Inativa_Ou_De_Outro_Hotel_Nao_Deve_Salvar(
        int hotelDaCategoria,
        bool ativa)
    {
        var categoria = CriarCategoria(hotelDaCategoria);
        if (!ativa)
        {
            categoria.Desativar();
        }

        var repository = new QuartoRepositoryFake();
        var service = CriarService(repository, categoria);

        Func<Task> acao = () => service.Criar("101", CategoriaId);

        await acao.Should().ThrowAsync<ArgumentException>();
        (await repository.ObterTodosAsync(HotelId)).Should().BeEmpty();
    }

    [Fact]
    public async Task ObterParaGestao_Deve_Listar_Dados_Dos_Quartos_Do_Hotel()
    {
        var repository = new QuartoRepositoryFake(
            CriarQuarto(10, "101"), CriarQuarto(20, "201", hotelId: 2));
        var service = CriarService(repository, CriarCategoria());

        var resultado = await service.ObterParaGestao();

        resultado.Should().ContainSingle().Which.Should()
            .Be(new QuartoGestaoDto(10, "101", CategoriaId, true));
    }

    [Fact]
    public async Task Atualizar_Deve_Salvar_Numero_Normalizado_E_Nova_Categoria()
    {
        var quarto = CriarQuarto(10, "101", categoriaId: 9);
        var repository = new QuartoRepositoryFake(quarto);
        var service = CriarService(repository, CriarCategoria());

        await service.Atualizar(10, " 102 ", CategoriaId);

        quarto.Numero.Should().Be("102");
        quarto.CategoriaQuartoId.Should().Be(CategoriaId);
        repository.AlteracoesSalvas.Should().Be(1);
    }

    [Fact]
    public async Task Atualizar_Deve_Permitir_Manter_A_Categoria_Inativa_E_O_Proprio_Numero()
    {
        var quarto = CriarQuarto(10, "101");
        var categoria = CriarCategoria();
        categoria.Desativar();
        var repository = new QuartoRepositoryFake(quarto);
        var service = CriarService(repository, categoria);

        await service.Atualizar(10, " 101 ", CategoriaId);

        quarto.CategoriaQuartoId.Should().Be(CategoriaId);
        repository.AlteracoesSalvas.Should().Be(1);
    }

    [Fact]
    public async Task Atualizar_Com_Numero_Duplicado_Nao_Deve_Alterar_O_Quarto()
    {
        var quarto = CriarQuarto(10, "101");
        var repository = new QuartoRepositoryFake(quarto, CriarQuarto(20, "102"));
        var service = CriarService(repository, CriarCategoria());

        Func<Task> acao = () => service.Atualizar(10, " 102 ", CategoriaId);

        await acao.Should().ThrowAsync<ConflictException>();
        quarto.Numero.Should().Be("101");
        repository.AlteracoesSalvas.Should().Be(0);
    }

    [Theory]
    [InlineData(HotelId, false)]
    [InlineData(2, true)]
    public async Task Atualizar_Nao_Deve_Trocar_Para_Categoria_Inativa_Ou_De_Outro_Hotel(
        int hotelDaCategoria,
        bool ativa)
    {
        var quarto = CriarQuarto(10, "101", categoriaId: 9);
        var categoria = CriarCategoria(hotelDaCategoria);
        if (!ativa)
        {
            categoria.Desativar();
        }

        var repository = new QuartoRepositoryFake(quarto);
        var service = CriarService(repository, categoria);

        Func<Task> acao = () => service.Atualizar(10, "102", CategoriaId);

        await acao.Should().ThrowAsync<ArgumentException>();
        quarto.Numero.Should().Be("101");
        quarto.CategoriaQuartoId.Should().Be(9);
        repository.AlteracoesSalvas.Should().Be(0);
    }

    [Fact]
    public async Task Atualizar_Quarto_De_Outro_Hotel_Deve_Lancar_NotFound()
    {
        var quarto = CriarQuarto(10, "101", hotelId: 2);
        var repository = new QuartoRepositoryFake(quarto);
        var service = CriarService(repository, CriarCategoria());

        Func<Task> acao = () => service.Atualizar(10, "102", CategoriaId);

        await acao.Should().ThrowAsync<NotFoundException>();
        quarto.Numero.Should().Be("101");
        repository.AlteracoesSalvas.Should().Be(0);
    }

    private static Quarto CriarQuarto(int id, string numero, int hotelId = HotelId, int categoriaId = CategoriaId)
    {
        var quarto = new Quarto(numero, categoriaId, hotelId);
        typeof(Quarto).GetProperty(nameof(Quarto.Id))!.SetValue(quarto, id);
        return quarto;
    }

    private static QuartoService CriarService(
        QuartoRepositoryFake repository,
        CategoriaQuarto categoria,
        List<Reserva>? reservas = null) =>
        new(
            repository,
            new ReservaRepositoryFake(reservas ?? []),
            new HotelContextoFake(HotelId),
            new CategoriaQuartoRepositoryFake(categoria));

    private static CategoriaQuarto CriarCategoria(int hotelId = HotelId)
    {
        var categoria = new CategoriaQuarto(hotelId, "Luxo", null, 2, 2);
        typeof(CategoriaQuarto)
            .GetProperty(nameof(CategoriaQuarto.Id))!
            .SetValue(categoria, CategoriaId);
        return categoria;
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
        private readonly List<Quarto> _quartos;
        public int AlteracoesSalvas { get; private set; }

        public QuartoRepositoryFake(params Quarto[] quartos)
        {
            _quartos = quartos.ToList();
        }

        public Task<List<Quarto>> ObterTodosAsync(int hotelId) =>
            Task.FromResult(_quartos.Where(q => q.HotelId == hotelId).ToList());

        public Task<Quarto?> ObterPorIdAsync(int quartoId, int hotelId) =>
            Task.FromResult(_quartos.FirstOrDefault(q => q.Id == quartoId && q.HotelId == hotelId));

        public Task<Quarto?> ObterPorNumeroAsync(string numero, int hotelId) =>
            Task.FromResult(_quartos.FirstOrDefault(q => q.Numero == numero && q.HotelId == hotelId));

        public Task<List<Quarto>> ObterPorIdsAsync(List<int> ids, int hotelId) =>
            Task.FromResult(_quartos.Where(q => ids.Contains(q.Id) && q.HotelId == hotelId).ToList());

        public Task<bool> ExisteNumeroAsync(string numero, int hotelId) =>
            Task.FromResult(_quartos.Any(q => q.Numero == numero && q.HotelId == hotelId));

        public Task AdicionarAsync(Quarto quarto)
        {
            _quartos.Add(quarto);
            return Task.CompletedTask;
        }

        public Task RemoverAsync(int quartoId, int hotelId) => Task.CompletedTask;

        public Task SalvarAlteracoesAsync()
        {
            AlteracoesSalvas++;
            return Task.CompletedTask;
        }
    }

    private sealed class CategoriaQuartoRepositoryFake(CategoriaQuarto categoria)
        : ICategoriaQuartoRepository
    {
        public Task<CategoriaQuarto?> ObterPorIdAsync(int categoriaId, int hotelId) =>
            Task.FromResult<CategoriaQuarto?>(
                categoria.Id == categoriaId && categoria.HotelId == hotelId ? categoria : null);

        public Task<List<CategoriaQuarto>> ObterTodasAsync(int hotelId) =>
            Task.FromResult<List<CategoriaQuarto>>(categoria.HotelId == hotelId ? [categoria] : []);

        public Task<List<CategoriaQuarto>> ObterAtivasAsync(int hotelId) =>
            Task.FromResult<List<CategoriaQuarto>>(
                categoria.HotelId == hotelId && categoria.Ativa ? [categoria] : []);

        public Task AdicionarAsync(CategoriaQuarto item) => throw new NotSupportedException();

        public Task AtualizarAsync(CategoriaQuarto item) => throw new NotSupportedException();

        public Task<bool> ExisteNomeAsync(string nome, int hotelId, int? categoriaIdIgnorada = null) =>
            throw new NotSupportedException();
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

        public Task<Reserva?> ObterPorChaveIdempotenciaAsync(
            Guid chaveIdempotencia,
            int hotelId) =>
            Task.FromResult<Reserva?>(_reservas.FirstOrDefault(r =>
                r.ChaveIdempotencia == chaveIdempotencia &&
                r.HotelId == hotelId));

        public Task AtualizarReservaAsync(Reserva reserva) => Task.CompletedTask;

        public Task<int> ContarReservasAsync(ReservaConsultaDto consulta, int hotelId) =>
            Task.FromResult(_reservas.Count);

        public Task<List<Reserva>> ListarReservasPaginadasAsync(
            ReservaConsultaDto consulta,
            int hotelId) => Task.FromResult(_reservas);
    }
}
