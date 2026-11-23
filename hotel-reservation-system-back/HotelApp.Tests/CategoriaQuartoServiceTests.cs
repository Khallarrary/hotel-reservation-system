using FluentAssertions;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Services;
using HotelApp.Domain;

namespace HotelApp.Tests;

public class CategoriaQuartoServiceTests
{
    [Fact]
    public async Task Criar_Deve_Adicionar_Categoria_Normalizada_No_Hotel_Do_Contexto()
    {
        var repository = new CategoriaQuartoRepositoryFake();
        var service = CriarService(repository, hotelId: 10);

        await service.Criar(
            "  Suite Master  ",
            "  Vista para o mar  ",
            4,
            3);

        repository.Adicionada.Should().NotBeNull();
        repository.Adicionada!.HotelId.Should().Be(10);
        repository.Adicionada.Nome.Should().Be("Suite Master");
        repository.Adicionada.Descricao.Should().Be("Vista para o mar");
        repository.Adicionada.CapacidadeFisica.Should().Be(4);
        repository.Adicionada.LimiteComercial.Should().Be(3);
        repository.UltimoNomeConsultado.Should().Be("Suite Master");
        repository.UltimoHotelIdConsultado.Should().Be(10);
    }

    [Fact]
    public async Task Criar_Sem_Hotel_No_Contexto_Deve_Lancar_ForbiddenException()
    {
        var repository = new CategoriaQuartoRepositoryFake();
        var service = CriarService(repository, hotelId: null);

        Func<Task> action = () => service.Criar(
            "Standard",
            null,
            2,
            2);

        await action.Should().ThrowAsync<ForbiddenException>();
        repository.ConsultasPorNome.Should().Be(0);
        repository.Adicionada.Should().BeNull();
    }

    [Fact]
    public async Task Criar_Quando_Nome_Ja_Existe_Deve_Lancar_ConflictException()
    {
        var existente = CriarCategoria(1, 10, "Standard");
        var repository = new CategoriaQuartoRepositoryFake(existente);
        var service = CriarService(repository, hotelId: 10);

        Func<Task> action = () => service.Criar(
            "  Standard  ",
            null,
            2,
            2);

        await action.Should().ThrowAsync<ConflictException>();
        repository.Adicionada.Should().BeNull();
    }

    [Fact]
    public async Task ObterTodas_Deve_Retornar_Dtos_Apenas_Do_Hotel_Do_Contexto()
    {
        var categoriaAtiva = CriarCategoria(1, 10, "Standard");
        var categoriaInativa = CriarCategoria(2, 10, "Luxo", ativa: false);
        var categoriaOutroHotel = CriarCategoria(3, 20, "Master");
        var repository = new CategoriaQuartoRepositoryFake(
            categoriaAtiva,
            categoriaInativa,
            categoriaOutroHotel);
        var service = CriarService(repository, hotelId: 10);

        var resultado = await service.ObterTodas();

        resultado.Should().HaveCount(2);
        resultado.Select(categoria => categoria.Id)
            .Should().BeEquivalentTo([1, 2]);
        resultado.Single(categoria => categoria.Id == 1)
            .Nome.Should().Be("Standard");
        resultado.Single(categoria => categoria.Id == 2)
            .Ativa.Should().BeFalse();
        repository.UltimoHotelIdConsultado.Should().Be(10);
    }

    [Fact]
    public async Task ObterTodas_Sem_Hotel_No_Contexto_Deve_Lancar_ForbiddenException()
    {
        var repository = new CategoriaQuartoRepositoryFake();
        var service = CriarService(repository, hotelId: null);

        Func<Task> action = () => service.ObterTodas();

        await action.Should().ThrowAsync<ForbiddenException>();
        repository.ConsultasTodas.Should().Be(0);
    }

    [Fact]
    public async Task ObterAtivas_Deve_Retornar_Somente_Categorias_Ativas_Do_Hotel()
    {
        var categoriaAtiva = CriarCategoria(1, 10, "Standard");
        var categoriaInativa = CriarCategoria(2, 10, "Luxo", ativa: false);
        var categoriaOutroHotel = CriarCategoria(3, 20, "Master");
        var repository = new CategoriaQuartoRepositoryFake(
            categoriaAtiva,
            categoriaInativa,
            categoriaOutroHotel);
        var service = CriarService(repository, hotelId: 10);

        var resultado = await service.ObterAtivas();

        resultado.Should().ContainSingle();
        resultado[0].Id.Should().Be(1);
        resultado[0].Nome.Should().Be("Standard");
        resultado[0].Ativa.Should().BeTrue();
        repository.UltimoHotelIdConsultado.Should().Be(10);
    }

