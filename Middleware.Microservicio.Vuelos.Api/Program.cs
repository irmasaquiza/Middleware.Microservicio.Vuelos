using Middleware.Vuelos.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddBusApiVersioning();
builder.Services.AddBusCors(builder.Configuration);
builder.Services.AddBusSwagger();

// HttpClients
builder.Services.AddSeguridadHttpClient(builder.Configuration);
builder.Services.AddGeografiaHttpClient(builder.Configuration);
builder.Services.AddAeropuertosHttpClient(builder.Configuration);
builder.Services.AddClientesHttpClient(builder.Configuration);
builder.Services.AddVuelosHttpClient(builder.Configuration);
builder.Services.AddReservasFHttpClient(builder.Configuration);

// JWT
builder.Services.AddBusJwtAuthentication(builder.Configuration);

// Orchestrators
builder.Services.AddBusOrchestrators();

var app = builder.Build();

app.UseBusSwagger();
app.UseCors("BusPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();