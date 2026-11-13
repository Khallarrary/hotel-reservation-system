using FluentAssertions;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Services;
using HotelApp.Domain;

namespace HotelApp.Tests;

public class HospedeServiceTests
{
    [Fact]
    public async Task Criar_Deve_Adicionar_Hospede_E_Retornar_Dto_Normalizado()
    {
        var repository = new HospedeRepositoryFake();
        var service = CriarService(repository: repository);

        var resultado = await service.Criar(
            "  Jose da Silva  ",
            "cpf",
            "123.123.123-09",
            "  JOSE@EMAIL.COM  ",
            "(11) 99999-9999");

        resultado.Id.Should().Be(1);
        resultado.Nome.Should().Be("Jose da Silva");
        resultado.TipoDocumento.Should().Be("CPF");
        resultado.Documento.Should().Be("12312312309");
        resultado.Email.Should().Be("jose@email.com");
        resultado.Telefone.Should().Be("11999999999");
        repository.Adicionado.Should().NotBeNull();
        repository.Adicionado!.HotelId.Should().Be(1);
    }

    [Fact]
    public async Task Criar_Sem_Hotel_No_Contexto_Deve_Lancar_ForbiddenException()
    {
        var repository = new HospedeRepositoryFake();
        var hotelRepository = new HotelRepositoryFake(CriarHotel());
        var service = CriarService(
            repository,
            new HotelContextoFake(null),
            hotelRepository);

        Func<Task> action = () => service.Criar(
            "Jose da Silva",
            "CPF",
            "12312312309",
            null,
            null);

        await action.Should().ThrowAsync<ForbiddenException>();
        hotelRepository.ConsultasPorId.Should().Be(0);
        repository.Adicionado.Should().BeNull();
    }

    [Fact]
    public async Task Criar_Com_Tipo_De_Documento_Invalido_Deve_Lancar_ArgumentException()
    {
        var repository = new HospedeRepositoryFake();
        var service = CriarService(repository: repository);

        Func<Task> action = () => service.Criar(
            "Jose da Silva",
            "CNH",
            "123456789",
            null,
            null);

        await action.Should().ThrowAsync<ArgumentException>();
        repository.Adicionado.Should().BeNull();
    }

    [Fact]
    public async Task Criar_Com_Cpf_Malformado_Deve_Lancar_ArgumentException()
    {
        var repository = new HospedeRepositoryFake();
        var service = CriarService(repository: repository);

        Func<Task> action = () => service.Criar(
            "Jose da Silva",
            "CPF",
            "123.456.789-0A",
            null,
            null);

        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*CPF inválido*");
        repository.Adicionado.Should().BeNull();
    }

    [Fact]
    public async Task Criar_Quando_Hotel_Nao_Existe_Deve_Lancar_NotFoundException()
    {
        var repository = new HospedeRepositoryFake();
        var service = CriarService(
            repository,
            hotelRepository: new HotelRepositoryFake(null));

        Func<Task> action = () => service.Criar(
            "Jose da Silva",
            "CPF",
            "12312312309",
            null,
            null);

        await action.Should().ThrowAsync<NotFoundException>();
        repository.Adicionado.Should().BeNull();
    }

    [Fact]
    public async Task Criar_Quando_Hotel_Esta_Inativo_Deve_Lancar_ForbiddenException()
    {
        var repository = new HospedeRepositoryFake();
        var service = CriarService(
            repository,
            hotelRepository: new HotelRepositoryFake(CriarHotel(ativo: false)));

        Func<Task> action = () => service.Criar(
            "Jose da Silva",
            "CPF",
            "12312312309",
            null,
            null);

        await action.Should().ThrowAsync<ForbiddenException>();
        repository.Adicionado.Should().BeNull();
    }

    [Fact]
    public async Task Criar_Quando_Documento_Ja_Existe_Deve_Lancar_ConflictException()
    {
        var existente = CriarHospede(id: 10);
        var repository = new HospedeRepositoryFake(existente);
        var service = CriarService(repository: repository);

        Func<Task> action = () => service.Criar(
            "Outro Hospede",
            "CPF",
            "123.123.123-09",
            null,
            null);

        await action.Should().ThrowAsync<ConflictException>();
        repository.Adicionado.Should().BeNull();
    }

