using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs
{
    public class CriarReservaPorIdDto
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int QuartoId { get; set; }
        public List<CriarReservaHospedeDto> Hospedes { get; set; } = [];
    }
}
