using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Middleware.Vuelos.Business.DTOs.Seguridad;
using Middleware.Vuelos.Business.DTOs.Aeropuertos;
using Middleware.Vuelos.Business.DTOs.Vuelos;
using Middleware.Vuelos.Business.Interfaces;

namespace Middleware.Vuelos.Api.Controllers.V1;

// ============================================================
// AuthController — Login desde el frontend
// ============================================================

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly ISeguridadOrchestrator _seguridadOrchestrator;

    public AuthController(ISeguridadOrchestrator seguridadOrchestrator)
    {
        _seguridadOrchestrator = seguridadOrchestrator;
    }

    /// <summary>
    /// Login — el Bus lo reenvía a MS Seguridad y devuelve el JWT.
    /// El frontend guarda el token y lo envía en cada request posterior.
    /// POST /api/v1/auth/login
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _seguridadOrchestrator.LoginAsync(request);
        return Ok(new { success = true, message = "Login exitoso.", data = result });
    }
}