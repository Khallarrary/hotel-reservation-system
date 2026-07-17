using HotelApp.Application.DTOs;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace HotelApp.Api.Controllers
{
    public class CaixaController : ControllerBase
    {
        private readonly CaixaService _service;

        public CaixaController(CaixaService service)
        {
            _service = service;
        }

        [Authorize]
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
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [Authorize]
        [HttpPost("reserva/{reservaId}/debito")]
        public async Task<IActionResult> LancarDebito(int reservaId, [FromBody] LancarDebitoDto request)
        {
            try
            {
                await _service.LancarDebito(reservaId, request.Valor, request.Descricao);

                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [Authorize]
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

        [Authorize]
        [HttpGet("reserva/{reservaId}/caixa")]
        public async Task<IActionResult> ResumoCaixa(int reservaId)
        {
            try
            {
                var caixa = await _service.ResumoCaixa(reservaId);
                return Ok(caixa);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpPatch("reserva/{reservaId}/caixa/encerrar")]
        public async Task<IActionResult> EncerrarConta(int reservaId)
        {
            try
            {
                await _service.EncerrarConta(reservaId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
