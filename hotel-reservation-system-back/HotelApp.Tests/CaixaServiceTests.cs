using FluentAssertions;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Services;
using HotelApp.Domain;

public class CaixaServiceTests
{
    [Fact]
    public async Task Deve_Lancar_Erro_Quando_Conta_Nao_Existir_Ao_Encerrar()
    {
        var contaRepo = new ContaReservaRepositoryFake(null);
        var lancamentoRepo = new LancamentoContaRepositoryFake();
        var service = new CaixaService(lancamentoRepo, contaRepo);

        Func<Task> action = () => service.EncerrarConta(1);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Deve_Lancar_Erro_Quando_Saldo_For_Diferente_De_Zero_Ao_Encerrar()
    {
        var conta = new ContaReserva(1);
        var contaRepo = new ContaReservaRepositoryFake(conta);
        var lancamentoRepo = new LancamentoContaRepositoryFake(new List<LancamentoConta>
        {
            new LancamentoConta(1, LancamentoTipo.Debito, "Diaria", 200),
            new LancamentoConta(1, LancamentoTipo.Credito, "Pagamento", 100, FormaPagamento.Pix)
        });
        var service = new CaixaService(lancamentoRepo, contaRepo);

        Func<Task> action = () => service.EncerrarConta(1);

        await action.Should().ThrowAsync<ArgumentException>();
        conta.Status.Should().Be(ContaStatus.Aberta);
        contaRepo.Atualizou.Should().BeFalse();
    }

    [Fact]
    public async Task Deve_Encerrar_Conta_Quando_Saldo_For_Zero()
    {
        var conta = new ContaReserva(1);
        var contaRepo = new ContaReservaRepositoryFake(conta);
        var lancamentoRepo = new LancamentoContaRepositoryFake(new List<LancamentoConta>
        {
            new LancamentoConta(1, LancamentoTipo.Debito, "Diaria", 200),
            new LancamentoConta(1, LancamentoTipo.Credito, "Pagamento", 200, FormaPagamento.Pix)
        });
        var service = new CaixaService(lancamentoRepo, contaRepo);

        await service.EncerrarConta(1);

        conta.Status.Should().Be(ContaStatus.Encerrada);
        conta.DataEncerramento.Should().NotBeNull();
        contaRepo.Atualizou.Should().BeTrue();
    }

    private class ContaReservaRepositoryFake : IContaReservaRepository
    {
        private readonly ContaReserva? _conta;

        public bool Atualizou { get; private set; }

        public ContaReservaRepositoryFake(ContaReserva? conta)
        {
            _conta = conta;
        }

        public Task AdicionarAsync(ContaReserva conta)
        {
            return Task.CompletedTask;
        }

        public Task<ContaReserva?> ObterPorReservaIdAsync(int reservaId)
        {
            return Task.FromResult(_conta);
        }

        public Task<ContaReserva?> ObterPorIdAsync(int id)
        {
            return Task.FromResult(_conta);
        }

        public Task AtualizarAsync(ContaReserva conta)
        {
            Atualizou = true;
            return Task.CompletedTask;
        }
    }

    private class LancamentoContaRepositoryFake : ILancamentoContaRepository
    {
        private readonly List<LancamentoConta> _lancamentos;

        public LancamentoContaRepositoryFake()
            : this(new List<LancamentoConta>())
        {
        }

        public LancamentoContaRepositoryFake(List<LancamentoConta> lancamentos)
        {
            _lancamentos = lancamentos;
        }

        public Task AdicionarAsync(LancamentoConta lancamento)
        {
            _lancamentos.Add(lancamento);
            return Task.CompletedTask;
        }

        public Task<List<LancamentoConta>> ListarPorContaReservaIdAsync(int contaReservaId)
        {
            return Task.FromResult(_lancamentos
                .Where(l => l.ContaReservaId == contaReservaId)
                .ToList());
        }

        public Task<List<LancamentoConta>> ListarPorReservaIdAsync(int reservaId)
        {
            return Task.FromResult(_lancamentos);
        }
    }
}