    [Fact]
    public async Task ObterPorId_Deve_Retornar_Dto_Da_Categoria()
    {
        var categoria = CriarCategoria(
            5,
            10,
            "Suite Master",
            "Vista para o mar",
            4,
            3);
        var repository = new CategoriaQuartoRepositoryFake(categoria);
        var service = CriarService(repository, hotelId: 10);

        var resultado = await service.ObterPorId(5);

        resultado.Id.Should().Be(5);
        resultado.Nome.Should().Be("Suite Master");
        resultado.Descricao.Should().Be("Vista para o mar");
        resultado.CapacidadeFisica.Should().Be(4);
        resultado.LimiteComercial.Should().Be(3);
        resultado.Ativa.Should().BeTrue();
        repository.UltimoHotelIdConsultado.Should().Be(10);
    }

    [Fact]
    public async Task ObterPorId_Quando_Categoria_Nao_Existe_Deve_Lancar_NotFoundException()
    {
        var repository = new CategoriaQuartoRepositoryFake();
        var service = CriarService(repository, hotelId: 10);

        Func<Task> action = () => service.ObterPorId(99);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ObterPorId_Nao_Deve_Retornar_Categoria_De_Outro_Hotel()
    {
        var categoriaOutroHotel = CriarCategoria(5, 20, "Luxo");
        var repository = new CategoriaQuartoRepositoryFake(categoriaOutroHotel);
        var service = CriarService(repository, hotelId: 10);

        Func<Task> action = () => service.ObterPorId(5);

        await action.Should().ThrowAsync<NotFoundException>();
        repository.UltimoHotelIdConsultado.Should().Be(10);
    }

    [Fact]
    public async Task Atualizar_Deve_Alterar_Categoria_E_Ignorar_O_Proprio_Id_Na_Duplicidade()
    {
        var categoria = CriarCategoria(5, 10, "Standard");
        var repository = new CategoriaQuartoRepositoryFake(categoria);
        var service = CriarService(repository, hotelId: 10);

        await service.Atualizar(
            5,
            "  Standard  ",
            "  Categoria atualizada  ",
            3,
            2);

        repository.Atualizada.Should().BeSameAs(categoria);
        repository.UltimaCategoriaIdIgnorada.Should().Be(5);
        repository.UltimoNomeConsultado.Should().Be("Standard");
        categoria.Descricao.Should().Be("Categoria atualizada");
        categoria.CapacidadeFisica.Should().Be(3);
        categoria.LimiteComercial.Should().Be(2);
    }

    [Fact]
    public async Task Atualizar_Quando_Outra_Categoria_Tem_O_Mesmo_Nome_Deve_Lancar_ConflictException()
    {
        var categoria = CriarCategoria(5, 10, "Standard");
        var outraCategoria = CriarCategoria(6, 10, "Luxo");
        var repository = new CategoriaQuartoRepositoryFake(
            categoria,
            outraCategoria);
        var service = CriarService(repository, hotelId: 10);

        Func<Task> action = () => service.Atualizar(
            5,
            "Luxo",
            null,
            3,
            2);

        await action.Should().ThrowAsync<ConflictException>();
        repository.Atualizada.Should().BeNull();
    }

    [Fact]
    public async Task Atualizar_Quando_Categoria_Nao_Existe_Deve_Lancar_NotFoundException()
    {
        var repository = new CategoriaQuartoRepositoryFake();
        var service = CriarService(repository, hotelId: 10);

        Func<Task> action = () => service.Atualizar(
            99,
            "Standard",
            null,
            2,
            2);

        await action.Should().ThrowAsync<NotFoundException>();
        repository.ConsultasPorNome.Should().Be(0);
        repository.Atualizada.Should().BeNull();
    }

    [Fact]
    public async Task Atualizar_Sem_Hotel_No_Contexto_Deve_Lancar_ForbiddenException()
    {
        var repository = new CategoriaQuartoRepositoryFake();
        var service = CriarService(repository, hotelId: null);

        Func<Task> action = () => service.Atualizar(
            5,
            "Standard",
            null,
            2,
            2);

        await action.Should().ThrowAsync<ForbiddenException>();
        repository.ConsultasPorId.Should().Be(0);
        repository.Atualizada.Should().BeNull();
    }

    [Fact]
    public async Task Ativar_Deve_Ativar_Categoria_E_Persistir()
    {
        var categoria = CriarCategoria(5, 10, "Standard", ativa: false);
        var repository = new CategoriaQuartoRepositoryFake(categoria);
        var service = CriarService(repository, hotelId: 10);

        await service.Ativar(5);

        categoria.Ativa.Should().BeTrue();
        repository.Atualizada.Should().BeSameAs(categoria);
    }

    [Fact]
    public async Task Desativar_Deve_Desativar_Categoria_E_Persistir()
    {
        var categoria = CriarCategoria(5, 10, "Standard");
        var repository = new CategoriaQuartoRepositoryFake(categoria);
        var service = CriarService(repository, hotelId: 10);

        await service.Desativar(5);

        categoria.Ativa.Should().BeFalse();
        repository.Atualizada.Should().BeSameAs(categoria);
    }

    [Fact]
    public async Task AlterarStatus_Quando_Categoria_Nao_Existe_Deve_Lancar_NotFoundException()
    {
        var repository = new CategoriaQuartoRepositoryFake();
        var service = CriarService(repository, hotelId: 10);

        Func<Task> ativar = () => service.Ativar(99);
        Func<Task> desativar = () => service.Desativar(99);

        await ativar.Should().ThrowAsync<NotFoundException>();
        await desativar.Should().ThrowAsync<NotFoundException>();
        repository.Atualizada.Should().BeNull();
    }

    private static CategoriaQuartoService CriarService(
        CategoriaQuartoRepositoryFake? repository = null,
        int? hotelId = 10)
    {
        return new CategoriaQuartoService(
            repository ?? new CategoriaQuartoRepositoryFake(),
            new HotelContextoFake(hotelId));
    }

    private static CategoriaQuarto CriarCategoria(
        int id,
        int hotelId,
        string nome,
        string? descricao = null,
        int capacidadeFisica = 2,
        int limiteComercial = 2,
        bool ativa = true)
    {
        var categoria = new CategoriaQuarto(
            hotelId,
            nome,
            descricao,
            capacidadeFisica,
            limiteComercial);
        DefinirId(categoria, id);

        if (!ativa)
        {
            categoria.Desativar();
        }

        return categoria;
    }

    private static void DefinirId<T>(T entidade, int id)
    {
        typeof(T)
            .GetProperty("Id")!
            .SetValue(entidade, id);
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

    private sealed class CategoriaQuartoRepositoryFake
        : ICategoriaQuartoRepository
    {
        private readonly List<CategoriaQuarto> _categorias;

        public CategoriaQuartoRepositoryFake(
            params CategoriaQuarto[] categorias)
        {
            _categorias = categorias.ToList();
        }

        public CategoriaQuarto? Adicionada { get; private set; }
        public CategoriaQuarto? Atualizada { get; private set; }
        public int ConsultasPorId { get; private set; }
        public int ConsultasPorNome { get; private set; }
        public int ConsultasTodas { get; private set; }
        public int ConsultasAtivas { get; private set; }
        public string? UltimoNomeConsultado { get; private set; }
        public int? UltimoHotelIdConsultado { get; private set; }
        public int? UltimaCategoriaIdIgnorada { get; private set; }

        public Task AdicionarAsync(CategoriaQuarto categoria)
        {
            var proximoId = _categorias.Count == 0
                ? 1
                : _categorias.Max(item => item.Id) + 1;
            DefinirId(categoria, proximoId);
            _categorias.Add(categoria);
            Adicionada = categoria;
            return Task.CompletedTask;
        }

        public Task<CategoriaQuarto?> ObterPorIdAsync(
            int categoriaId,
            int hotelId)
        {
            ConsultasPorId++;
            UltimoHotelIdConsultado = hotelId;

            return Task.FromResult(
                _categorias.FirstOrDefault(categoria =>
                    categoria.Id == categoriaId &&
                    categoria.HotelId == hotelId));
        }

        public Task<List<CategoriaQuarto>> ObterTodasAsync(int hotelId)
        {
            ConsultasTodas++;
            UltimoHotelIdConsultado = hotelId;

            return Task.FromResult(
                _categorias
                    .Where(categoria => categoria.HotelId == hotelId)
                    .ToList());
        }

        public Task<List<CategoriaQuarto>> ObterAtivasAsync(int hotelId)
        {
            ConsultasAtivas++;
            UltimoHotelIdConsultado = hotelId;

            return Task.FromResult(
                _categorias
                    .Where(categoria =>
                        categoria.HotelId == hotelId &&
                        categoria.Ativa)
                    .ToList());
        }

        public Task<bool> ExisteNomeAsync(
            string nome,
            int hotelId,
            int? categoriaIdIgnorada = null)
        {
            ConsultasPorNome++;
            UltimoNomeConsultado = nome;
            UltimoHotelIdConsultado = hotelId;
            UltimaCategoriaIdIgnorada = categoriaIdIgnorada;

            return Task.FromResult(
                _categorias.Any(categoria =>
                    categoria.HotelId == hotelId &&
                    categoria.Nome == nome &&
                    (!categoriaIdIgnorada.HasValue ||
                     categoria.Id != categoriaIdIgnorada.Value)));
        }

        public Task AtualizarAsync(CategoriaQuarto categoria)
        {
            Atualizada = categoria;
            return Task.CompletedTask;
        }
    }
}
