
using System.Text.Json.Serialization;

namespace Middleware.Vuelos.DataAccess.Models
{
    public class BookingSessionRedirectRequestDto
    {
        [JsonPropertyName("idVuelo")]
        public int IdVuelo { get; set; }

        [JsonPropertyName("idAsientos")]
        public List<int> IdAsientos { get; set; } = [];

        [JsonPropertyName("urlRetorno")]
        public string UrlRetorno { get; set; } = null!;
    }

    public class BookingSessionRedirectResponseDto
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = null!;

        [JsonPropertyName("urlRedirect")]
        public string UrlRedirect { get; set; } = null!;

        [JsonPropertyName("expiracion")]
        public DateTime Expiracion { get; set; }
    }
}