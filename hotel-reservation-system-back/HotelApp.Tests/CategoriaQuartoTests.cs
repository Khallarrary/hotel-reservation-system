using FluentAssertions;
using HotelApp.Domain;

namespace HotelApp.Tests;

public class CategoriaQuartoTests
{
    [Fact]
    public void Deve_Criar_Categoria_Com_Dados_Validos()
    {
        var categoria = new CategoriaQuarto(
            1,
            "  Suite Master  ",
            "  Suite com vista para o mar  ",
            4,
            3);

        categoria.HotelId.Should().Be(1);
        categoria.Nome.Should().Be("Suite Master");
        categoria.Descricao.Should().Be("Suite com vista para o mar");
        categoria.CapacidadeFisica.Should().Be(4);
        categoria.LimiteComercial.Should().Be(3);
        categoria.Ativa.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Deve_Converter_Descricao_Vazia_Em_Null(string? descricao)
    {
        var categoria = new CategoriaQuarto(
            1,
            "Standard",
            descricao,
            2,
            2);

        categoria.Descricao.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Nao_Deve_Criar_Categoria_Com_Hotel_Invalido(int hotelId)
    {
        Action action = () => new CategoriaQuarto(
            hotelId,
            "Standard",
            null,
            2,
            2);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Hotel inválido*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Nao_Deve_Criar_Categoria_Com_Nome_Vazio(string? nome)
    {
        Action action = () => new CategoriaQuarto(
            1,
            nome!,
            null,
            2,
            2);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Nome da categoria*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Nao_Deve_Criar_Categoria_Com_Capacidade_Fisica_Invalida(
        int capacidadeFisica)
    {
        Action action = () => new CategoriaQuarto(
            1,
            "Standard",
            null,
            capacidadeFisica,
            1);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Capacidade*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Nao_Deve_Criar_Categoria_Com_Limite_Comercial_Invalido(
        int limiteComercial)
    {
        Action action = () => new CategoriaQuarto(
            1,
            "Standard",
            null,
            2,
            limiteComercial);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Limite comercial*");
    }

    [Fact]
    public void Nao_Deve_Criar_Quando_Limite_Superar_Capacidade_Fisica()
    {
        Action action = () => new CategoriaQuarto(
            1,
            "Standard",
            null,
            2,
            3);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*não pode superar*");
    }

    [Fact]
    public void Deve_Permitir_Limite_Igual_A_Capacidade_Fisica()
    {
        Action action = () => new CategoriaQuarto(
            1,
            "Standard",
            null,
            2,
            2);

        action.Should().NotThrow();
    }

    [Fact]
    public void Deve_Atualizar_Dados_Da_Categoria()
    {
        var categoria = new CategoriaQuarto(
            1,
            "Standard",
            null,
            2,
            2);

        categoria.AtualizarDados(
            "  Luxo  ",
            "  Quarto com varanda  ",
            4,
            3);

        categoria.HotelId.Should().Be(1);
        categoria.Nome.Should().Be("Luxo");
        categoria.Descricao.Should().Be("Quarto com varanda");
        categoria.CapacidadeFisica.Should().Be(4);
        categoria.LimiteComercial.Should().Be(3);
        categoria.Ativa.Should().BeTrue();
    }

    [Fact]
    public void Nao_Deve_Alterar_Dados_Quando_Atualizacao_For_Invalida()
    {
        var categoria = new CategoriaQuarto(
            1,
            "Standard",
            "Quarto standard",
            2,
            2);

        Action action = () => categoria.AtualizarDados(
            "Luxo",
            "Quarto de luxo",
            2,
            3);

        action.Should().Throw<ArgumentException>();
        categoria.HotelId.Should().Be(1);
        categoria.Nome.Should().Be("Standard");
        categoria.Descricao.Should().Be("Quarto standard");
        categoria.CapacidadeFisica.Should().Be(2);
        categoria.LimiteComercial.Should().Be(2);
        categoria.Ativa.Should().BeTrue();
    }

    [Fact]
    public void Deve_Desativar_E_Ativar_Categoria()
    {
        var categoria = new CategoriaQuarto(
            1,
            "Standard",
            null,
            2,
            2);

        categoria.Desativar();

        categoria.Ativa.Should().BeFalse();

        categoria.Ativar();

        categoria.Ativa.Should().BeTrue();
    }
}
