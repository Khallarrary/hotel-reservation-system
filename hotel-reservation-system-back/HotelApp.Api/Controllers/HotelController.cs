using HotelApp.Application.DTOs;
using HotelApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HotelApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly HotelService _service;

        public HotelController(HotelService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Master")]
        [HttpPost]
        public async Task<IActionResult> CriarHotelAsync([FromBody] CriarHotelDto hotel)
        {
            await _service.CriarHotel(hotel);
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
