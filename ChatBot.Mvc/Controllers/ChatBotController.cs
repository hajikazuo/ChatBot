using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace ChatBot.Mvc.Controllers
{
    public class ChatBotController : Controller
    {
        private readonly IConfiguration _iconfiguration;
        private readonly string _apiUrl;
        public ChatBotController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            _apiUrl = _iconfiguration["ApiUrl"] ?? throw new ArgumentNullException("Url is missing.");
        }

        public IActionResult Create()
        {
            ViewBag.Confirm = TempData["Confirm"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string message)
        {
            var httpClient = new HttpClient();

            var json = JsonSerializer.Serialize(message);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = await httpClient.PostAsync($"{_apiUrl}/api/ChatBot/Add", content);
            TempData["Confirm"] = request.IsSuccessStatusCode
                ? "<script>$(document).ready(function () {MostraConfirm('Sucesso', 'Cadastrado com sucesso!');})</script>"
                : "<script>$(document).ready(function () {MostraConfirm('Erro', 'Erro ao cadastrar!');})</script>";

            return RedirectToAction(nameof(Create));
        }
    }
}
