using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs
{
    public class ReservaDto
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string NomeDoHospede { get; set; }
        public int QuartoId { get; set; }

    }
}
