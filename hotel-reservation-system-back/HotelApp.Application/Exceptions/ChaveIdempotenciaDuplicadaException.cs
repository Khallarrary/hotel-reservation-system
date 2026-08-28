using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Exceptions
{
    public class ChaveIdempotenciaDuplicadaException : Exception
    {
        public ChaveIdempotenciaDuplicadaException(string  message, Exception innerException) : base(message, innerException) { }
    }
}
