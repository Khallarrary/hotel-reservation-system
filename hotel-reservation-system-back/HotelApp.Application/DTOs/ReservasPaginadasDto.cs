using HotelApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs
{
    public class ReservasPaginadasDto
    {
        public List<ReservaDto> Itens { get; set; }
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalItens { get; set; }
        public int TotalPaginas { get; set; }
    }
}
