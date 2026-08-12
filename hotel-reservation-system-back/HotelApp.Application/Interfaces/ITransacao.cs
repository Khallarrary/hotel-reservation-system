using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Interfaces
{
    public interface ITransacao
    {
        Task ExecutarAsync(Func<Task> operacao);
    }
}
