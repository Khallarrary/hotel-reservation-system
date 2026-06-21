using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs
{
    internal class CaixaResumoDto
    {
        public int ReservaId { get; set; }
        public int ContaReservaId { get; set; }
        public string StatusConta { get; set; }
        public decimal TotalDebitos { get; set; }
        public decimal TotalCreditos { get; set; }
        public decimal Saldo { get; set; }
        public List<LancamentoContaDto> Lancamentos { get; set; }
    }
}
