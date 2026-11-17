using HotelApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Interfaces
{
    public interface IHospedeRepository
    {
        Task AdicionarAsync(Hospede hospede);

        Task<Hospede?> ObterPorIdAsync(int id, int hotelId);

        Task<Hospede?> ObterPorDocumentoAsync(TipoDocumento tipoDocumento, string documentoNormalizado, int hotelId);

        Task AtualizarAsync(Hospede hospede);

        Task<List<Hospede>> ObterPorIdsAsync(IReadOnlyCollection<int> hospedeIds, int hotelId);
    }
}