    [Fact]
    public async Task ObterPorDocumento_Deve_Normalizar_Documento_E_Retornar_Dto()
    {
        var existente = CriarHospede(id: 10);
        var repository = new HospedeRepositoryFake(existente);
        var service = CriarService(repository: repository);

        var resultado = await service.ObterPorDocumentoAsync(
            "cpf",
            "123.123.123-09");

        resultado.Id.Should().Be(10);
        resultado.Nome.Should().Be("Jose da Silva");
        resultado.TipoDocumento.Should().Be("CPF");
        resultado.Documento.Should().Be("12312312309");
        repository.UltimoDocumentoConsultado.Should().Be("12312312309");
        repository.UltimoHotelIdConsultado.Should().Be(1);
    }

    [Fact]
    public async Task ObterPorDocumento_Sem_Hotel_No_Contexto_Deve_Lancar_ForbiddenException()
    {
        var repository = new HospedeRepositoryFake(CriarHospede(id: 10));
        var service = CriarService(
            repository,
            new HotelContextoFake(null));

        Func<Task> action = () => service.ObterPorDocumentoAsync(
            "CPF",
            "12312312309");

        await action.Should().ThrowAsync<ForbiddenException>();
        repository.ConsultasPorDocumento.Should().Be(0);
    }

    [Fact]
    public async Task ObterPorDocumento_Com_Tipo_Invalido_Deve_Lancar_ArgumentException()
    {
        var repository = new HospedeRepositoryFake(CriarHospede(id: 10));
        var service = CriarService(repository: repository);

        Func<Task> action = () => service.ObterPorDocumentoAsync(
            "CNH",
            "123456789");

        await action.Should().ThrowAsync<ArgumentException>();
        repository.ConsultasPorDocumento.Should().Be(0);
    }

    [Fact]
    public async Task ObterPorDocumento_Quando_Hotel_Nao_Existe_Deve_Lancar_NotFoundException()
    {
        var repository = new HospedeRepositoryFake(CriarHospede(id: 10));
        var service = CriarService(
            repository,
            hotelRepository: new HotelRepositoryFake(null));

        Func<Task> action = () => service.ObterPorDocumentoAsync(
            "CPF",
            "12312312309");

        await action.Should().ThrowAsync<NotFoundException>();
        repository.ConsultasPorDocumento.Should().Be(0);
    }

