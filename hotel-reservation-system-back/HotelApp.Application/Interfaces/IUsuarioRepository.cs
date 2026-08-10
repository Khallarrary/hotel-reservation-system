using HotelApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task AdicionarAsync(Usuario usuario);
        Task<List<Usuario>> ListarUsuariosAsync(int hotelId);
        Task<Usuario?> ObterUsuarioPorIdAsync(int id, int hotelId);
        Task AtualizarUsuarioAsync(Usuario usuario);
    }
}
