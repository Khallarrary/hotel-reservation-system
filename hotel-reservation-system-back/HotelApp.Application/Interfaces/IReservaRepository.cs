using System;
using System.Collections.Generic;
using System.Text;
using HotelApp.Domain;
using HotelApp.Application.DTOs;

namespace HotelApp.Application.Interfaces
{
    public interface IReservaRepository
    {
        Task<List<Reserva>> ObterReservasPorQuartoAsync(int quartoId, int hotelId);
        Task AdicionarReservaAsync(Reserva reserva);
        Task<List<Reserva>> ListarReservasAsync(int hotelId);
        Task DeletarReservaAsync(Reserva reserva);
        Task<Reserva?> ObterReservaPorIdAsync(int id, int hotelId);
        Task AtualizarReservaAsync(Reserva reserva);
        Task<int> ContarReservasAsync(ReservaConsultaDto consulta, int hotelId);
        Task<List<Reserva>> ListarReservasPaginadasAsync(ReservaConsultaDto consulta, int hotelId);
        
    }
}
