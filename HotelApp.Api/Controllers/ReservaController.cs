using Microsoft.AspNetCore.Mvc;
using HotelApp.Application.DTOs;
using HotelApp.Application.Services;
using HotelApp.Application.Exceptions;
using HotelApp.Domain;

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

        [HttpPost]
        public async Task<IActionResult> CriarReserva([FromBody] ReservaDto request)
        {
            try
            {
                await _service.CriarReserva(
                    request.CheckIn, 
                    request.CheckOut, 
                    request.NomeDoHospede, 
                    request.QuartoId
                    );

                return StatusCode(201);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ConflictException ex) 
            {
                return Conflict(ex.Message);
            }
            
        }

        [HttpGet]
        public async Task<ActionResult<ReservaDto>> Get()
        {
            var reservas = await _service.ListarReservas();


            return Ok(reservas);
        }
    }
}
