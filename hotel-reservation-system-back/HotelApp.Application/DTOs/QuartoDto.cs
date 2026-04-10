using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs
{
    public class QuartoDto
    {
        public string numero { get; set; }
        public string tipo { get; set; }
        public List<ReservaDto>? ReservaList
        {
            get; set;
        }
    }
}