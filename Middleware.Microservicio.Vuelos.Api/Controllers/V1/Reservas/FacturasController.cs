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
    [Route("api/v{version:apiVersion}/facturas")]
    [Authorize]
    public class FacturasController : ControllerBase
    {
        private readonly IFacturacionOrchestrator _facturacionOrchestrator;

        public FacturasController(IFacturacionOrchestrator facturacionOrchestrator)
        {
            _facturacionOrchestrator = facturacionOrchestrator;
        }

        /// <summary>
        /// Obtiene la factura de una reserva.
        /// GET /api/v1/facturas/reserva/{id_reserva}
        /// Roles: ADMINISTRADOR, AEROLINEA, CLIENTE
        /// </summary>
        [HttpGet("reserva/{id_reserva:int}")]
        [Authorize(Roles = "ADMINISTRADOR,AEROLINEA,CLIENTE")]
        public async Task<IActionResult> GetByReserva([FromRoute] int id_reserva)
        {
            var token = ObtenerToken();
            var result = await _facturacionOrchestrator.GetFacturaByReservaAsync(
                id_reserva, token);
            return Ok(new { success = true, data = result });
        }

        private string ObtenerToken() =>
            Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");
    }
}