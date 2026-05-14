using Middleware.Vuelos.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ── Servicios ──────────────────────────────────────────
builder.Services.AddControllers();

// Seguridad - JWT y HttpClient
builder.Services.AddSeguridadHttpClient(builder.Configuration);
builder.Services.AddBusJwtAuthentication(builder.Configuration);

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

app.MapControllers();
app.Run();