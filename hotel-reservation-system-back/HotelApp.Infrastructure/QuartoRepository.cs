using HotelApp.Application.Interfaces;
using HotelApp.Domain;
using Microsoft.EntityFrameworkCore;


namespace HotelApp.Infrastructure
{
    public class QuartoRepository : IQuartoRepository
    {
        private readonly AppDbContext _context;

        public QuartoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Quarto?> ObterPorIdAsync(int quartoId, int hotelId)
        {

            return await _context.Quartos
                .FirstOrDefaultAsync(q => q.Id == quartoId && q.HotelId == hotelId);

        }

        public async Task<List<Quarto>> ObterTodosAsync(int hotelId)
        {
            return await _context.Quartos.Where(q => q.HotelId == hotelId).ToListAsync();
        }

        public async Task AdicionarAsync(Quarto quarto)
        {
            await _context.Quartos.AddAsync(quarto);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int quartoId, int hotelId)
        {
            var quarto = await _context.Quartos.FirstOrDefaultAsync(q => q.Id == quartoId && q.HotelId == hotelId);

            if (quarto == null)
            {
                return;
            }

            _context.Quartos.Remove(quarto);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteNumeroAsync(string numero, int hotelId)
        {
            return await _context.Quartos.AnyAsync(q => q.Numero == numero && q.HotelId == hotelId);
        }

        public async Task<Quarto?> ObterPorNumeroAsync(string numero, int hotelId)
        {
            return await _context.Quartos
                .FirstOrDefaultAsync(q => q.Numero == numero && q.HotelId == hotelId);
        }

        public async Task<List<Quarto>> ObterPorIdsAsync(List<int> ids, int hotelId)
        {
            return await _context.Quartos
                .Where(q => ids.Contains(q.Id) && q.HotelId == hotelId)
                .ToListAsync();
        }

    }
}
