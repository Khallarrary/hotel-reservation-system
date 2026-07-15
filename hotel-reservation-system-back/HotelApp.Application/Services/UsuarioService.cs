using HotelApp.Application.DTOs;
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
        private readonly ITokenService _tokenService;

        public UsuarioService(IUsuarioRepository repo, ISenhaHasher senha, ITokenService token)
        {
            _repo = repo;
            _senhaHasher = senha;
            _tokenService = token;
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

        public async Task<LoginRespostaDto> Login(string email, string senha) 
        {

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email é obrigatorio.");
            }

            if (string.IsNullOrWhiteSpace(senha))
            {
                throw new ArgumentException("Senha é obrigatorio.");
            }

            var usuario = await _repo.ObterPorEmailAsync(email);

            if (usuario == null)
            {
                throw new ArgumentException("Email não encontrado.");
                
            }

            if (!usuario.Ativo) 
            {
                throw new ArgumentException("Usuario desativado.");
                
            }

            var verificaSenha = _senhaHasher.Verificar(usuario.SenhaHash, senha);

            if (!verificaSenha)
            {
                throw new ArgumentException("Senha incorreta.");
            }
            
            var token = _tokenService.GerarToken(usuario);

            return new LoginRespostaDto { Token = token, Email = usuario.Email, Nome = usuario.Nome, Perfil = usuario.Perfil.ToString() };

        }
    }
}
