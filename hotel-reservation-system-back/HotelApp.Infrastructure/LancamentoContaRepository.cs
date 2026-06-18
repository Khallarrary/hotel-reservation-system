using HotelApp.Application.Interfaces;
using HotelApp.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure
{
    public class LancamentoContaRepository : ILancamentoContaRepository
    {
        private readonly AppDbContext _context;

        public LancamentoContaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(LancamentoConta lancamento)
        {
            await _context.LancamentoConta.AddAsync(lancamento);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LancamentoConta>> ListarPorContaReservaIdAsync(int contaReservaId)
        {
            return await _context.LancamentoConta
           
                .Where(x => x.ContaReservaId == contaReservaId)
                .OrderBy(x => x.DataLancamento)
                .ToListAsync();
        }

        public async Task<List<LancamentoConta>> ListarPorReservaIdAsync(int reservaId)
        {
            return await _context.LancamentoConta
                .Join(
                    _context.ContaReserva,
                    lancamento => lancamento.ContaReservaId,
                    conta => conta.Id,
                    (lancamento, conta) => new { lancamento, conta }
                )
                .Where(x => x.conta.ReservaId == reservaId)
                .OrderBy(x => x.lancamento.DataLancamento)
                .Select(x => x.lancamento)
                .ToListAsync();
        }
    }
}
