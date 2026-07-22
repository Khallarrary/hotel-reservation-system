using HotelApp.Application.DTOs;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Services
{
    public class HotelService
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelService(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public async Task CriarHotel(CriarHotelDto dto)
        {
            var hotel = new Hotel(dto.Nome, dto.Documento, dto.FusoHorario);

            var conferirDoc = await _hotelRepository.ObterPorDocumentoAsync(hotel.Documento);

            if (conferirDoc != null)
            {
                throw new ConflictException("Documento ja existente.");
            }

            
            await _hotelRepository.AdicionarAsync(hotel);
        }
    }
}