    [Fact]
    public async Task ObterPorDocumento_Quando_Hospede_Nao_Existe_Deve_Lancar_NotFoundException()
    {
        var service = CriarService();

        Func<Task> action = () => service.ObterPorDocumentoAsync(
            "CPF",
            "12312312309");

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ObterPorDocumento_Nao_Deve_Retornar_Hospede_De_Outro_Hotel()
    {
        var hospedeDoHotelUm = CriarHospede(id: 10, hotelId: 1);
        var repository = new HospedeRepositoryFake(hospedeDoHotelUm);
        var service = CriarService(
            repository,
            new HotelContextoFake(2),
            new HotelRepositoryFake(CriarHotel(id: 2)));

        Func<Task> action = () => service.ObterPorDocumentoAsync(
            "CPF",
            "12312312309");

        await action.Should().ThrowAsync<NotFoundException>();
        repository.UltimoHotelIdConsultado.Should().Be(2);
    }

    [Fact]
    public async Task ObterPorDocumento_Quando_Hotel_Esta_Inativo_Deve_Lancar_ForbiddenException()
    {
        var repository = new HospedeRepositoryFake(CriarHospede(id: 10));
        var service = CriarService(
            repository,
            hotelRepository: new HotelRepositoryFake(CriarHotel(ativo: false)));

        Func<Task> action = () => service.ObterPorDocumentoAsync(
            "CPF",
            "12312312309");

        await action.Should().ThrowAsync<ForbiddenException>();
        repository.ConsultasPorDocumento.Should().Be(0);
    }

    [Fact]
    public async Task Atualizar_Deve_Alterar_Hospede_E_Persistir_Dados_Normalizados()
    {
        var existente = CriarHospede(id: 10);
        var repository = new HospedeRepositoryFake(existente);
        var service = CriarService(repository: repository);

        await service.Atualizar(
            10,
            "  Maria Souza  ",
            "cpf",
            "987.654.321-00",
            "  MARIA@EMAIL.COM  ",
            "(11) 98888-7777");

        repository.Atualizado.Should().BeSameAs(existente);
        existente.Nome.Should().Be("Maria Souza");
        existente.Documento.Should().Be("98765432100");
        existente.Email.Should().Be("maria@email.com");
        existente.Telefone.Should().Be("11988887777");
    }

    [Fact]
    public async Task Atualizar_Sem_Hotel_No_Contexto_Deve_Lancar_ForbiddenException()
    {
        var repository = new HospedeRepositoryFake(CriarHospede(id: 10));
        var service = CriarService(
            repository,
            new HotelContextoFake(null));

        Func<Task> action = () => service.Atualizar(
            10,
            "Maria Souza",
            "CPF",
            "98765432100",
            null,
            null);

        await action.Should().ThrowAsync<ForbiddenException>();
        repository.ConsultasPorId.Should().Be(0);
    }

    [Fact]
    public async Task Atualizar_Quando_Hotel_Nao_Existe_Deve_Lancar_NotFoundException()
    {
        var repository = new HospedeRepositoryFake(CriarHospede(id: 10));
        var service = CriarService(
            repository,
            hotelRepository: new HotelRepositoryFake(null));

        Func<Task> action = () => service.Atualizar(
            10,
            "Maria Souza",
            "CPF",
            "98765432100",
            null,
            null);

        await action.Should().ThrowAsync<NotFoundException>();
        repository.ConsultasPorId.Should().Be(0);
    }

    [Fact]
    public async Task Atualizar_Quando_Hospede_Nao_Existe_Deve_Lancar_NotFoundException()
    {
        var repository = new HospedeRepositoryFake();
        var service = CriarService(repository: repository);

        Func<Task> action = () => service.Atualizar(
            10,
            "Maria Souza",
            "CPF",
            "98765432100",
            null,
            null);

        await action.Should().ThrowAsync<NotFoundException>();
        repository.Atualizado.Should().BeNull();
    }

    [Fact]
    public async Task Atualizar_Com_Tipo_De_Documento_Invalido_Deve_Lancar_ArgumentException()
    {
        var repository = new HospedeRepositoryFake(CriarHospede(id: 10));
        var service = CriarService(repository: repository);

        Func<Task> action = () => service.Atualizar(
            10,
            "Maria Souza",
            "CNH",
            "987654321",
            null,
            null);

        await action.Should().ThrowAsync<ArgumentException>();
        repository.Atualizado.Should().BeNull();
    }

    [Fact]
    public async Task Atualizar_Nao_Deve_Alterar_Hospede_De_Outro_Hotel()
    {
        var hospedeDoHotelUm = CriarHospede(id: 10, hotelId: 1);
        var repository = new HospedeRepositoryFake(hospedeDoHotelUm);
        var service = CriarService(
            repository,
            new HotelContextoFake(2),
            new HotelRepositoryFake(CriarHotel(id: 2)));

        Func<Task> action = () => service.Atualizar(
            10,
            "Maria Souza",
            "CPF",
            "98765432100",
            null,
            null);

        await action.Should().ThrowAsync<NotFoundException>();
        repository.Atualizado.Should().BeNull();
        hospedeDoHotelUm.Nome.Should().Be("Jose da Silva");
    }

    [Fact]
    public async Task Atualizar_Quando_Documento_Pertence_A_Outro_Hospede_Deve_Lancar_ConflictException()
    {
        var hospede = CriarHospede(id: 10);
        var outroHospede = CriarHospede(
            id: 20,
            documento: "98765432100");
        var repository = new HospedeRepositoryFake(hospede, outroHospede);
        var service = CriarService(repository: repository);

        Func<Task> action = () => service.Atualizar(
            10,
            "Jose da Silva",
            "CPF",
            "987.654.321-00",
            null,
            null);

        await action.Should().ThrowAsync<ConflictException>();
        repository.Atualizado.Should().BeNull();
    }

    [Fact]
    public async Task Atualizar_Quando_Documento_Pertence_Ao_Proprio_Hospede_Deve_Permitir()
    {
        var hospede = CriarHospede(id: 10);
        var repository = new HospedeRepositoryFake(hospede);
        var service = CriarService(repository: repository);

        await service.Atualizar(
            10,
            "Jose Atualizado",
            "CPF",
            "123.123.123-09",
            null,
            null);

        repository.Atualizado.Should().BeSameAs(hospede);
        hospede.Nome.Should().Be("Jose Atualizado");
    }

    [Fact]
    public async Task Atualizar_Quando_Hotel_Esta_Inativo_Deve_Lancar_ForbiddenException()
    {
        var repository = new HospedeRepositoryFake(CriarHospede(id: 10));
        var service = CriarService(
            repository,
            hotelRepository: new HotelRepositoryFake(CriarHotel(ativo: false)));

        Func<Task> action = () => service.Atualizar(
            10,
            "Maria Souza",
            "CPF",
            "98765432100",
            null,
            null);

        await action.Should().ThrowAsync<ForbiddenException>();
        repository.ConsultasPorId.Should().Be(0);
        repository.Atualizado.Should().BeNull();
    }

    private static HospedeService CriarService(
        HospedeRepositoryFake? repository = null,
        IHotelContexto? hotelContexto = null,
        IHotelRepository? hotelRepository = null)
    {
        return new HospedeService(
            repository ?? new HospedeRepositoryFake(),
            hotelContexto ?? new HotelContextoFake(1),
            hotelRepository ?? new HotelRepositoryFake(CriarHotel()));
    }

    private static Hotel CriarHotel(bool ativo = true, int id = 1)
    {
        var hotel = new Hotel(
            "Hotel Teste",
            "12.345.678/0001-90",
            "America/Sao_Paulo");
        DefinirId(hotel, id);

        if (!ativo)
        {
            hotel.Desativar();
        }

        return hotel;
    }

    private static Hospede CriarHospede(
        int id,
        string documento = "12312312309",
        int hotelId = 1)
    {
        var hospede = new Hospede(
            hotelId,
            "Jose da Silva",
            TipoDocumento.CPF,
            documento,
            "jose@email.com",
            "11999999999");
        DefinirId(hospede, id);
        return hospede;
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

    private sealed class HotelRepositoryFake : IHotelRepository
    {
        private readonly Hotel? _hotel;

        public HotelRepositoryFake(Hotel? hotel)
        {
            _hotel = hotel;
        }

        public int ConsultasPorId { get; private set; }

        public Task<Hotel?> ObterPorIdAsync(int id)
        {
            ConsultasPorId++;
            return Task.FromResult(
                _hotel is not null && _hotel.Id == id
                    ? _hotel
                    : null);
        }

        public Task AdicionarAsync(Hotel hotel) => Task.CompletedTask;

        public Task<Hotel?> ObterPorDocumentoAsync(string documento) =>
            Task.FromResult<Hotel?>(null);

        public Task<List<Hotel>> ListarAsync() =>
            Task.FromResult(new List<Hotel>());

        public Task AtualizarAsync(Hotel hotel) => Task.CompletedTask;
    }

    private sealed class HospedeRepositoryFake : IHospedeRepository
    {
        private readonly List<Hospede> _hospedes;

        public HospedeRepositoryFake(params Hospede[] hospedes)
        {
            _hospedes = hospedes.ToList();
        }

        public Hospede? Adicionado { get; private set; }
        public Hospede? Atualizado { get; private set; }
        public int ConsultasPorId { get; private set; }
        public int ConsultasPorDocumento { get; private set; }
        public string? UltimoDocumentoConsultado { get; private set; }
        public int? UltimoHotelIdConsultado { get; private set; }

        public Task AdicionarAsync(Hospede hospede)
        {
            var proximoId = _hospedes.Count == 0
                ? 1
                : _hospedes.Max(item => item.Id) + 1;
            DefinirId(hospede, proximoId);
            _hospedes.Add(hospede);
            Adicionado = hospede;
            return Task.CompletedTask;
        }

        public Task<Hospede?> ObterPorIdAsync(int id, int hotelId)
        {
            ConsultasPorId++;
            return Task.FromResult(
                _hospedes.FirstOrDefault(hospede =>
                    hospede.Id == id &&
                    hospede.HotelId == hotelId));
        }

        public Task<Hospede?> ObterPorDocumentoAsync(
            TipoDocumento tipoDocumento,
            string documentoNormalizado,
            int hotelId)
        {
            ConsultasPorDocumento++;
            UltimoDocumentoConsultado = documentoNormalizado;
            UltimoHotelIdConsultado = hotelId;

            return Task.FromResult(
                _hospedes.FirstOrDefault(hospede =>
                    hospede.TipoDocumento == tipoDocumento &&
                    hospede.Documento == documentoNormalizado &&
                    hospede.HotelId == hotelId));
        }

        public Task AtualizarAsync(Hospede hospede)
        {
            Atualizado = hospede;
            return Task.CompletedTask;
        }

        public Task<List<Hospede>> ObterPorIdsAsync(IReadOnlyCollection<int> hospedeIds, int hotelId)
        {
            var encontrados = _hospedes
                .Where(hospede =>
                    hospede.HotelId == hotelId &&
                    hospedeIds.Contains(hospede.Id))
                .ToList();

            return Task.FromResult(encontrados);
        }
    }
}
