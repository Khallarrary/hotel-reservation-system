using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs
{
    public class QuartoDto
    {
        public string Numero { get; set; }
        public string Tipo { get; set; }
        public List<ReservaDto> ReservaList
        {
            get; set;
        }
    }
}