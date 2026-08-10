using HotelApp.Application.Interfaces;
using HotelApp.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AdicionarAsync(Usuario usuario)
        {
            await _context.Usuario.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Usuario>> ListarUsuariosAsync(int hotelId)
        {
            return await _context.Usuario.Where(u => u.HotelId == hotelId).ToListAsync();
        }

        public async Task<Usuario?> ObterUsuarioPorIdAsync(int usuarioId, int hotelId)
        {
            return await _context.Usuario.FirstOrDefaultAsync(u => u.Id == usuarioId && u.HotelId == hotelId);

        }

        public async Task AtualizarUsuarioAsync(Usuario usuario) 
        {
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
