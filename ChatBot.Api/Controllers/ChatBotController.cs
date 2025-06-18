using ChatBot.Api.Data;
using ChatBot.Api.Models;
using ChatBot.Api.Models.Dtos;
using ChatBot.Common.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RT.Comb;
using System.Text.Json;

namespace ChatBot.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {
        private IHubContext<ChatHub> _hub;
        private readonly HttpClient _client;
        private readonly ChatBotDbContext _context;
        private readonly ICombProvider _comb;
        private readonly string _baseUrl;

        public ChatBotController(IHubContext<ChatHub> hub, HttpClient client, ChatBotDbContext context, ICombProvider comb, IConfiguration config)
        {
            _hub = hub;
            _client = client;
            _context = context;
            _comb = comb;
            _baseUrl = config["Ollama:BaseUrl"] ?? throw new ArgumentNullException("Ollama:BaseUrl is not configured");
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] string message)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/embeddings");

                var payload = new OllamaRequestDto()
                {
                    Model = "nomic-embed-text",
                    Prompt = message
                };

                request.Content = new StringContent(JsonSerializer.Serialize(payload));
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var embeddingVectorResponse = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>();
                if (embeddingVectorResponse == null || embeddingVectorResponse.Embedding == null)
                {
                    throw new Exception("Embedding vector not generated or returned null.");
                }

                var embedding = new TextEmbedding
                {
                    Message = message,
                    Embedding = FloatArrayToByteArray(embeddingVectorResponse.Embedding)
                };

                embedding.Id = _comb.Create();
                await _context.AddAsync(embedding);
                await _context.SaveChangesAsync();

                return Ok(embedding);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] string question)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/embeddings");
                request.Content = new StringContent(JsonSerializer.Serialize(new OllamaRequestDto()
                {
                    Model = "nomic-embed-text",
                    Prompt = question
                }), System.Text.Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var embeddingResponse = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>();

                if (embeddingResponse == null || embeddingResponse.Embedding == null)
                {
                    throw new Exception("Embedding vector not generated or returned null.");
                }

                var questionEmbedding = embeddingResponse.Embedding;

                var allEmbeddings = await _context.Embeddings.ToListAsync();

                var topMatches = allEmbeddings
                   .Select(e => new
                   {
                       TextEmbedding = e,
                       Score = CosineSimilarity(questionEmbedding, ByteArrayToFloatArray(e.Embedding))
                   })
                   .OrderByDescending(x => x.Score)
                   .Take(3)
                   .ToList();

                if (topMatches.Count == 0)
                {
                    return NotFound("I don't have enough context to answer that, Could you try something different?");
                }

                var context = string.Join("\n---\n", topMatches.Select(x => x.TextEmbedding.Message));
                var prompt = $"Context:\n{context}\n\n Question: {question}\n";

                var chatRequest = new OllamaRequestDto
                {
                    Model = "llama3",
                    Prompt = prompt
                };

                var chatHttpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/generate")
                {
                    Content = new StringContent(JsonSerializer.Serialize(chatRequest), System.Text.Encoding.UTF8, "application/json")
                };
                var chatResponse = await _client.SendAsync(chatHttpRequest);
                chatResponse.EnsureSuccessStatusCode();

                using var stream = await chatResponse.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);

                var fragments = new List<OllamaResponseDto>();
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var fragment = JsonSerializer.Deserialize<OllamaResponseDto>(line);
                        if (fragment != null)
                            fragments.Add(fragment);
                    }
                }

                var fullResponse = string.Join("", fragments.Select(f => f.Response));
                var finalFragment = fragments.LastOrDefault(f => f.Done);

                await _hub.Clients.All.SendAsync("ReceiveMessage", new
                {
                    message = fullResponse
                });

                return Ok(new
                {
                    Answer = fullResponse,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        private static float CosineSimilarity(float[] a, float[] b)
        {
            float dot = 0f, magA = 0f, magB = 0f;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                magA += a[i] * a[i];
                magB += b[i] * b[i];
            }
            return dot / (MathF.Sqrt(magA) * MathF.Sqrt(magB));
        }
        private static float[] ByteArrayToFloatArray(byte[] bytes)
        {
            float[] floats = new float[bytes.Length / sizeof(float)];
            Buffer.BlockCopy(bytes, 0, floats, 0, bytes.Length);
            return floats;
        }

        private static byte[] FloatArrayToByteArray(float[] array)
        {
            var result = new byte[array.Length * sizeof(float)];
            Buffer.BlockCopy(array, 0, result, 0, result.Length);
            return result;
        }
    }
}
