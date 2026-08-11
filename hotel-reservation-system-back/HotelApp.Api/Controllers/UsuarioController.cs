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
                     (request.Nome, request.Email, request.Senha, request.Perfil);

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

        [Authorize(Roles = "Master")]
        [HttpPost("gestor-hotel")]
        public async Task<IActionResult> CriarGestorAsync([FromBody] CriarGestorHotelDto request)
        {
            try
            {
                await _service.CriarGestorHotel
                     (request.Nome, request.Email, request.Senha, request.HotelId);

                return StatusCode(201);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize(Roles = "Gestor")]
        [HttpGet]
        public async Task<ActionResult<List<UsuarioDto>>> ListarUsuarios()
        {
            
            var usuarios = await _service.ListarUsuarios();

            return Ok(usuarios);
            
        }

        [Authorize(Roles = "Gestor")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> AlterarAtivacaoAsync(int id, [FromBody] AtivoDto request)
        {
           await _service.AlterarAtivacao(id, request.Ativo);
           return NoContent();            
        }

        [Authorize(Roles = "Gestor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> AlterarUsuarioAsync(int id, [FromBody] AlterarUsuarioDto request)
        {
            await _service.AlterarUsuario(request.Nome, request.Email, request.Perfil, id);
            return NoContent();
        }
    }
}
