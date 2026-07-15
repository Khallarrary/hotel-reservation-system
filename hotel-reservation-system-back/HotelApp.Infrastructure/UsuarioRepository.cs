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

        public UsuarioRepository (AppDbContext context)
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
    }
}
