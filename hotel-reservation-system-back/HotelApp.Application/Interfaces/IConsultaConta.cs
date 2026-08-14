using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Interfaces
{
    public interface IConsultaSaldoConta
    {
        Task<decimal> ObterSaldoAsync(int reservaId);
    }
}
