using System;
using System.Collections.Generic;
using System.Text;
using HotelApp.Domain;

namespace HotelApp.Application
{
    public interface IReservaRepository
    {
        Task<List<Reserva>> ObterPorQuartoAsync(int quartoId);
        Task AdicionarAsync(Reserva reserva);
    }
}
