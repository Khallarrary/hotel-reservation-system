using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Services
{
    public class ConsultaSaldoConta : IConsultaSaldoConta
    {
        private readonly IContaReservaRepository _contaReservaRepository;
        private readonly ILancamentoContaRepository _lancamentoContaRepository;


        public ConsultaSaldoConta(IContaReservaRepository contaReservaRepository, ILancamentoContaRepository lancamentoContaRepository)
        {
            _contaReservaRepository = contaReservaRepository;
            _lancamentoContaRepository = lancamentoContaRepository;
        }

        public async Task<decimal> ObterSaldoAsync(int idReserva)
        {
            var conta = await _contaReservaRepository.ObterPorReservaIdAsync(idReserva);

            if (conta == null)
            {
                throw new NotFoundException("Conta nao encontrado");
            }

            var lancamentos = await _lancamentoContaRepository.ListarPorContaReservaIdAsync(conta.Id);

            var totalDebitos = lancamentos
            .Where(l => l.Tipo == LancamentoTipo.Debito)
            .Sum(l => l.Valor);

            var totalCreditos = lancamentos
                .Where(l => l.Tipo == LancamentoTipo.Credito)
                .Sum(l => l.Valor);

            return totalDebitos - totalCreditos;
        }
    }
}
