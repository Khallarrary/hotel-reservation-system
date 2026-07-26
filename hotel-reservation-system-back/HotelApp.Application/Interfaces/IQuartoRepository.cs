using System;
using System.Collections.Generic;
using System.Text;
using HotelApp.Domain;

namespace HotelApp.Application.Interfaces
{
    public interface IQuartoRepository
    {
        Task <Quarto?> ObterPorIdAsync(int quartoId, int hotelId);
        Task<List<Quarto>> ObterTodosAsync(int hotelId);
        Task RemoverAsync(int quartoId, int hotelId);
        Task AdicionarAsync(Quarto quarto);
        Task<bool> ExisteNumeroAsync(string numero, int hotelId);
        Task<Quarto?> ObterPorNumeroAsync(string numero, int hotelId);
        Task<List<Quarto>> ObterPorIdsAsync(List<int> ids, int hotelId);


    }
}
