using HotelApp.Application.DTOs;
using HotelApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservaController : ControllerBase
    {
        private readonly ReservaService _service;

        public ReservaController(ReservaService service) { 
        
            _service = service;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CriarReserva(
            [FromBody] ReservaDto request,
            [FromHeader(Name = "Idempotency-Key")] Guid chaveIdempotencia)
        {
            await _service.CriarReserva(
                request.CheckIn,
                request.CheckOut,
                request.NomeDoHospede,
                request.QuartoId,
                chaveIdempotencia
                );

            return StatusCode(201);
        }

        [Authorize]
        [HttpPost ("numero")]
        public async Task<IActionResult> CriarReservaPorNumero(
            [FromBody] CriarReservaDto request,
            [FromHeader(Name = "Idempotency-Key")] Guid chaveIdempotencia)
        {
            await _service.CriarReservaPorNumero(
                request.CheckIn,
                request.CheckOut,
                request.NomeDoHospede,
                request.NumeroDoQuarto,
                chaveIdempotencia
                );

            return StatusCode(201);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ReservaDto>> Get()
        {
            var reservas = await _service.ListarReservas();


            return Ok(reservas);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarReserva(int id)
        {
            await _service.CancelarReserva(id);
            return NoContent();
        }

        [Authorize]
        [HttpPatch("{id}/check-in")]
        public async Task<IActionResult> RealizarCheckIn(int id)
        {
            await _service.RealizarCheckIn(id);
            return NoContent();
        }

        [Authorize]
        [HttpPatch("{id}/check-out")]
        public async Task<IActionResult> RealizarCheckOut(int id)
        {
            await _service.RealizarCheckOut(id);
            return NoContent();
        }

        [Authorize]
        [HttpGet("paginadas")]
        public async Task<ActionResult<ReservasPaginadasDto>> GetReservasPaginadas([FromQuery]ReservaConsultaDto consulta)
        {
            var reservasPaginadas = await _service.ListarReservasPaginadas(consulta);

            return Ok(reservasPaginadas);
        }
    }
}
