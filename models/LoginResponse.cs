using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProductWeb.models
{
    public class LoginResponse
    {
        [JsonPropertyName("token")]
        public string AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? Expiration { get; set; }
        // Tambahkan properti lain sesuai response dari API kamu
    }
}