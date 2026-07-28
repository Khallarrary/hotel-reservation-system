using HotelApp.Application.DTOs;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Services
{
    public class CaixaService
    {
        private readonly ILancamentoContaRepository _lancamentoRepo;
        private readonly IContaReservaRepository _contaRepo;
        private readonly IHotelContexto _hotelContexto;
        private readonly IReservaRepository _reservaRepo;


        public CaixaService(ILancamentoContaRepository repo, IContaReservaRepository contaRepo, IHotelContexto hotelContexto, IReservaRepository reservaRepo    )
        {
            _lancamentoRepo = repo;
            _contaRepo = contaRepo;
            _hotelContexto = hotelContexto;
            _reservaRepo = reservaRepo;
        }

        private async Task<ContaReserva> ObterContaSegura(int reservaId)
        {
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
                throw new ForbiddenException("Hotel não encontrado");

            var reserva = await _reservaRepo.ObterReservaPorIdAsync(reservaId, hotelId.Value);

            if (reserva == null)
                throw new NotFoundException("Reserva não encontrada");

            var conta = await _contaRepo.ObterPorReservaIdAsync(reservaId);

            if (conta == null)
                throw new NotFoundException("Conta não encontrada");

            return conta;

        }

        public async Task LancarCredito(int reservaId, decimal valor, FormaPagamento formaPagamento, string descricao)
        {
            var conta = await ObterContaSegura(reservaId);

            if (conta.Status == ContaStatus.Encerrada)
            {
                throw new ArgumentException("Não é possível lançar crédito em uma conta encerrada.");
            }

            var lancamento = new LancamentoConta(
            conta.Id,
            LancamentoTipo.Credito,
            descricao,
            valor,
            formaPagamento
            );

            await _lancamentoRepo.AdicionarAsync(lancamento);
        }
        public async Task LancarDebito(int reservaId, decimal valor, string descricao)
        {
            var conta = await ObterContaSegura(reservaId);
                       
            if (conta.Status == ContaStatus.Encerrada)
            {
                throw new ArgumentException("Não é possível lançar débitos em uma conta encerrada.");
            }

            var lancamento = new LancamentoConta(
            conta.Id,
            LancamentoTipo.Debito,
            descricao,
            valor
            );

            await _lancamentoRepo.AdicionarAsync(lancamento);
        }

        public async Task<List<LancamentoContaDto>> ListarLancamentosPorReserva(int reservaId)
        {
            var conta = await ObterContaSegura(reservaId);

            var lancamentos = await _lancamentoRepo.ListarPorContaReservaIdAsync(conta.Id);

            return lancamentos.Select(l => new LancamentoContaDto
            {
                Id = l.Id,
                Tipo = l.Tipo.ToString(),
                Descricao = l.Descricao,
                Valor = l.Valor,
                FormaPagamento = l.FormaPagamento?.ToString(),
                DataLancamento = l.DataLancamento
            }).ToList();
        }

        public async Task<CaixaResumoDto> ResumoCaixa(int reservaId) 
        {
            var conta = await ObterContaSegura(reservaId);

            var lancamentos = await ListarLancamentosPorReserva(reservaId);

            var totalDebitos = lancamentos
                .Where(l => l.Tipo == "Debito")
                .Sum(l => l.Valor);

            var totalCreditos = lancamentos
                .Where(l => l.Tipo == "Credito")
                .Sum(l => l.Valor);

            return new CaixaResumoDto
            {
                ReservaId = reservaId,
                ContaReservaId = conta.Id,
                StatusConta = conta.Status.ToString(),
                TotalDebitos = totalDebitos,
                TotalCreditos = totalCreditos,
                Saldo = totalDebitos - totalCreditos,
                Lancamentos = lancamentos
            };
        }

        public async Task EncerrarConta(int reservaId)
        {
            var conta = await ObterContaSegura(reservaId);

            var resumo = await ResumoCaixa(reservaId);

            var saldo = resumo.Saldo;

            if(saldo != 0)
            {
                throw new ArgumentException("Conta com saldo diferente de zero não pode ser encerrada");
            }

            conta.Encerrar();

            await _contaRepo.AtualizarAsync(conta);
        }
    }
}
