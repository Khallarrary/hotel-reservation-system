using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Exceptions
{
    public class ConflitoPeriodoReservaException : Exception
    {
        public ConflitoPeriodoReservaException(string message, Exception innerException) : base(message, innerException) { }
    }
}
