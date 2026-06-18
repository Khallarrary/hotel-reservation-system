using HotelApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Interfaces
{
    public interface IContaReservaRepository
    {
        Task AdicionarAsync(ContaReserva conta);
        Task<ContaReserva?> ObterPorReservaIdAsync(int reservaId);
        Task<ContaReserva?> ObterPorIdAsync(int id);
        Task AtualizarAsync(ContaReserva conta);
    }

}
