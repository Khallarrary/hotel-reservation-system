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

        [Authorize(Roles = "Gestor")]
        [HttpGet("gestao")]
        public async Task<ActionResult<List<QuartoGestaoDto>>> ObterParaGestao()
        {
            return Ok(await _service.ObterParaGestao());
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
        public async Task<IActionResult> Post([FromBody] CriarQuartoDto request)
        {
            await _service.Criar(request.Numero, request.CategoriaQuartoId);
            return Ok();
        }

        [Authorize(Roles = "Gestor")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarQuartoDto request)
        {
            await _service.Atualizar(id, request.Numero, request.CategoriaQuartoId);
            return NoContent();
        }

        [Authorize(Roles = "Gestor")]
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

        [Authorize(Roles = "Gestor")]
        [HttpDelete("numero/{numero}")]
        public async Task<IActionResult> DeletePorNumero(string numero)
        {
            await _service.RemoverPorNumero(numero);
            return NoContent();
        }
    }
}
