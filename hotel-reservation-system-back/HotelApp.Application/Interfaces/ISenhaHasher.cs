using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Interfaces
{
    public interface ISenhaHasher
    {
        public string GerarSenhaHash(string senha);
        public bool Verificar(string senhaHash, string senha);
    }
}
