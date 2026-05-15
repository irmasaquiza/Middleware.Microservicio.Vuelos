using Middleware.Vuelos.DataAccess.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Middleware.Vuelos.DataAccess.Clients;

public partial class VuelosClient
{
    // ── Booking endpoints ─────────────────────────────────────────────────────

    /// <summary>
    /// Busca vuelos disponibles para booking.
    /// GET /api/v1/booking/vuelos/buscar
    /// Público.
    /// </summary>
    public async Task<VuelosAdminPagedDto<VueloDto>?> BookingBuscarVuelosAsync(
        string? codigoIataOrigen, string? codigoIataDestino,
        int? idAeropuertoOrigen, int? idAeropuertoDestino,
        DateTime? fechaSalida, int? cantidadPasajeros,
        string? clase, int page, int pageSize)
    {
        var query = $"api/v1/booking/vuelos/buscar?page={page}&page_size={pageSize}";
        if (!string.IsNullOrWhiteSpace(codigoIataOrigen)) query += $"&codigo_iata_origen={codigoIataOrigen}";
        if (!string.IsNullOrWhiteSpace(codigoIataDestino)) query += $"&codigo_iata_destino={codigoIataDestino}";
        if (idAeropuertoOrigen.HasValue) query += $"&id_aeropuerto_origen={idAeropuertoOrigen}";
        if (idAeropuertoDestino.HasValue) query += $"&id_aeropuerto_destino={idAeropuertoDestino}";
        if (fechaSalida.HasValue) query += $"&fecha_salida={fechaSalida:yyyy-MM-dd}";
        if (cantidadPasajeros.HasValue) query += $"&cantidad_pasajeros={cantidadPasajeros}";
        if (!string.IsNullOrWhiteSpace(clase)) query += $"&clase={clase}";

        HttpResponseMessage response;
        try { response = await _httpClient.GetAsync(query); }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException(
                "No se pudo conectar con MS Vuelos.", ex);
        }

        if (!response.IsSuccessStatusCode) return null;
        var body = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer
            .Deserialize<VuelosApiResponseDto<VuelosAdminPagedDto<VueloDto>>>(
                body, _jsonOptions);
        return apiResponse?.Data;
    }

    /// <summary>
    /// Obtiene detalle de vuelo para booking.
    /// GET /api/v1/booking/vuelos/{id_vuelo}
    /// Público.
    /// </summary>
    public async Task<VueloDto?> BookingGetVueloByIdAsync(int idVuelo)
    {
        var endpoint = $"api/v1/booking/vuelos/{idVuelo}";
        HttpResponseMessage response;
        try { response = await _httpClient.GetAsync(endpoint); }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException(
                "No se pudo conectar con MS Vuelos.", ex);
        }

        if (!response.IsSuccessStatusCode) return null;
        var body = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer
            .Deserialize<VuelosApiResponseDto<VueloDto>>(body, _jsonOptions);
        return apiResponse?.Success == true ? apiResponse.Data : null;
    }

    /// <summary>
    /// Obtiene escalas de vuelo para booking.
    /// GET /api/v1/booking/vuelos/{id_vuelo}/escalas
    /// Público.
    /// </summary>
    public async Task<List<EscalaDto>> BookingGetEscalasAsync(int idVuelo)
    {
        var endpoint = $"api/v1/booking/vuelos/{idVuelo}/escalas";
        HttpResponseMessage response;
        try { response = await _httpClient.GetAsync(endpoint); }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException(
                "No se pudo conectar con MS Vuelos.", ex);
        }

        if (!response.IsSuccessStatusCode) return [];
        var body = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer
            .Deserialize<VuelosApiResponseDto<VuelosPagedResultDto<EscalaDto>>>(
                body, _jsonOptions);
        return apiResponse?.Data?.Items ?? [];
    }

    /// <summary>
    /// Obtiene asientos disponibles para booking.
    /// GET /api/v1/booking/vuelos/{id_vuelo}/asientos
    /// Público.
    /// </summary>
    public async Task<List<AsientoDto>> BookingGetAsientosAsync(int idVuelo)
    {
        var endpoint = $"api/v1/booking/vuelos/{idVuelo}/asientos";
        HttpResponseMessage response;
        try { response = await _httpClient.GetAsync(endpoint); }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException(
                "No se pudo conectar con MS Vuelos.", ex);
        }

        if (!response.IsSuccessStatusCode) return [];
        var body = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer
            .Deserialize<VuelosApiResponseDto<VuelosPagedResultDto<AsientoDto>>>(
                body, _jsonOptions);
        return apiResponse?.Data?.Items ?? [];
    }

    /// <summary>
    /// Busca aeropuertos para booking.
    /// GET /api/v1/booking/aeropuertos
    /// Público.
    /// </summary>
    public async Task<List<AeropuertoDto>> BookingBuscarAeropuertosAsync(
        string? search, int? idPais, int limit)
    {
        var query = $"api/v1/booking/aeropuertos?limit={limit}";
        if (!string.IsNullOrWhiteSpace(search)) query += $"&search={search}";
        if (idPais.HasValue) query += $"&id_pais={idPais}";

        HttpResponseMessage response;
        try { response = await _httpClient.GetAsync(query); }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException(
                "No se pudo conectar con MS Vuelos.", ex);
        }

        if (!response.IsSuccessStatusCode) return [];
        var body = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer
            .Deserialize<VuelosApiResponseDto<List<AeropuertoDto>>>(body, _jsonOptions);
        return apiResponse?.Data ?? [];
    }

    /// <summary>
    /// Inicia sesión de redirect a aerolínea.
    /// POST /api/v1/booking/vuelos/sesion-redirect
    /// Rol: BOOKING
    /// </summary>
    public async Task<BookingSessionRedirectResponseDto?> BookingSessionRedirectAsync(
        BookingSessionRedirectRequestDto request, string jwtToken)
    {
        const string endpoint = "api/v1/booking/vuelos/sesion-redirect";
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", jwtToken);
        requestMessage.Content = JsonContent.Create(request);

        HttpResponseMessage response;
        try { response = await _httpClient.SendAsync(requestMessage); }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException(
                "No se pudo conectar con MS Vuelos.", ex);
        }

        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) return null;
        var apiResponse = JsonSerializer
            .Deserialize<VuelosApiResponseDto<BookingSessionRedirectResponseDto>>(
                body, _jsonOptions);
        return apiResponse?.Success == true ? apiResponse.Data : null;
    }
}

// ── DTOs de Booking ───────────────────────────────────────────────────────────
