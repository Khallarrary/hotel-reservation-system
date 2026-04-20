using HotelApp.Application.Interfaces;
using HotelApp.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure
{
    public class QuartoRepository : IQuartoRepository
    {
        private readonly AppDbContext _context;

        public QuartoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Quarto?> ObterPorIdAsync(int quartoId)
        {

            return await _context.Quartos
                .FirstOrDefaultAsync(q => q.Id == quartoId);

        }

        public async Task<List<Quarto>> ObterTodosAsync()
        {
            return await _context.Quartos.ToListAsync();
        }

        public async Task AdicionarAsync(Quarto quarto)
        {
            await _context.Quartos.AddAsync(quarto);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int quartoId)
        {
            var quarto = await _context.Quartos.FirstOrDefaultAsync(q => q.Id == quartoId);

            if (quarto == null) {
                return;
            }

            _context.Quartos.Remove(quarto);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteNumeroAsync(string numero)
        {
            return await _context.Quartos.AnyAsync(q => q.Numero == numero);
        }

        public async Task<Quarto?> ObterPorNumeroAsync(string numero)
        {
            return await _context.Quartos
                .FirstOrDefaultAsync(q => q.Numero == numero);
        }

   
    }
}
