using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs
{
    public class LancamentoContaDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string? FormaPagamento { get; set; }
        public DateTime DataLancamento { get; set; }
    }
}
