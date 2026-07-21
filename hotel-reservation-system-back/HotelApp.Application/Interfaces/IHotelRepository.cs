using System;
using System.Collections.Generic;
using System.Text;
using HotelApp.Domain;

namespace HotelApp.Application.Interfaces
{
    internal interface IHotelRepository
    {
        Task AdicionarAsync(Hotel hotel);
        Task<Hotel?> ObterPorIdAsync(int id);
        Task<Hotel?> ObterPorDocumentoAsync(string documento);
        Task<List<Hotel>> ListarAsync();
        Task AtualizarAsync(Hotel hotel);
    }
}
