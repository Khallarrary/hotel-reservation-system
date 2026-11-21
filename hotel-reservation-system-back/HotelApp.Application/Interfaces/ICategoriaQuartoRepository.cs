using HotelApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Interfaces
{
    public interface ICategoriaQuartoRepository
    {
        Task AdicionarAsync(CategoriaQuarto categoria);

        Task<CategoriaQuarto?> ObterPorIdAsync(
            int categoriaId,
            int hotelId);

        Task<List<CategoriaQuarto>> ObterTodasAsync(int hotelId);

        Task<List<CategoriaQuarto>> ObterAtivasAsync(int hotelId);

        Task<bool> ExisteNomeAsync(
            string nome,
            int hotelId,
            int? categoriaIdIgnorada = null);

        Task AtualizarAsync(CategoriaQuarto categoria);
    }
}
