using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
