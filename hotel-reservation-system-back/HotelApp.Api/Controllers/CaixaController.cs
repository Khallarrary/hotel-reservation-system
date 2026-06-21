using HotelApp.Application.Exceptions;
using HotelApp.Application.DTOs;
using HotelApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers
{
    public class CaixaController : ControllerBase
    {
        private readonly CaixaService _service;

        public CaixaController(CaixaService service)
        {
            _service = service;
        }

        [HttpPost("reserva/{reservaId}/credito")]
        public async Task<IActionResult> LancarCredito(int reservaId, [FromBody] LancarCreditoDto request)
        {
            try
            {
                await _service.LancarCredito(reservaId, request.Valor, request.FormaPagamento, request.Descricao);

                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            
        }

        [HttpGet("reserva/{reservaId}/lancamentos")]
        public async Task<IActionResult> ListarLancamento(int reservaId)
        {
            try
            {
                var lancamentos = await _service.ListarLancamentosPorReserva(reservaId);
                return Ok(lancamentos);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("reserva/{reservaId}/caixa")]
        public async Task<IActionResult> ResumoCaixa(int reservaId)
        {
            try
            {
                var caixa = await _service.ResumoCaixa(reservaId);
                return Ok(caixa);
            } 
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
