using System.Reflection;
using HotelApp.Api.Controllers;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Services;
using HotelApp.Domain;
using Microsoft.AspNetCore.Authorization;

public class UsuarioServiceTests
{
    [Fact]
    public async Task Deve_Vincular_Usuario_Ao_Hotel_Do_Contexto_Autenticado()
    {
        var usuarioRepo = new UsuarioRepositoryFake();
        var hotel = CriarHotel(id: 7);
        var service = CriarService(usuarioRepo, new HotelRepositoryFake(hotel), hotelId: 7);

        await service.CriarUsuario(
            "Operador Teste",
            "operador@teste.com",
            "senha-segura",
            "Operador");

        Assert.NotNull(usuarioRepo.UsuarioAdicionado);
        Assert.Equal(7, usuarioRepo.UsuarioAdicionado.HotelId);
        Assert.Equal(PerfilUsuario.Operador, usuarioRepo.UsuarioAdicionado.Perfil);
    }

    [Fact]
    public async Task Deve_Bloquear_Criacao_Quando_Token_Nao_Possuir_Hotel()
    {
        var usuarioRepo = new UsuarioRepositoryFake();
        var service = CriarService(usuarioRepo, new HotelRepositoryFake(null), hotelId: null);

        var action = () => service.CriarUsuario(
            "Operador Teste",
            "operador@teste.com",
            "senha-segura",
            "Operador");

        await Assert.ThrowsAsync<ForbiddenException>(action);
        Assert.Null(usuarioRepo.UsuarioAdicionado);
    }

    [Fact]
    public async Task Deve_Bloquear_Criacao_Quando_Hotel_Do_Token_Nao_Existir()
    {
        var usuarioRepo = new UsuarioRepositoryFake();
        var service = CriarService(usuarioRepo, new HotelRepositoryFake(null), hotelId: 99);

        var action = () => service.CriarUsuario(
            "Operador Teste",
            "operador@teste.com",
            "senha-segura",
            "Operador");

        await Assert.ThrowsAsync<NotFoundException>(action);
        Assert.Null(usuarioRepo.UsuarioAdicionado);
    }

    [Fact]
    public async Task Deve_Bloquear_Criacao_Quando_Hotel_Estiver_Inativo()
    {
        var usuarioRepo = new UsuarioRepositoryFake();
        var hotel = CriarHotel(id: 3);
        hotel.Desativar();
        var service = CriarService(usuarioRepo, new HotelRepositoryFake(hotel), hotelId: 3);

        var action = () => service.CriarUsuario(
            "Operador Teste",
            "operador@teste.com",
            "senha-segura",
            "Operador");

        await Assert.ThrowsAsync<ForbiddenException>(action);
        Assert.Null(usuarioRepo.UsuarioAdicionado);
    }

    [Fact]
    public async Task Deve_Bloquear_Criacao_De_Perfil_Master_Neste_Fluxo()
    {
        var usuarioRepo = new UsuarioRepositoryFake();
        var service = CriarService(
            usuarioRepo,
            new HotelRepositoryFake(CriarHotel(id: 1)),
            hotelId: 1);

        var action = () => service.CriarUsuario(
            "Master Teste",
            "master@teste.com",
            "senha-segura",
            "Master");

        await Assert.ThrowsAsync<ArgumentException>(action);
        Assert.Null(usuarioRepo.UsuarioAdicionado);
    }

    [Fact]
    public void Endpoint_De_Criacao_Deve_Ser_Exclusivo_De_Gestor()
    {
        var metodo = typeof(UsuarioController).GetMethod(
            nameof(UsuarioController.CriarUsuarioAsync));

        var authorize = metodo?.GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(authorize);
        Assert.Equal("Gestor", authorize.Roles);
    }

    private static UsuarioService CriarService(
        IUsuarioRepository usuarioRepo,
        IHotelRepository hotelRepo,
        int? hotelId)
    {
        return new UsuarioService(
            usuarioRepo,
            new SenhaHasherFake(),
            new TokenServiceFake(),
            hotelRepo,
            new HotelContextoFake(hotelId));
    }

    private static Hotel CriarHotel(int id)
    {
        var hotel = new Hotel(
            "Hotel Teste",
            $"12.345.678/0001-{id:D2}",
            "America/Sao_Paulo");

        typeof(Hotel)
            .GetProperty(nameof(Hotel.Id))!
            .SetValue(hotel, id);

        return hotel;
    }

    private class UsuarioRepositoryFake : IUsuarioRepository
    {
        public Usuario? UsuarioAdicionado { get; private set; }

        public Task<Usuario?> ObterPorEmailAsync(string email)
        {
            return Task.FromResult<Usuario?>(null);
        }

        public Task AdicionarAsync(Usuario usuario)
        {
            UsuarioAdicionado = usuario;
            return Task.CompletedTask;
        }
    }

    private class HotelRepositoryFake : IHotelRepository
    {
        private readonly Hotel? _hotel;

        public HotelRepositoryFake(Hotel? hotel)
        {
            _hotel = hotel;
        }

        public Task<Hotel?> ObterPorIdAsync(int id)
        {
            var resultado = _hotel?.Id == id ? _hotel : null;
            return Task.FromResult(resultado);
        }

        public Task AdicionarAsync(Hotel hotel) => Task.CompletedTask;

        public Task<Hotel?> ObterPorDocumentoAsync(string documento)
        {
            return Task.FromResult<Hotel?>(null);
        }

        public Task<List<Hotel>> ListarAsync()
        {
            return Task.FromResult(new List<Hotel>());
        }

        public Task AtualizarAsync(Hotel hotel) => Task.CompletedTask;
    }

    private class HotelContextoFake : IHotelContexto
    {
        private readonly int? _hotelId;

        public HotelContextoFake(int? hotelId)
        {
            _hotelId = hotelId;
        }

        public int? ObterHotelId() => _hotelId;
    }

    private class SenhaHasherFake : ISenhaHasher
    {
        public string GerarSenhaHash(string senha) => $"hash:{senha}";

        public bool Verificar(string senhaHash, string senha)
        {
            return senhaHash == $"hash:{senha}";
        }
    }

    private class TokenServiceFake : ITokenService
    {
        public string GerarToken(Usuario usuario) => "token-teste";
    }
}
