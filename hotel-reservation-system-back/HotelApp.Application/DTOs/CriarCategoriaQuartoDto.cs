using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs
{
    public class CriarCategoriaQuartoDto
    {
        public string Nome { get; set; } = null!;
        public string? Descricao { get; set; }
        public int CapacidadeFisica { get; set; }
        public int LimiteComercial { get; set; }
    }
}
