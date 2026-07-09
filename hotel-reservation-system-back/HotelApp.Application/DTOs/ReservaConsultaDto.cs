using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs
{
    public class ReservaConsultaDto
    {
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;
        public string? NomeHospede { get; set; }
        public string? Status { get; set; }
        public string? NumeroQuarto { get; set; }
        public  int? ReservaId { get; set; }
        public DateTime? CheckInDe { get; set; }
        public DateTime? CheckInAte { get; set; }
        public DateTime? CheckOutDe { get; set; }
        public DateTime? CheckOutAte { get; set; }
    }
}
