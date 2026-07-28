using HotelApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Interfaces
{
    public interface ILancamentoContaRepository
    {
        Task AdicionarAsync(LancamentoConta lancamento);
        Task<List<LancamentoConta>> ListarPorContaReservaIdAsync(int contaReservaId);
        
    }
}
