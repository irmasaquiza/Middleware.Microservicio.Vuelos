using Middleware.Vuelos.Api.Extensions;
using Middleware.Vuelos.DataManagement.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── Servicios ──────────────────────────────────────────
builder.Services.AddControllers();

// Seguridad - JWT y HttpClient
builder.Services.AddSeguridadHttpClient(builder.Configuration);
builder.Services.AddBusJwtAuthentication(builder.Configuration);

builder.Services.AddGeografiaHttpClient(builder.Configuration);
// ── Build ──────────────────────────────────────────────
var app = builder.Build();

// ── Pipeline ───────────────────────────────────────────
app.UseHttpsRedirection();

app.UseAuthentication(); // ← antes de Authorization
app.UseAuthorization();
/*
app.MapGet("/test/ping", (HttpContext ctx) =>
{
    var user = ctx.User.Identity?.Name ?? "anónimo";
    var roles = ctx.User.Claims
        .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
        .Select(c => c.Value)
        .ToList();

    return Results.Ok(new
    {
        mensaje = "Bus funcionando",
        usuario = user,
        roles = roles,
        autenticado = ctx.User.Identity?.IsAuthenticated
    });
})
.RequireAuthorization();*/

// prueba de endpoint protegido para verificar geografi 

app.MapGet("/test/geografia/pais/{id}", async (int id, IGeografiaDataService svc) =>
{
    var pais = await svc.GetPaisByIdAsync(id);
    return pais is null ? Results.NotFound("País no encontrado") : Results.Ok(pais);
});

app.MapGet("/test/geografia/ciudad/{id}", async (int id, IGeografiaDataService svc) =>
{
    var ciudad = await svc.GetCiudadByIdAsync(id);
    return ciudad is null ? Results.NotFound("Ciudad no encontrada") : Results.Ok(ciudad);
});

app.MapControllers();
app.Run();