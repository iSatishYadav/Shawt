using System.Text.Json.Serialization;

namespace Shawt.Models
{
    public class Jwks
    {
        [JsonPropertyName("keys")]
        public SigningKey[] Keys { get; set; }
    }
}
