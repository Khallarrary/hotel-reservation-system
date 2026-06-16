using System;
using System.Collections.Generic;
using System.Text;
using HotelApp.Domain;
using HotelApp.Application.DTOs;

namespace HotelApp.Application.Interfaces
{
    public interface IReservaRepository
    {
        Task<List<Reserva>> ObterReservasPorQuartoAsync(int quartoId);
        Task AdicionarReservaAsync(Reserva reserva);
        Task<List<Reserva>> ListarReservasAsync();
        Task DeletarReservaAsync(Reserva reserva);
        Task<Reserva?> ObterReservaPorIdAsync(int id);
        Task AtualizarReservaAsync(Reserva reserva);
    }
}
