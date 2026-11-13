using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs
{
    public class CriarReservaDto
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string NumeroDoQuarto { get; set; } = string.Empty;

        public List<CriarReservaHospedeDto> Hospedes { get; set; } = [];
    }
}
