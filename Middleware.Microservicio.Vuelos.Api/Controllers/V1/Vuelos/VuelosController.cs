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
    [Route("api/v{version:apiVersion}/vuelos")]
    public class VuelosController : ControllerBase
    {
        private readonly IVuelosOrchestrator _vuelosOrchestrator;

        public VuelosController(IVuelosOrchestrator vuelosOrchestrator)
        {
            _vuelosOrchestrator = vuelosOrchestrator;
        }

        /// <summary>
        /// Busca vuelos disponibles según filtros.
        /// GET /api/v1/vuelos/buscar
        /// Público — no requiere JWT.
        /// </summary>
        [HttpGet("buscar")]
        [AllowAnonymous]
        public async Task<IActionResult> Buscar([FromQuery] BuscarVuelosRequest request)
        {
            var result = await _vuelosOrchestrator.BuscarVuelosAsync(request);
            return Ok(new { success = true, data = result });
        }

        /// <summary>
        /// Obtiene el detalle de un vuelo con sus escalas.
        /// GET /api/v1/vuelos/{id_vuelo}
        /// Público — no requiere JWT.
        /// </summary>
        [HttpGet("{id_vuelo:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromRoute] int id_vuelo)
        {
            var result = await _vuelosOrchestrator.GetVueloByIdAsync(id_vuelo);
            return Ok(new { success = true, data = result });
        }
    }
}