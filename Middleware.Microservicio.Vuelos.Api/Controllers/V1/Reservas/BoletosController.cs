using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Middleware.Vuelos.Business.DTOs.Reservas;
using Middleware.Vuelos.Business.Interfaces;

namespace Middleware.Vuelos.Api.Controllers.V1.Reservas
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/boletos")]
    [Authorize]
    public class BoletosController : ControllerBase
    {
        private readonly IReservaOrchestrator _reservaOrchestrator;

        public BoletosController(IReservaOrchestrator reservaOrchestrator)
        {
            _reservaOrchestrator = reservaOrchestrator;
        }

        /// <summary>
        /// Obtiene los boletos de una reserva.
        /// GET /api/v1/boletos?idReserva={id}
        /// Roles: ADMINISTRADOR, AEROLINEA, CLIENTE
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ADMINISTRADOR,AEROLINEA,CLIENTE")]
        public async Task<IActionResult> GetByReserva([FromQuery] int idReserva)
        {
            var token = ObtenerToken();
            var result = await _reservaOrchestrator.GetBoletosByReservaAsync(
                idReserva, token);
            return Ok(new { success = true, data = result });
        }

        private string ObtenerToken() =>
            Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");
    }
}