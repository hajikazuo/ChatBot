using System.Text.Json.Serialization;

namespace ChatBot.Api.Models.Dtos
{
    public class OllamaRequestDto
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; }
        [JsonPropertyName("prompt")]
        public string? Prompt { get; set; }
    }
}
