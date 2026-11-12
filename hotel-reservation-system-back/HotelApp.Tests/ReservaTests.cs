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
    public void Nao_Deve_Conflitar_Quando_Reserva_Existente_Estiver_Cancelada()
    {
        var reservaExistente = new Reserva(
            new DateTime(2030, 4, 10),
            new DateTime(2030, 4, 15),
            "Joao",
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

        reservaExistente.Cancelar();

        var resultado = novaReserva.ConflitaCom(reservaExistente);

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

    [Fact]
    public void Deve_Realizar_CheckOut_Na_Data_Prevista_Sem_Confirmacao()
    {
        var reserva = new Reserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 3),
            "Joao",
            1,
            1,
            DataAtual
        );

        reserva.RealizarCheckIn(new DateOnly(2030, 4, 2));

        reserva.RealizarCheckOut(new DateOnly(2030, 4, 3), false);

        reserva.Status.Should().Be(ReservaStatus.CheckOut);


    }

    [Fact]
    public void Deve_Bloquear_CheckOut_Antecipado_Sem_Confirmacao()
    {
        var reserva = new Reserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 4),
            "Joao",
            1,
            1,
            DataAtual
        );

        reserva.RealizarCheckIn(new DateOnly(2030, 4, 2));

        Action action = () => reserva.RealizarCheckOut(new DateOnly(2030, 4, 3), false);

        action.Should().Throw<ArgumentException>();
        reserva.Status.Should().Be(ReservaStatus.CheckIn);


    }

    [Fact]
    public void Deve_Realizar_CheckOut_Antecipado_Quando_Confirmado()
    {
        var reserva = new Reserva(
            new DateTime(2030, 4, 2),
            new DateTime(2030, 4, 4),
            "Joao",
            1,
            1,
            DataAtual
        );

        reserva.RealizarCheckIn(new DateOnly(2030, 4, 2));

        reserva.RealizarCheckOut(new DateOnly(2030, 4, 3), true);

        reserva.Status.Should().Be(ReservaStatus.CheckOut);


    }

    private static Reserva CriarReserva(int hotelId = 1)
    {
        return new Reserva(
            new DateTime(2030, 4, 10),
            new DateTime(2030, 4, 12),
            "Titular legado",
            1,
            hotelId,
            DataAtual);
    }

    private static Hospede CriarHospede(
        int id,
        int hotelId = 1)
    {
        var hospede = new Hospede(
            hotelId,
            $"Hospede {id}",
            TipoDocumento.CPF,
            id.ToString("D11"),
            null,
            null);

        typeof(Hospede)
            .GetProperty(nameof(Hospede.Id))!
            .SetValue(hospede, id);

        return hospede;
    }


    [Fact]
    public void Deve_Adicionar_Primeiro_Hospede_Como_Titular()
    {
        var reserva = CriarReserva();
        var hospede = CriarHospede(10);

        reserva.AdicionarHospede(hospede, true);

        reserva.Hospedes.Should().ContainSingle();

        var vinculo = reserva.Hospedes.Single();

        vinculo.HospedeId.Should().Be(10);
        vinculo.HotelId.Should().Be(1);
        vinculo.Titular.Should().BeTrue();
    }

    [Fact]
    public void Nao_Deve_Adicionar_Primeiro_Hospede_Como_Acompanhante()
    {
        var reserva = CriarReserva();
        var hospede = CriarHospede(10);

        Action action = () =>
            reserva.AdicionarHospede(hospede, false);

        action.Should().Throw<ArgumentException>();
        reserva.Hospedes.Should().BeEmpty();
    }

    [Fact]
    public void Deve_Adicionar_Acompanhante_Depois_Do_Titular()
    {
        var reserva = CriarReserva();
        var titular = CriarHospede(10);
        var acompanhante = CriarHospede(20);

        reserva.AdicionarHospede(titular, true);
        reserva.AdicionarHospede(acompanhante, false);

        reserva.Hospedes.Should().HaveCount(2);
        reserva.Hospedes.Count(item => item.Titular)
            .Should().Be(1);

        reserva.Hospedes.Should().Contain(item =>
            item.HospedeId == 20 &&
            !item.Titular);
    }

    [Fact]
    public void Nao_Deve_Adicionar_Hospede_Nulo()
    {
        var reserva = CriarReserva();

        Action action = () =>
            reserva.AdicionarHospede(null!, true);

        action.Should().Throw<ArgumentException>();
        reserva.Hospedes.Should().BeEmpty();
    }

    [Fact]
    public void Nao_Deve_Adicionar_Hospede_Duplicado()
    {
        var reserva = CriarReserva();
        var hospede = CriarHospede(10);
        reserva.AdicionarHospede(hospede, true);

        Action action = () =>
            reserva.AdicionarHospede(hospede, false);

        action.Should().Throw<ArgumentException>();
        reserva.Hospedes.Should().ContainSingle();
        reserva.Hospedes.Single().Titular.Should().BeTrue();
    }

    [Fact]
    public void Nao_Deve_Adicionar_Hospede_De_Outro_Hotel()
    {
        var reserva = CriarReserva(hotelId: 1);
        var hospede = CriarHospede(10, hotelId: 2);

        Action action = () =>
            reserva.AdicionarHospede(hospede, true);

        action.Should().Throw<ArgumentException>();
        reserva.Hospedes.Should().BeEmpty();
    }

    [Fact]
    public void Nao_Deve_Adicionar_Segundo_Titular()
    {
        var reserva = CriarReserva();
        var titular = CriarHospede(10);
        var segundoTitular = CriarHospede(20);
        reserva.AdicionarHospede(titular, true);

        Action action = () =>
            reserva.AdicionarHospede(segundoTitular, true);

        action.Should().Throw<ArgumentException>();
        reserva.Hospedes.Should().ContainSingle();
        reserva.Hospedes.Single().HospedeId.Should().Be(10);
    }

    [Fact]
    public void PossuiHospede_Deve_Retornar_True_Para_Participante()
    {
        var reserva = CriarReserva();
        reserva.AdicionarHospede(CriarHospede(10), true);

        var resultado = reserva.PossuiHospede(10);

        resultado.Should().BeTrue();
    }

    [Fact]
    public void PossuiHospede_Deve_Retornar_False_Para_Nao_Participante()
    {
        var reserva = CriarReserva();
        reserva.AdicionarHospede(CriarHospede(10), true);

        var resultado = reserva.PossuiHospede(20);

        resultado.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PossuiHospede_Deve_Rejeitar_Id_Invalido(int hospedeId)
    {
        var reserva = CriarReserva();

        Action action = () => reserva.PossuiHospede(hospedeId);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Trocar_Hospede_Titular()
    {
        var reserva = CriarReserva();
        reserva.AdicionarHospede(CriarHospede(10), true);
        reserva.AdicionarHospede(CriarHospede(20), false);

        reserva.DefinirTitular(20);

        reserva.Hospedes.Count(item => item.Titular)
            .Should().Be(1);
        reserva.Hospedes.Single(item => item.Titular)
            .HospedeId.Should().Be(20);
        reserva.Hospedes.Single(item => item.HospedeId == 10)
            .Titular.Should().BeFalse();
    }

    [Fact]
    public void Definir_Titular_Atual_Novamente_Nao_Deve_Alterar_Colecao()
    {
        var reserva = CriarReserva();
        reserva.AdicionarHospede(CriarHospede(10), true);
        reserva.AdicionarHospede(CriarHospede(20), false);

        reserva.DefinirTitular(10);

        reserva.Hospedes.Should().HaveCount(2);
        reserva.Hospedes.Count(item => item.Titular)
            .Should().Be(1);
        reserva.Hospedes.Single(item => item.Titular)
            .HospedeId.Should().Be(10);
    }

    [Fact]
    public void Nao_Deve_Definir_Titular_Que_Nao_Pertence_A_Reserva()
    {
        var reserva = CriarReserva();
        reserva.AdicionarHospede(CriarHospede(10), true);

        Action action = () => reserva.DefinirTitular(20);

        action.Should().Throw<ArgumentException>();
        reserva.Hospedes.Single(item => item.Titular)
            .HospedeId.Should().Be(10);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DefinirTitular_Deve_Rejeitar_Id_Invalido(int hospedeId)
    {
        var reserva = CriarReserva();
        reserva.AdicionarHospede(CriarHospede(10), true);

        Action action = () => reserva.DefinirTitular(hospedeId);

        action.Should().Throw<ArgumentException>();
        reserva.Hospedes.Single().Titular.Should().BeTrue();
    }

    [Fact]
    public void Deve_Remover_Acompanhante()
    {
        var reserva = CriarReserva();
        reserva.AdicionarHospede(CriarHospede(10), true);
        reserva.AdicionarHospede(CriarHospede(20), false);

        reserva.RemoverHospede(20);

        reserva.Hospedes.Should().ContainSingle();
        reserva.PossuiHospede(20).Should().BeFalse();
        reserva.Hospedes.Single().HospedeId.Should().Be(10);
        reserva.Hospedes.Single().Titular.Should().BeTrue();
    }

    [Fact]
    public void Nao_Deve_Remover_Hospede_Titular()
    {
        var reserva = CriarReserva();
        reserva.AdicionarHospede(CriarHospede(10), true);
        reserva.AdicionarHospede(CriarHospede(20), false);

        Action action = () => reserva.RemoverHospede(10);

        action.Should().Throw<ArgumentException>();
        reserva.Hospedes.Should().HaveCount(2);
        reserva.Hospedes.Single(item => item.Titular)
            .HospedeId.Should().Be(10);
    }

    [Fact]
    public void Nao_Deve_Remover_Hospede_Que_Nao_Pertence_A_Reserva()
    {
        var reserva = CriarReserva();
        reserva.AdicionarHospede(CriarHospede(10), true);

        Action action = () => reserva.RemoverHospede(20);

        action.Should().Throw<ArgumentException>();
        reserva.Hospedes.Should().ContainSingle();
        reserva.Hospedes.Single().HospedeId.Should().Be(10);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RemoverHospede_Deve_Rejeitar_Id_Invalido(int hospedeId)
    {
        var reserva = CriarReserva();
        reserva.AdicionarHospede(CriarHospede(10), true);

        Action action = () => reserva.RemoverHospede(hospedeId);

        action.Should().Throw<ArgumentException>();
        reserva.Hospedes.Should().ContainSingle();
    }


}
