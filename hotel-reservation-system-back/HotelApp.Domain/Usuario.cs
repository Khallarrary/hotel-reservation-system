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
        public int? HotelId { get; private set; }

        private Usuario() { }
        public Usuario(string nome, string email, string senhaHash, PerfilUsuario perfil, int? hotelId)
        {
            if(perfil == PerfilUsuario.Master && hotelId != null)
            {
                throw new ArgumentException("Usuário Master não pode estar vinculado a um hotel.");
            }

            if ((perfil == PerfilUsuario.Gestor || perfil == PerfilUsuario.Operador) && (hotelId == null || hotelId < 1))
            {
                throw new ArgumentException("Usuários Gestor ou Operador devem estar vinculados a um hotel");
            }

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
            HotelId = hotelId;
        }

       public void Ativar() 
       {
            Ativo = true; 
       }

        public void Desativar()
        {
            Ativo = false;
        }

        public void AtualizarDados(string nome, string email, PerfilUsuario perfil)
        {
         
            if(!HotelId.HasValue)
            {
                throw new ArgumentException("Usuário não está vinculado a um hotel.");
            }

            if ((perfil != PerfilUsuario.Gestor && perfil != PerfilUsuario.Operador))
            {
                throw new ArgumentException("Perfil permitido apenas para Gestor ou Operador.");
            }

            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("Usuario deve conter um nome valido");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Usuario deve conter um e-mail valido");
            }

         
            Nome = nome.Trim();
            Email = email.Trim().ToLowerInvariant();
            Perfil = perfil;
            
        }
    }
}
