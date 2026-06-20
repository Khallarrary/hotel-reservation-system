using HotelApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs
{
    public class LancarCreditoDto
    {
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
    }
}
