using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs
{
    public class CategoriaQuartoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string? Descricao { get; set; }
        public int CapacidadeFisica { get; set; }
        public int LimiteComercial { get; set; }
        public bool Ativa { get; set; }
    }
}
