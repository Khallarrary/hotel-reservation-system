using HotelApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace HotelApp.Infrastructure
{
    public class RelogioHotel : IRelogioHotel
    {
        
        public DateOnly ObterDataAtual(string fusoHorario)
        {
            var fuso = TimeZoneInfo.FindSystemTimeZoneById(fusoHorario);
            var agoraLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, fuso);

            return DateOnly.FromDateTime(agoraLocal);
        }
    }
}
