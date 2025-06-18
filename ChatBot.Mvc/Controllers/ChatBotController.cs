using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace ChatBot.Mvc.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class ChatBotController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatBotController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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
            var client = _httpClientFactory.CreateClient("ApiClient");

            var jwtToken = User.Claims.FirstOrDefault(c => c.Type == "JWT")?.Value;

            if (string.IsNullOrEmpty(jwtToken))
            {
                TempData["Confirm"] = "<script>$(document).ready(function () {MostraErro('Erro', 'Token de autenticação ausente!');})</script>";
                return RedirectToAction(nameof(Create));
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
            var json = JsonSerializer.Serialize(message);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/ChatBot/Add", content);
            TempData["Confirm"] = response.IsSuccessStatusCode
                ? "<script>$(document).ready(function () {MostraConfirm('Sucesso', 'Cadastrado com sucesso!');})</script>"
                : "<script>$(document).ready(function () {MostraErro('Erro', 'Erro ao cadastrar!');})</script>";

            return RedirectToAction(nameof(Create));
        }
    }
}
