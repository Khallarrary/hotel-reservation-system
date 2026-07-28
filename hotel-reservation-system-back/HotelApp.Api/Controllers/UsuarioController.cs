using HotelApp.Application.DTOs;
using HotelApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace HotelApp.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {

        private readonly UsuarioService _service;

        public UsuarioController(UsuarioService service) 
        { 
            _service = service;
        }


        [Authorize(Roles = "Gestor")]
        [HttpPost]
        public async Task<IActionResult> CriarUsuarioAsync([FromBody] CriarUsuarioDto request)
        {
            try
            {
                await _service.CriarUsuario
                     (request.Nome, request.Email, request.Senha, request.Perfil, request.HotelId);

                return StatusCode(201);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

       
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginUsuarioDto request)
        {
            try
            {
                var resposta = await _service.Login(request.Email, request.Senha);
                
                return Ok(resposta);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
