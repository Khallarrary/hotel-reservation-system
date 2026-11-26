using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs
{
   public class CriarQuartoDto
    {
        public string Numero { get; set; } = string.Empty;
        public int CategoriaQuartoId { get; set; }
    }
}
