using FluentAssertions;
using HotelApp.Domain;

public class ContaReservaTests
{
    [Fact]
    public void Deve_Criar_Conta_Aberta_Quando_Reserva_For_Valida()
    {
        var conta = new ContaReserva(1);

        conta.ReservaId.Should().Be(1);
        conta.Status.Should().Be(ContaStatus.Aberta);
        conta.DataAbertura.Should().NotBe(default);
        conta.DataEncerramento.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_Lancar_Erro_Quando_Reserva_For_Invalida(int reservaId)
    {
        Action action = () => new ContaReserva(reservaId);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Encerrar_Conta_Aberta()
    {
        var conta = new ContaReserva(1);

        conta.Encerrar();

        conta.Status.Should().Be(ContaStatus.Encerrada);
        conta.DataEncerramento.Should().NotBeNull();
    }

    [Fact]
    public void Deve_Lancar_Erro_Quando_Conta_Ja_Estiver_Encerrada()
    {
        var conta = new ContaReserva(1);
        conta.Encerrar();

        Action action = () => conta.Encerrar();

        action.Should().Throw<ArgumentException>();
    }
}
