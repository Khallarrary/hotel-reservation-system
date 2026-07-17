using HotelApp.Application.DTOs;
using HotelApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HotelApp.Api.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class QuartoController : ControllerBase
    {
        private readonly QuartoService _service;

        public QuartoController(QuartoService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var quartos = await _service.ObterTodos();
            return Ok(quartos);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> getById(int id) {

            var quarto = await _service.ObterPorId(id);

            if (quarto == null) {

                return NotFound();
            }

            return Ok(quarto);
            
        }

        [Authorize(Roles = "Gestor")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] QuartoDto request)
        {
            await _service.Criar(request.numero, request.tipo);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePorId(int id)
        {
            await _service.RemoverPorId(id);
            return NoContent();
        }

        [Authorize]
        [HttpGet("numero/{numero}")]
        public async Task<IActionResult> GetByNumero(string numero)
        {

            var quarto = await _service.ObterPorNumero(numero);

            if (quarto == null)
            {

                return NotFound();
            }

            return Ok(quarto);
        }

        [Authorize]
        [HttpDelete("numero/{numero}")]
        public async Task<IActionResult> DeletePorNumero(string numero)
        {
            await _service.RemoverPorNumero(numero);
            return NoContent();
        }
    }
}
