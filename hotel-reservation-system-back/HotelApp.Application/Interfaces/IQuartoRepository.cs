using System;
using System.Collections.Generic;
using System.Text;
using HotelApp.Domain;

namespace HotelApp.Application.Interfaces
{
    public interface IQuartoRepository
    {
        Task <Quarto?> ObterPorIdAsync(int quartoId);
        Task<List<Quarto>> ObterTodosAsync();
        Task RemoverAsync(int quartoId);
        Task AdicionarAsync(Quarto quarto);

    }
}
