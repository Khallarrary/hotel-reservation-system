using HotelApp.Application.DTOs;
using HotelApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

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
    }
}
