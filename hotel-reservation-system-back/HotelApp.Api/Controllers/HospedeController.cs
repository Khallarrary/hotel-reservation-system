using HotelApp.Application.DTOs;
using HotelApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HospedeController : ControllerBase
    {

        private readonly HospedeService _service;

        public HospedeController(HospedeService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<HospedeDto>> Criar([FromBody] CriarHospedeDto request) 
        {
            var hospede = await _service.Criar(request.Nome, request.TipoDocumento, request.Documento, request.Email, request.Telefone);

            return StatusCode(StatusCodes.Status201Created, hospede);
        }

        [Authorize]
        [HttpPost("buscar")]
        public async Task<ActionResult<HospedeDto>> ObterPorDocumento([FromBody] BuscarHospedeDto request)
        {
            var hospede = await _service.ObterPorDocumentoAsync(request.TipoDocumento, request.Documento);

            return Ok(hospede);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarHospedeDto request)
        {
            await _service.Atualizar(id, request.Nome, request.TipoDocumento, request.Documento, request.Email,request.Telefone);

            return NoContent();
        }
    }
}
