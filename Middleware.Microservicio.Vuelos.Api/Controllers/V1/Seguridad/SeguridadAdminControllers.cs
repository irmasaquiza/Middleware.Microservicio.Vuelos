using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Middleware.Vuelos.Business.DTOs.Seguridad;
using Middleware.Vuelos.Business.Interfaces;
using Middleware.Vuelos.DataAccess.Clients;
using Middleware.Vuelos.DataAccess.Models;

namespace Middleware.Vuelos.Api.Controllers.V1.Seguridad
{
    // ── AuthAdminController — login, logout y register ────────────────────────

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthAdminController : ControllerBase
    {
        private readonly SeguridadClient _seguridadClient;
        private readonly ISeguridadOrchestrator _seguridadOrchestrator;

        public AuthAdminController(
            SeguridadClient seguridadClient,
            ISeguridadOrchestrator seguridadOrchestrator)
        {
            _seguridadClient = seguridadClient;
            _seguridadOrchestrator = seguridadOrchestrator;
        }

        /// <summary>
        /// Login — el Bus lo reenvía a MS Seguridad y devuelve el JWT.
        /// POST /api/v1/auth/login
        /// Público — no requiere JWT.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _seguridadOrchestrator.LoginAsync(request);
            return Ok(new { success = true, message = "Login exitoso.", data = result });
        }

