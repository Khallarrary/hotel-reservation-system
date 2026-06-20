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


        public CaixaService(ILancamentoContaRepository repo, IContaReservaRepository contaRepo)
        {
            _lancamentoRepo = repo;
            _contaRepo = contaRepo;
        }

        public async Task LancarCredito(int reservaId, decimal valor, FormaPagamento formaPagamento, string descricao)
        {
            var conta = await _contaRepo.ObterPorReservaIdAsync(reservaId);

            if (conta == null)
            {
                throw new ArgumentException("Conta nao encontrada");
            }

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

        public async Task<List<LancamentoContaDto>> ListarLancamentosPorReserva(int reservaId)
        {
            var conta = await _contaRepo.ObterPorReservaIdAsync(reservaId);

            if(conta == null)
            {
                throw new NotFoundException("Conta nao encontrada.");
            }

            var lancamentos = await _lancamentoRepo.ListarPorReservaIdAsync(reservaId);

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
    }
}
