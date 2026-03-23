using System;
using System.Collections.Generic;
using System.Text;
using HotelApp.Domain;
using HotelApp.Application;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Infrastructure
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly AppDbContext _context;

        public ReservaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reserva>> ObterPorQuartoAsync(int quartoId) {

            return await _context.Reservas
                .Where(r => r.QuartoId == quartoId)
                .ToListAsync();
        }

        public async Task AdicionarAsync(Reserva reserva)
        {
            await _context.Reservas.AddAsync(reserva);
            await _context.SaveChangesAsync();
        }
    }
}
