using System;
using System.Collections.Generic;
using System.Text;
using HotelApp.Application.Interfaces;
using HotelApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Infrastructure
{
    public class HotelRepository : IHotelRepository
    {
        private readonly AppDbContext _context;

        public HotelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Hotel hotel)
        {
            await _context.AddAsync(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task<Hotel?> ObterPorIdAsync(int hotelId)
        {
            return await _context.Hoteis
                .FirstOrDefaultAsync(h => h.Id == hotelId);
        }

        public async Task<Hotel?> ObterPorDocumentoAsync(string documento)
        {
            return await _context.Hoteis.
                FirstOrDefaultAsync(h => h.Documento == documento);
        }

        public async Task<List<Hotel>> ListarAsync()
        {
            return await _context.Hoteis.ToListAsync();
        }

        public async Task AtualizarAsync(Hotel hotel)
        {
            _context.Hoteis.Update(hotel);
            await _context.SaveChangesAsync();
        }

    }
}
