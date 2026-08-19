using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Interfaces
{
    public interface IRelogioHotel
    {
        DateOnly ObterDataAtual(string fusoHorario);
    }
}
