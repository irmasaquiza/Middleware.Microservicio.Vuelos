using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Middleware.Vuelos.Business.DTOs.Seguridad;
using Middleware.Vuelos.Business.DTOs.Aeropuertos;
using Middleware.Vuelos.Business.DTOs.Vuelos;
using Middleware.Vuelos.Business.Interfaces;

namespace Middleware.Vuelos.Api.Controllers.V1.Vuelos
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/vuelos/{id_vuelo:int}/escalas")]
    public class EscalasController : ControllerBase
    {
        private readonly IVuelosOrchestrator _vuelosOrchestrator;

        public EscalasController(IVuelosOrchestrator vuelosOrchestrator)
        {
            _vuelosOrchestrator = vuelosOrchestrator;
        }

        /// <summary>
        /// Obtiene las escalas de un vuelo.
        /// GET /api/v1/vuelos/{id_vuelo}/escalas
        /// Público — no requiere JWT.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEscalas([FromRoute] int id_vuelo)
        {
            var result = await _vuelosOrchestrator.GetEscalasAsync(id_vuelo);
            return Ok(new { success = true, data = result });
        }
    }
}