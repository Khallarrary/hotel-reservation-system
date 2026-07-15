using HotelApp.Application.Interfaces;
using HotelApp.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HotelApp.Application.Services
{
    public class UsuarioService
    {

        private readonly IUsuarioRepository _repo;
        private readonly ISenhaHasher _senhaHasher;

        public UsuarioService(IUsuarioRepository repo, ISenhaHasher senha)
        {
            _repo = repo;
            _senhaHasher = senha;
        }

        public async Task CriarUsuario(string nome, string email, string senha, string perfil)
        {

            if (string.IsNullOrWhiteSpace(nome)) 
            {
                throw new ArgumentException("Nome do usuario é obrigatorio.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email do usuario é obrigatorio.");
            }

            var verificaEmail = await _repo.ObterPorEmailAsync(email);

            if (verificaEmail != null)
            {
                throw new ArgumentException("Email já existente.");
            }

            if (string.IsNullOrWhiteSpace(senha))
            {
                throw new ArgumentException("Senha do usuario é obrigatorio.");
            }

            var senhaHash = _senhaHasher.GerarSenhaHash(senha);

            var perfilValido= Enum.TryParse<PerfilUsuario>(perfil, true, out var perfilConvertido);

            if (!perfilValido)
            {
                throw new ArgumentException("Perfil de usuario invalido.");
            }

            var novo = new Usuario(nome, email, senhaHash, perfilConvertido);


            await _repo.AdicionarAsync(novo);
        }
    }
}
