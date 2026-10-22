using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
            try
                {
                 await _context.LancamentoConta.AddAsync(lancamento);
                 await _context.SaveChangesAsync();
                }
                catch(DbUpdateException ex) when (ex.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation && postgresException.ConstraintName == "IX_LancamentoConta_ContaReservaId_ChaveIdempotencia")
                {
                throw new ChaveIdempotenciaDuplicadaException("A tentativa de lançamento ja foi processada.", ex);
                }
       
         }

        public async Task<List<LancamentoConta>> ListarPorContaReservaIdAsync(int contaReservaId)
        {
            return await _context.LancamentoConta
           
                .Where(x => x.ContaReservaId == contaReservaId)
                .OrderBy(x => x.DataLancamento)
                .ToListAsync();
        }

        public async Task<LancamentoConta?> ObterPorChaveIdempotenciaAsync(Guid chaveIdempotencia, int contaReservaId)
        {
            return await _context.LancamentoConta.AsNoTracking().FirstOrDefaultAsync(l => l.ChaveIdempotencia == chaveIdempotencia && l.ContaReservaId == contaReservaId);
        }


    }
}
