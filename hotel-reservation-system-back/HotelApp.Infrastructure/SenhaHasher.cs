using HotelApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace HotelApp.Infrastructure
{
    public class SenhaHasher : ISenhaHasher
    {
        
        private readonly PasswordHasher<object> _hasher = new();

        public string GerarSenhaHash(string senha)
        {
            return _hasher.HashPassword(new object(), senha);
        }

        public bool Verificar(string senhaHash, string senha)
        {
            var resultado = _hasher.VerifyHashedPassword(new object(), senhaHash, senha);
            return resultado == PasswordVerificationResult.Success || resultado == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }

}
