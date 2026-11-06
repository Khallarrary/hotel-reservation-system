using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs
{
    public class AtualizarHospedeDto
    {
        public string Nome { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefone { get; set; }
    }
}
