using Microsoft.AspNetCore.Mvc;
using HotelApp.Application.DTOs;
using HotelApp.Application.Services;


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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var quartos = await _service.ObterTodos();
            return Ok(quartos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getById(int id) {

            var quarto = await _service.ObterPorId(id);

            if (quarto == null) {

                return NotFound();
            }

            return Ok(quarto);
            
        }

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

        [HttpDelete("numero/{numero}")]
        public async Task<IActionResult> DeletePorNumero(string numero)
        {
            await _service.RemoverPorNumero(numero);
            return NoContent();
        }
    }
}
