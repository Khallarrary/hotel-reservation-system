using System;
using System.Collections.Generic;
using System.Text;
using HotelApp.Application.Interfaces;
using HotelApp.Domain;
using Microsoft.EntityFrameworkCore;


namespace HotelApp.Infrastructure
{
    public class HospedeRepository : IHospedeRepository
    {
        private readonly AppDbContext _context;

        public HospedeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Hospede hospede)
        {
            await _context.Hospedes.AddAsync(hospede);
            await _context.SaveChangesAsync();
        }   

        public async Task<Hospede?> ObterPorIdAsync(int id, int hotelId)
        {
            return await _context.Hospedes.FirstOrDefaultAsync(hospede => hospede.Id == id && hospede.HotelId == hotelId);
        }

        public async Task<Hospede?> ObterPorDocumentoAsync(TipoDocumento tipoDocumento, string documentoNormalizado, int hotelId)
        {
            return await _context.Hospedes.FirstOrDefaultAsync(hospede => hospede.TipoDocumento == tipoDocumento && hospede.Documento == documentoNormalizado && hospede.HotelId == hotelId);
        }

        public async Task AtualizarAsync(Hospede hospede)
        {
            _context.Hospedes.Update(hospede);
            await _context.SaveChangesAsync();
        }
    }
}