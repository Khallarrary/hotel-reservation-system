using Xunit;
using FluentAssertions;
using HotelApp.Domain;

public class ReservaTests
{
    private static readonly DateOnly DataAtual = new(2030, 4, 1);

    [Fact]
    public void Deve_Conflitar_Quando_Datas_Se_Sobrepoem()
    {
        // Arrange
        var reservaExistente = new Reserva(
            new DateTime(2030, 4, 10),
            new DateTime(2030, 4, 15),
            "João",
            1,
            1,
            DataAtual
        );

        var novaReserva = new Reserva(
            new DateTime(2030, 4, 12),
            new DateTime(2030, 4, 18),
            "Maria",
            1,
            1,
            DataAtual
        );

        // Act
        var resultado = novaReserva.ConflitaCom(reservaExistente);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void Nao_Deve_Conflitar_Quando_Datas_Nao_Se_Sobrepoem()
    {
        // Arrange
        var reservaExistente = new Reserva(
            new DateTime(2030, 4, 10),
            new DateTime(2030, 4, 15),
            "João",
            1,
            1,
            DataAtual
        );

        var novaReserva = new Reserva(
            new DateTime(2030, 4, 16),
            new DateTime(2030, 4, 20),
            "Maria",
            1,
            1,
            DataAtual
        );

        // Act
        var resultado = novaReserva.ConflitaCom(reservaExistente);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void Deve_Lancar_Erro_Quando_CheckOut_Menor_Que_CheckIn()
    {
        Action action = () => new Reserva(
            new DateTime(2030, 4, 15),
            new DateTime(2030, 4, 10),
            "João",
            1,
            1,
            DataAtual
        );

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Nao_Deve_Conflitar_Quando_For_Quartos_Diferentes()
    {
        var reserva1 = new Reserva(
            new DateTime(2030, 4, 10),
            new DateTime(2030, 4, 15),
            "João",
            1,
            1,
            DataAtual
        );

        var reserva2 = new Reserva(
            new DateTime(2030, 4, 12),
            new DateTime(2030, 4, 18),
            "Maria",
            2,
            1,
            DataAtual
        );

        var resultado = reserva2.ConflitaCom(reserva1);

        resultado.Should().BeFalse();
    }

    [Fact]
    public void Deve_Lancar_Erro_Quando_Datas_Invalidas()
    {
        Action action = () => new Reserva(
            new DateTime(2030, 4, 15),
            new DateTime(2030, 4, 10),
            "João",
            1,
            1,
            DataAtual
        );

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Permitir_CheckIn_Na_Data_De_Hoje()
    {
        Action action = () => new Reserva(
            new DateTime(2030, 4, 1),
            new DateTime(2030, 4, 2),
            "João",
            1,
            1,
            DataAtual
        );

        action.Should().NotThrow();
    }

    [Fact]
    public void Deve_Lancar_Erro_Quando_Reserva_Ultrapassar_30_Dias()
    {
        Action action = () => new Reserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 5, 3),
            "João",
            1,
            1,
            DataAtual
        );

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Cancelar_Reserva_Pendente()
    {
        var reserva = new Reserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 3),
            "Joao",
            1,
            1,
            DataAtual
        );

        reserva.Cancelar();

        reserva.Status.Should().Be(ReservaStatus.Cancelada);
    }

    [Fact]
    public void Nao_Deve_Cancelar_Reserva_Em_CheckIn()
    {
        var reserva = new Reserva(
            new DateTime(2030, 4, 1),
            new DateTime(2030, 4, 2),
            "Joao",
            1,
            1,
            DataAtual
        );
        reserva.RealizarCheckIn(DataAtual);

        Action action = () => reserva.Cancelar();

        action.Should().Throw<ArgumentException>();
        reserva.Status.Should().Be(ReservaStatus.CheckIn);
    }

    [Fact]
    public void Nao_Deve_Cancelar_Reserva_Ja_Cancelada()
    {
        var reserva = new Reserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 3),
            "Joao",
            1,
            1,
            DataAtual
        );
        reserva.Cancelar();

        Action action = () => reserva.Cancelar();

        action.Should().Throw<ArgumentException>();
        reserva.Status.Should().Be(ReservaStatus.Cancelada);
    }

}
