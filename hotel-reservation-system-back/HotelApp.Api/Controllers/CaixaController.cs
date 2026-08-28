using HotelApp.Application.DTOs;
using HotelApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost("reserva/{reservaId}/credito")]
        public async Task<IActionResult> LancarCredito(int reservaId, [FromBody] LancarCreditoDto request)
        {
            await _service.LancarCredito(reservaId, request.Valor, request.FormaPagamento, request.Descricao);

            return NoContent();
        }

        [Authorize]
        [HttpPost("reserva/{reservaId}/debito")]
        public async Task<IActionResult> LancarDebito(int reservaId, [FromBody] LancarDebitoDto request)
        {
            await _service.LancarDebito(reservaId, request.Valor, request.Descricao);

            return NoContent();
        }

        [Authorize]
        [HttpGet("reserva/{reservaId}/lancamentos")]
        public async Task<IActionResult> ListarLancamento(int reservaId)
        {
            var lancamentos = await _service.ListarLancamentosPorReserva(reservaId);
            return Ok(lancamentos);
        }

        [Authorize]
        [HttpGet("reserva/{reservaId}/caixa")]
        public async Task<IActionResult> ResumoCaixa(int reservaId)
        {
            var caixa = await _service.ResumoCaixa(reservaId);
            return Ok(caixa);
        }

        [Authorize]
        [HttpPatch("reserva/{reservaId}/caixa/encerrar")]
        public async Task<IActionResult> EncerrarConta(int reservaId)
        {
            await _service.EncerrarConta(reservaId);
            return NoContent();
        }
    }
}
