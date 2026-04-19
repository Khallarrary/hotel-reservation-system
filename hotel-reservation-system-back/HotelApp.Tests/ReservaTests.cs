using Xunit;
using FluentAssertions;
using HotelApp.Domain;

public class ReservaTests
{
    [Fact]
    public void Deve_Conflitar_Quando_Datas_Se_Sobrepoem()
    {
        // Arrange
        var reservaExistente = new Reserva(
            new DateTime(2030, 4, 10),
            new DateTime(2030, 4, 15),
            "João",
            1
        );

        var novaReserva = new Reserva(
            new DateTime(2030, 4, 12),
            new DateTime(2030, 4, 18),
            "Maria",
            1
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
            1
        );

        var novaReserva = new Reserva(
            new DateTime(2030, 4, 16),
            new DateTime(2030, 4, 20),
            "Maria",
            1
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
            1
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
            1
        );

        var reserva2 = new Reserva(
            new DateTime(2030, 4, 12),
            new DateTime(2030, 4, 18),
            "Maria",
            2
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
            1
        );

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Permitir_CheckIn_Na_Data_De_Hoje()
    {
        Action action = () => new Reserva(
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(1),
            "João",
            1
        );

        action.Should().NotThrow();
    }

    [Fact]
    public void Deve_Lancar_Erro_Quando_Reserva_Ultrapassar_30_Dias()
    {
        Action action = () => new Reserva(
            DateTime.UtcNow.Date.AddDays(1),
            DateTime.UtcNow.Date.AddDays(32),
            "João",
            1
        );

        action.Should().Throw<ArgumentException>();
    }

}