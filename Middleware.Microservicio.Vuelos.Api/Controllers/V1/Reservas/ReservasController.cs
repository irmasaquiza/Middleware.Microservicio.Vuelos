using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Middleware.Vuelos.Business.DTOs.Reservas;
using Middleware.Vuelos.Business.Interfaces;

namespace Middleware.Vuelos.Api.Controllers.V1.Reservas
{
    // ── ReservasController ────────────────────────────────────────────────────

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/reservas")]
    [Authorize]
    public class ReservasController : ControllerBase
    {
        private readonly IReservaOrchestrator _reservaOrchestrator;

        public ReservasController(IReservaOrchestrator reservaOrchestrator)
        {
            _reservaOrchestrator = reservaOrchestrator;
        }

        /// <summary>
        /// Obtiene una reserva por su id.
        /// GET /api/v1/reservas/{id_reserva}
        /// Roles: ADMINISTRADOR, AEROLINEA, CLIENTE
        /// </summary>
        [HttpGet("{id_reserva:int}")]
        [Authorize(Roles = "ADMINISTRADOR,AEROLINEA,CLIENTE")]
        public async Task<IActionResult> GetById([FromRoute] int id_reserva)
        {
            var token = ObtenerToken();
            var result = await _reservaOrchestrator.GetReservaByIdAsync(
                id_reserva, token);
            return Ok(new { success = true, data = result });
        }

        /// <summary>
        /// Crea una reserva.
        /// POST /api/v1/reservas
        /// Roles: ADMINISTRADOR, AEROLINEA, CLIENTE
        /// El id_cliente se extrae automáticamente del JWT.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMINISTRADOR,AEROLINEA,CLIENTE")]
        public async Task<IActionResult> Crear([FromBody] CrearReservaRequest request)
        {
            var token = ObtenerToken();
            var idCliente = ObtenerIdCliente();

            var result = await _reservaOrchestrator.CrearReservaAsync(
                request, idCliente, token);

            return CreatedAtAction(nameof(GetById),
                new { id_reserva = result.IdReserva },
                new { success = true, message = "Reserva creada correctamente.", data = result });
        }

        /// <summary>
        /// Procesa el pago de una reserva.
        /// PATCH /api/v1/reservas/{id_reserva}/pagar
        /// Roles: ADMINISTRADOR, AEROLINEA, CLIENTE
        /// Coordina MS ReservasF + MS Vuelos con compensaciones automáticas.
        /// </summary>
        [HttpPatch("{id_reserva:int}/pagar")]
        [Authorize(Roles = "ADMINISTRADOR,AEROLINEA,CLIENTE")]
        public async Task<IActionResult> Pagar([FromRoute] int id_reserva)
        {
            var token = ObtenerToken();
            var result = await _reservaOrchestrator.PagarReservaAsync(id_reserva, token);
            return Ok(new { success = true, message = "Pago procesado correctamente.", data = result });
        }

        /// <summary>
        /// Cancela una reserva.
        /// PATCH /api/v1/reservas/{id_reserva}/cancelar
        /// Roles: ADMINISTRADOR, AEROLINEA, CLIENTE
        /// Si la reserva estaba pagada, libera los asientos en MS Vuelos.
        /// </summary>
        [HttpPatch("{id_reserva:int}/cancelar")]
        [Authorize(Roles = "ADMINISTRADOR,AEROLINEA,CLIENTE")]
        public async Task<IActionResult> Cancelar(
            [FromRoute] int id_reserva,
            [FromBody] CancelarReservaRequest request)
        {
            var token = ObtenerToken();
            var result = await _reservaOrchestrator.CancelarReservaAsync(
                id_reserva, request, token);
            return Ok(new { success = true, message = "Reserva cancelada.", data = result });
        }

        /// <summary>
        /// Obtiene los boletos de una reserva.
        /// GET /api/v1/reservas/{id_reserva}/boletos
        /// Roles: ADMINISTRADOR, AEROLINEA, CLIENTE
        /// </summary>
        [HttpGet("{id_reserva:int}/boletos")]
        [Authorize(Roles = "ADMINISTRADOR,AEROLINEA,CLIENTE")]
        public async Task<IActionResult> GetBoletos([FromRoute] int id_reserva)
        {
            var token = ObtenerToken();
            var result = await _reservaOrchestrator.GetBoletosByReservaAsync(
                id_reserva, token);
            return Ok(new { success = true, data = result });
        }

        // ── Helpers privados ──────────────────────────────────────────────────

        private string ObtenerToken() =>
            Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

        private int ObtenerIdCliente()
        {
            var claim = User.Claims
                .FirstOrDefault(c => c.Type == "id_cliente")?.Value;

            if (string.IsNullOrWhiteSpace(claim) || !int.TryParse(claim, out var idCliente))
                throw new UnauthorizedAccessException(
                    "El token no contiene un id_cliente válido.");

            return idCliente;
        }
    }
}