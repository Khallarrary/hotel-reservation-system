using HotelApp.Application.Interfaces;
using HotelApp.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure
{
    public class ContaReservaRepository : IContaReservaRepository
    {
        private readonly AppDbContext _context;

        public ContaReservaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(ContaReserva conta)
        {
            await _context.ContaReserva.AddAsync(conta);
            await _context.SaveChangesAsync();
        }

        public async Task<ContaReserva?> ObterPorReservaIdAsync(int reservaId)
        {
            return await _context.ContaReserva.
                FirstOrDefaultAsync(c => c.ReservaId == reservaId);
        }

        public async Task<ContaReserva?> ObterPorIdAsync(int id) 
        {
            return await _context.ContaReserva.FindAsync(id);
                
        }

        public async Task AtualizarAsync(ContaReserva conta)
        {
            _context.ContaReserva.Update(conta);
            await _context.SaveChangesAsync();
        }
    }
}
