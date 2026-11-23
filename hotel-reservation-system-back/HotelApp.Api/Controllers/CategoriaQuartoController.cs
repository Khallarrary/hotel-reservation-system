using HotelApp.Application.DTOs;
using HotelApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaQuartoController : ControllerBase
    {

        private readonly CategoriaQuartoService _service;

        public CategoriaQuartoController(CategoriaQuartoService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Gestor")]
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarCategoriaQuartoDto request)
        {
            await _service.Criar(request.Nome, request.Descricao, request.CapacidadeFisica, request.LimiteComercial);
            return StatusCode(StatusCodes.Status201Created);
        }

        [Authorize(Roles = "Gestor")]
        [HttpPut("{categoriaId:int}")]
        public async Task<IActionResult> Atualizar(int categoriaId, [FromBody] AtualizarCategoriaQuartoDto request)
        {
            await _service.Atualizar(categoriaId, request.Nome, request.Descricao, request.CapacidadeFisica, request.LimiteComercial);
            return NoContent();
        }

        [Authorize(Roles = "Gestor")]
        [HttpPatch("{categoriaId:int}/status")]
        public async Task<IActionResult> AlterarStatus(int categoriaId, [FromBody] AtivoDto request)
        {
            if (request.Ativo)
            {
                await _service.Ativar(categoriaId);
            }
            else
            {
                await _service.Desativar(categoriaId);
            }
            return NoContent();
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<CategoriaQuartoDto>>> ObterTodas()
        {
            return Ok(await _service.ObterTodas());
        }

        [Authorize]
        [HttpGet("ativas")]
        public async Task<ActionResult<List<CategoriaQuartoDto>>> ObterAtivas()
        {
            return Ok(await _service.ObterAtivas());
        }

        [Authorize]
        [HttpGet("{categoriaId:int}")]
        public async Task<ActionResult<CategoriaQuartoDto>> ObterPorId(int categoriaId)
        {
            return Ok(await _service.ObterPorId(categoriaId));
        }


    }
}
