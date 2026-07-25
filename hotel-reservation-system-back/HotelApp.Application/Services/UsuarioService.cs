using HotelApp.Application.DTOs;
using HotelApp.Application.Exceptions;
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
        private readonly IHotelRepository _hotelRepository;

        public UsuarioService(IUsuarioRepository repo, ISenhaHasher senha, ITokenService token, IHotelRepository hotelRepository)
        {
            _repo = repo;
            _senhaHasher = senha;
            _tokenService = token;
            _hotelRepository = hotelRepository;
        }

        public async Task CriarUsuario(string nome, string email, string senha, string perfil, int hotelId)
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
                throw new ConflictException("Email já existente.");
            }

            if (string.IsNullOrWhiteSpace(senha))
            {
                throw new ArgumentException("Senha do usuario é obrigatorio.");
            }

            var perfilValido = Enum.TryParse<PerfilUsuario>(perfil, true, out var perfilConvertido);

            var perfilPermitido = perfilConvertido == PerfilUsuario.Operador || perfilConvertido == PerfilUsuario.Gestor;

            if (!perfilPermitido)
            {
                throw new ArgumentException("Perfil de usuario invalido.");
            }

            if (!perfilValido)
            {
                throw new ArgumentException("Perfil de usuario invalido.");
            }

            if(perfilConvertido == PerfilUsuario.Master)
            {
                throw new ArgumentException("O perfil Master não pode ser criado por este endpoint.");
            }

            if(hotelId <= 0)
            {
                throw new ArgumentException("Hotel Id invalido.");
            }

            var verificaHotelId = await _hotelRepository.ObterPorIdAsync(hotelId);

            if(verificaHotelId == null)
            {
                throw new NotFoundException("Hotel id invalido.");
            }

            if (!verificaHotelId.Ativo)
            {
                throw new ArgumentException("Hotel Id inativo.");
            }

            var senhaHash = _senhaHasher.GerarSenhaHash(senha);

            var novo = new Usuario(nome, email, senhaHash, perfilConvertido, verificaHotelId.Id);


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
