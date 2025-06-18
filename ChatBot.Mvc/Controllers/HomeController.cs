using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatBot.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _iconfiguration;
        private readonly string _apiUrl;
        public HomeController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            _apiUrl = _iconfiguration["ApiUrl"] ?? throw new ArgumentNullException("Url is missing.");
        }

        public IActionResult Index()
        {
            ViewBag.urlApi = _apiUrl;
            return View();
        }

        public async Task<IActionResult> Chat(string question)
        {
            var httpClient = new HttpClient();  

            var json = JsonSerializer.Serialize(question);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = await httpClient.PostAsync($"{_apiUrl}/api/ChatBot/Ask", content);
            var response = await request.Content.ReadAsStringAsync();

            return Ok();
        }
    }
}
