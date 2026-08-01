using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HotelApp.Application.DTOs
{
    public class CriarGestorHotelDto
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public int HotelId { get; set; }
    }
}
