using System;
using System.Collections.Generic;
using System.Text;
using HotelApp.Domain;
using Microsoft.EntityFrameworkCore;
using HotelApp.Application.Interfaces;

namespace HotelApp.Infrastructure
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly AppDbContext _context;

        public ReservaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reserva>> ObterReservasPorQuartoAsync(int quartoId) {

            return await _context.Reservas
                .Where(r => r.QuartoId == quartoId)
                .ToListAsync();
        }

        public async Task AdicionarReservaAsync(Reserva reserva)
        {
            await _context.Reservas.AddAsync(reserva);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Reserva>> ListarReservasAsync()
        {
            return await _context.Reservas.ToListAsync();
        }

        public async Task DeletarReservaAsync(Reserva reserva)
        {
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

        }

        public async Task<Reserva?> ObterReservaPorIdAsync(int id)
        {
            return await _context.Reservas.FindAsync(id);
        }

        public async Task AtualizarReservaAsync(Reserva reserva)
        {
            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();
        }
    }
}
