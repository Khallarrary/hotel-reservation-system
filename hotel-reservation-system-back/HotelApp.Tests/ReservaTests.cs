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
            1,
            1
        );

        var novaReserva = new Reserva(
            new DateTime(2030, 4, 12),
            new DateTime(2030, 4, 18),
            "Maria",
            1,
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
            1,
            1
        );

        var novaReserva = new Reserva(
            new DateTime(2030, 4, 16),
            new DateTime(2030, 4, 20),
            "Maria",
            1,
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
            1,
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
            1,
            1
        );

        var reserva2 = new Reserva(
            new DateTime(2030, 4, 12),
            new DateTime(2030, 4, 18),
            "Maria",
            2,
            1
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
            1,
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
            1,
            1
        );

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Cancelar_Reserva_Pendente()
    {
        var reserva = new Reserva(
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(2),
            "Joao",
            1,
            1
        );

        reserva.Cancelar();

        reserva.Status.Should().Be(ReservaStatus.Cancelada);
    }

    [Fact]
    public void Nao_Deve_Cancelar_Reserva_Em_CheckIn()
    {
        var reserva = new Reserva(
            DateTime.Today,
            DateTime.Today.AddDays(1),
            "Joao",
            1,
            1
        );
        reserva.RealizarCheckIn();

        Action action = () => reserva.Cancelar();

        action.Should().Throw<ArgumentException>();
        reserva.Status.Should().Be(ReservaStatus.CheckIn);
    }

    [Fact]
    public void Nao_Deve_Cancelar_Reserva_Ja_Cancelada()
    {
        var reserva = new Reserva(
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(2),
            "Joao",
            1,
            1
        );
        reserva.Cancelar();

        Action action = () => reserva.Cancelar();

        action.Should().Throw<ArgumentException>();
        reserva.Status.Should().Be(ReservaStatus.Cancelada);
    }

}
