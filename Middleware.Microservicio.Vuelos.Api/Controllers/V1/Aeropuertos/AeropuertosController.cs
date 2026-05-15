using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Middleware.Vuelos.Business.DTOs.Seguridad;
using Middleware.Vuelos.Business.DTOs.Aeropuertos;
using Middleware.Vuelos.Business.DTOs.Vuelos;
using Middleware.Vuelos.Business.Interfaces;

namespace Middleware.Vuelos.Api.Controllers.V1.Aeropuertos
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/aeropuertos")]
    public class AeropuertosController : ControllerBase
    {
        private readonly IAeropuertosOrchestrator _aeropuertosOrchestrator;

        public AeropuertosController(IAeropuertosOrchestrator aeropuertosOrchestrator)
        {
            _aeropuertosOrchestrator = aeropuertosOrchestrator;
        }

        /// <summary>
        /// Obtiene un aeropuerto por su id.
        /// GET /api/v1/aeropuertos/{id_aeropuerto}
        /// Público — no requiere JWT.
        /// </summary>
        [HttpGet("{id_aeropuerto:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromRoute] int id_aeropuerto)
        {
            var result = await _aeropuertosOrchestrator.GetByIdAsync(id_aeropuerto);
            return Ok(new { success = true, data = result });
        }

        /// <summary>
        /// Obtiene un aeropuerto por código IATA.
        /// GET /api/v1/aeropuertos/iata/{codigo}
        /// Público — no requiere JWT.
        /// </summary>
        [HttpGet("iata/{codigo}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIata([FromRoute] string codigo)
        {
            var result = await _aeropuertosOrchestrator.GetByCodigoIataAsync(codigo);
            return Ok(new { success = true, data = result });
        }
    }
}