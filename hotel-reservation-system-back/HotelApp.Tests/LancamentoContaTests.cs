using FluentAssertions;
using HotelApp.Domain;

public class LancamentoContaTests
{
    [Fact]
    public void Deve_Criar_Credito_Com_Forma_De_Pagamento()
    {
        var lancamento = new LancamentoConta(
            1,
            LancamentoTipo.Credito,
            "Pagamento",
            100,
            FormaPagamento.Pix
        );

        lancamento.ContaReservaId.Should().Be(1);
        lancamento.Tipo.Should().Be(LancamentoTipo.Credito);
        lancamento.Descricao.Should().Be("Pagamento");
        lancamento.Valor.Should().Be(100);
        lancamento.FormaPagamento.Should().Be(FormaPagamento.Pix);
        lancamento.DataLancamento.Should().NotBe(default);
    }

    [Fact]
    public void Deve_Criar_Debito_Sem_Forma_De_Pagamento()
    {
        var lancamento = new LancamentoConta(
            1,
            LancamentoTipo.Debito,
            "Diaria",
            150
        );

        lancamento.Tipo.Should().Be(LancamentoTipo.Debito);
        lancamento.FormaPagamento.Should().BeNull();
    }

    [Fact]
    public void Deve_Lancar_Erro_Quando_Credito_Nao_Tiver_Forma_De_Pagamento()
    {
        Action action = () => new LancamentoConta(
            1,
            LancamentoTipo.Credito,
            "Pagamento",
            100
        );

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Lancar_Erro_Quando_Debito_Tiver_Forma_De_Pagamento()
    {
        Action action = () => new LancamentoConta(
            1,
            LancamentoTipo.Debito,
            "Diaria",
            150,
            FormaPagamento.Pix
        );

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_Lancar_Erro_Quando_Valor_Nao_For_Maior_Que_Zero(decimal valor)
    {
        Action action = () => new LancamentoConta(
            1,
            LancamentoTipo.Debito,
            "Diaria",
            valor
        );

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Lancar_Erro_Quando_Descricao_For_Vazia()
    {
        Action action = () => new LancamentoConta(
            1,
            LancamentoTipo.Debito,
            " ",
            100
        );

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Lancar_Erro_Quando_Conta_For_Invalida()
    {
        Action action = () => new LancamentoConta(
            0,
            LancamentoTipo.Debito,
            "Diaria",
            100
        );

        action.Should().Throw<ArgumentException>();
    }
}