        /// <summary>
        /// Cierra la sesión del usuario autenticado.
        /// POST /api/v1/auth/logout
        /// Requiere JWT.
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var token = ObtenerToken();
            var result = await _seguridadClient.LogoutAsync(token);
            return result
                ? Ok(new { success = true, message = "Sesión cerrada correctamente." })
                : BadRequest(new { success = false, message = "No se pudo cerrar la sesión." });
        }

        /// <summary>
        /// Registro público de cliente con cuenta de usuario.
        /// POST /api/v1/auth/register-cliente
        /// Público — no requiere JWT.
        /// </summary>
        [HttpPost("register-cliente")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCliente(
            [FromBody] RegisterClienteRequest request)
        {
            var dto = new RegisterClienteRequestDto
            {
                TipoIdentificacion = request.TipoIdentificacion,
                NumeroIdentificacion = request.NumeroIdentificacion,
                Nombres = request.Nombres,
                Apellidos = request.Apellidos,
                Correo = request.Correo,
                Telefono = request.Telefono,
                Direccion = request.Direccion,
                IdCiudadResidencia = request.IdCiudadResidencia,
                IdPaisNacionalidad = request.IdPaisNacionalidad,
                Username = request.Username,
                Password = request.Password
            };

            var result = await _seguridadClient.RegisterClienteAsync(dto);

            if (result is null)
                return BadRequest(new
                {
                    success = false,
                    message = "No se pudo registrar el cliente. Intenta nuevamente."
                });

            return Created(string.Empty, new
            {
                success = true,
                message = "Cuenta creada correctamente.",
                data = new RegisterClienteResponse
                {
                    IdCliente = result.IdCliente,
                    IdUsuario = result.IdUsuario,
                    Username = result.Username,
                    RolAsignado = result.RolAsignado
                }
            });
        }

        private string ObtenerToken() =>
            Request.Headers["Authorization"]
                .ToString().Replace("Bearer ", "");
    }

    // ── UsuariosAdminController ───────────────────────────────────────────────

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/usuarios")]
    [Authorize(Roles = "ADMINISTRADOR")]
    public class UsuariosAdminController : ControllerBase
    {
        private readonly SeguridadClient _seguridadClient;

        public UsuariosAdminController(SeguridadClient seguridadClient)
        {
            _seguridadClient = seguridadClient;
        }

        /// <summary>
        /// Lista usuarios con filtros y paginación.
        /// GET /api/v1/usuarios
        /// Rol: ADMINISTRADOR
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? username,
            [FromQuery] string? correo,
            [FromQuery] bool? activo,
            [FromQuery] int page = 1,
            [FromQuery] int page_size = 20)
        {
            var token = ObtenerToken();
            var result = await _seguridadClient.GetUsuariosAsync(
                username, correo, activo, page, page_size, token);
            return Ok(new { success = true, data = result });
        }

        /// <summary>
        /// Obtiene un usuario por id.
        /// GET /api/v1/usuarios/{id_usuario}
        /// Rol: ADMINISTRADOR
        /// </summary>
        [HttpGet("{id_usuario:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id_usuario)
        {
            var token = ObtenerToken();
            var result = await _seguridadClient.GetUsuarioByIdAsync(id_usuario, token);
            return result is null
                ? NotFound(new { success = false, message = "Usuario no encontrado." })
                : Ok(new { success = true, data = result });
        }

        /// <summary>
        /// Crea un usuario de aplicación.
        /// POST /api/v1/usuarios
        /// Rol: ADMINISTRADOR
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearUsuarioRequest request)
        {
            var token = ObtenerToken();
            var dto = new CrearUsuarioRequestDto
            {
                IdCliente = request.IdCliente,
                Username = request.Username,
                Correo = request.Correo,
                Password = request.Password
            };
            var result = await _seguridadClient.CrearUsuarioAsync(dto, token);
            return result is null
                ? BadRequest(new { success = false, message = "No se pudo crear el usuario." })
                : Created(string.Empty, new { success = true, data = result });
        }

        /// <summary>
        /// Actualiza correo o contraseña de un usuario.
        /// PUT /api/v1/usuarios/{id_usuario}
        /// Rol: ADMINISTRADOR
        /// </summary>
        [HttpPut("{id_usuario:int}")]
        public async Task<IActionResult> Actualizar(
            [FromRoute] int id_usuario,
            [FromBody] ActualizarUsuarioRequest request)
        {
            var token = ObtenerToken();
            var dto = new ActualizarUsuarioRequestDto
            {
                Correo = request.Correo,
                Password = request.Password
            };
            var result = await _seguridadClient.ActualizarUsuarioAsync(
                id_usuario, dto, token);
            return result is null
                ? NotFound(new { success = false, message = "Usuario no encontrado." })
                : Ok(new { success = true, data = result });
        }

        /// <summary>
        /// Elimina lógicamente un usuario.
        /// DELETE /api/v1/usuarios/{id_usuario}
        /// Rol: ADMINISTRADOR
        /// </summary>
        [HttpDelete("{id_usuario:int}")]
        public async Task<IActionResult> Eliminar([FromRoute] int id_usuario)
        {
            var token = ObtenerToken();
            var result = await _seguridadClient.EliminarUsuarioAsync(id_usuario, token);
            return result
                ? Ok(new { success = true, message = "Usuario eliminado." })
                : NotFound(new { success = false, message = "Usuario no encontrado." });
        }

        /// <summary>
        /// Asigna un rol a un usuario.
        /// POST /api/v1/usuarios/{id_usuario}/roles
        /// Rol: ADMINISTRADOR
        /// </summary>
        [HttpPost("{id_usuario:int}/roles")]
        public async Task<IActionResult> AsignarRol(
            [FromRoute] int id_usuario,
            [FromBody] AsignarRolRequest request)
        {
            var token = ObtenerToken();
            var dto = new AsignarRolRequestDto
            {
                IdUsuario = id_usuario,
                IdRol = request.IdRol
            };
            var result = await _seguridadClient.AsignarRolAsync(id_usuario, dto, token);
            return result is null
                ? BadRequest(new { success = false, message = "No se pudo asignar el rol." })
                : Created(string.Empty, new { success = true, data = result });
        }

        /// <summary>
        /// Remueve un rol de un usuario.
        /// DELETE /api/v1/usuarios/{id_usuario}/roles/{id_rol}
        /// Rol: ADMINISTRADOR
        /// </summary>
        [HttpDelete("{id_usuario:int}/roles/{id_rol:int}")]
        public async Task<IActionResult> RemoverRol(
            [FromRoute] int id_usuario,
            [FromRoute] int id_rol)
        {
            var token = ObtenerToken();
            var result = await _seguridadClient.RemoverRolAsync(id_usuario, id_rol, token);
            return result
                ? Ok(new { success = true, message = "Rol removido." })
                : NotFound(new { success = false, message = "Asignación no encontrada." });
        }

        private string ObtenerToken() =>
            Request.Headers["Authorization"]
                .ToString().Replace("Bearer ", "");
    }
}