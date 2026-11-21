using HotelApp.Application.Interfaces;
using HotelApp.Domain;
using Microsoft.EntityFrameworkCore;


namespace HotelApp.Infrastructure
{
    public class CategoriaQuartoRepository : ICategoriaQuartoRepository
    {
        private readonly AppDbContext _context;

        public CategoriaQuartoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(CategoriaQuarto categoria)
        {
            await _context.CategoriaQuartos.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task<CategoriaQuarto?> ObterPorIdAsync(int categoriaId, int hotelId)
        {
            return await _context.CategoriaQuartos.FirstOrDefaultAsync(categoria => categoria.Id == categoriaId && categoria.HotelId == hotelId);
        }

        public async Task<List<CategoriaQuarto>> ObterTodasAsync(int hotelId)
        {
            return await _context.CategoriaQuartos.AsNoTracking().Where(categoria => categoria.HotelId == hotelId).ToListAsync();
        }

        public async Task<List<CategoriaQuarto>> ObterAtivasAsync(int hotelId)
        {
            return await _context.CategoriaQuartos.AsNoTracking().Where(categoria => categoria.HotelId == hotelId && categoria.Ativa == true).ToListAsync();
        }

        public async Task<bool> ExisteNomeAsync(string nome, int hotelId, int? categoriaIdIgnorada = null)
        {
            return await _context.CategoriaQuartos.AnyAsync(categoria => categoria.Nome == nome && categoria.HotelId == hotelId &&
            (!categoriaIdIgnorada.HasValue ||
            categoria.Id != categoriaIdIgnorada.Value));
        }

        public async Task AtualizarAsync(CategoriaQuarto categoria)
        {
            _context.CategoriaQuartos.Update(categoria);
            await _context.SaveChangesAsync();
        }
    }
}
