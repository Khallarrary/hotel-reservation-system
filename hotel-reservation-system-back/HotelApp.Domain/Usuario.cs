using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain
{
    public class Usuario
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string SenhaHash { get; private set; }
        public bool Ativo { get; private set; }
        public PerfilUsuario Perfil { get; private set; }

        private Usuario() { }
        public Usuario(string nome, string email, string senhaHash, PerfilUsuario perfil)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("Usuario deve conter um nome valido");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Usuario deve conter um e-mail valido");
            }

            if (string.IsNullOrWhiteSpace(senhaHash))
            {
                throw new ArgumentException("Usuario deve conter uma senha valida");
            }

            Nome = nome;
            Email = email;
            SenhaHash = senhaHash;
            Perfil = perfil;
            Ativo = true;
        }

       
    }
}
