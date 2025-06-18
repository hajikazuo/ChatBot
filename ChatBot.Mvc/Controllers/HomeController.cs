using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatBot.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;

        public HomeController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _apiUrl = _configuration["ApiUrl"] ?? throw new ArgumentNullException("Url is missing.");
        }

        public IActionResult Index()
        {
            ViewBag.urlApi = _apiUrl;
            return View();
        }

        public async Task<IActionResult> Chat(string question)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            var json = JsonSerializer.Serialize(question);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/ChatBot/Ask", content);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Erro ao chamar a API.");
            }
            return Ok();
        }
    }
}
