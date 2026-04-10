using System;
using System.Collections.Generic;
using System.Text;
using HotelApp.Domain;
using HotelApp.Application.DTOs;

namespace HotelApp.Application.Interfaces
{
    public interface IReservaRepository
    {
        Task<List<Reserva>> ObterPorQuartoAsync(int quartoId);
        Task AdicionarAsync(Reserva reserva);
        Task<List<Reserva>> ListarReservasAsync();
    }
}
