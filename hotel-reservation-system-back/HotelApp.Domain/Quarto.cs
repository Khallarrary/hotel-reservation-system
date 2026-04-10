using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain
{
    public class Quarto
    {
        public int Id { get; private set; }
        public string Numero { get; private set; }
        public string Tipo { get; private set; }

        public Quarto(string numero, string tipo) 
        { 
            

            if (string.IsNullOrWhiteSpace(numero))
            {
                throw new ArgumentException("O quarto deve conter um numero");
            }

            Numero = numero;

            if (string.IsNullOrWhiteSpace(tipo))
            {
                throw new ArgumentException("O quarto deve conter um tipo");
            }

            Tipo = tipo;
        
        }
    }
}
