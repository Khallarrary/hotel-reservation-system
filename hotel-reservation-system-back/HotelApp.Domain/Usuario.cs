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


        public Usuario(string nome, string email, string senhaHash)
        {
            Nome = nome;
            Email = email;
            SenhaHash = senhaHash;
            Ativo = true;
        }
    }
}
