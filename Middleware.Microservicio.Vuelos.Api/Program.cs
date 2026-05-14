using Middleware.Vuelos.Api.Extensions;
using Middleware.Vuelos.DataManagement.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── Servicios ──────────────────────────────────────────
builder.Services.AddControllers();

// Seguridad - JWT y HttpClient
builder.Services.AddSeguridadHttpClient(builder.Configuration);
builder.Services.AddBusJwtAuthentication(builder.Configuration);

builder.Services.AddGeografiaHttpClient(builder.Configuration);
builder.Services.AddAeropuertosHttpClient(builder.Configuration);
builder.Services.AddClientesHttpClient(builder.Configuration);
builder.Services.AddVuelosHttpClient(builder.Configuration);
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
/*
app.MapGet("/test/geografia/pais/{id}", async (int id, IGeografiaDataService svc) =>
{
    var pais = await svc.GetPaisByIdAsync(id);
    return pais is null ? Results.NotFound("País no encontrado") : Results.Ok(pais);
});

app.MapGet("/test/geografia/ciudad/{id}", async (int id, IGeografiaDataService svc) =>
{
    var ciudad = await svc.GetCiudadByIdAsync(id);
    return ciudad is null ? Results.NotFound("Ciudad no encontrada") : Results.Ok(ciudad);
});*/


// Endpoint de prueba para verificar que el Bus puede comunicarse con MS Aeropuertos

/*app.MapGet("/test/aeropuertos/{id}", async (int id, IAeropuertosDataService svc) =>
{
    var result = await svc.GetByIdAsync(id);
    return result is null ? Results.NotFound() : Results.Ok(result);
});*/

// Pruenas para el bus de integracion de clientes

app.MapGet("/test/clientes/{id}", async (int id, IClientesDataService svc,
    HttpContext ctx) =>
{
    var token = ctx.Request.Headers["Authorization"]
        .ToString().Replace("Bearer ", "");
    var result = await svc.GetClienteByIdAsync(id, token);
    return result is null ? Results.NotFound() : Results.Ok(result);
});



app.MapGet("/test/pasajeros/{id}", async (
    int id,
    IClientesDataService svc,
    HttpContext ctx) =>
{
    var token = ctx.Request.Headers["Authorization"]
        .ToString().Replace("Bearer ", "");

    if (string.IsNullOrWhiteSpace(token))
        return Results.Unauthorized();

    var result = await svc.GetPasajeroByIdAsync(id, token);
    return result is null ? Results.NotFound("Pasajero no encontrado") : Results.Ok(result);
});


// ── TEST Vuelos ──────────────────────────────────────────────
app.MapGet("/test/vuelos/{id}", async (int id, IVuelosDataService svc) =>
{
    var result = await svc.GetVueloByIdAsync(id);
    return result is null ? Results.NotFound("Vuelo no encontrado") : Results.Ok(result);
});

app.MapGet("/test/vuelos/{id}/valido", async (int id, IVuelosDataService svc) =>
{
    var result = await svc.ValidarVueloOperableAsync(id);
    return result is null ? Results.BadRequest("Vuelo no operable") : Results.Ok(result);
});

// ── TEST Asientos ────────────────────────────────────────────
app.MapGet("/test/vuelos/{id}/asientos", async (int id, IVuelosDataService svc) =>
{
    var result = await svc.GetAsientosByVueloAsync(id);
    return Results.Ok(result);
});

app.MapGet("/test/vuelos/{idVuelo}/asientos/{idAsiento}", async (
    int idVuelo, int idAsiento, IVuelosDataService svc) =>
{
    var result = await svc.GetAsientoByIdAsync(idVuelo, idAsiento);
    return result is null ? Results.NotFound("Asiento no encontrado") : Results.Ok(result);
});

// ── TEST Escalas ─────────────────────────────────────────────
app.MapGet("/test/vuelos/{id}/escalas", async (int id, IVuelosDataService svc) =>
{
    var result = await svc.GetEscalasByVueloAsync(id);
    return Results.Ok(result);
});




app.MapControllers();
app.Run();