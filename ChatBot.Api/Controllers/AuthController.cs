using ChatBot.Api.Models.Users;
using ChatBot.Api.Services;
using ChatBot.Common.Dtos.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RT.Comb;

namespace ChatBot.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _service;
        private readonly ICombProvider _comb;

        public AuthController(UserManager<User> userManager, ITokenService service, ICombProvider comb)
        {
            _userManager = userManager;
            _service = service;
            _comb = comb;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var identityUser = await _userManager.FindByEmailAsync(request.Email);

            if (identityUser is not null)
            {
                var checkPasswordResult = await _userManager.CheckPasswordAsync(identityUser, request.Password);

                if (checkPasswordResult)
                {
                    var roles = await _userManager.GetRolesAsync(identityUser);

                    var jwtToken = _service.CreateJwtToken(identityUser, roles.ToList());
                    var response = new AuthResponseDto()
                    {
                        User = new UserDto
                        {
                            Id = identityUser.Id,
                            Email = identityUser.Email,
                            Roles = roles.ToList()
                        },
                        AuthToken = jwtToken.Token
                    };

                    return Ok(response);
                }
            }
            ModelState.AddModelError("", "Email ou senha inválidos");
            return ValidationProblem(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var user = new User
            {
                Id = _comb.Create(),
                UserName = request.Email?.Trim(),
                Email = request.Email?.Trim(),
            };

            var identityResult = await _userManager.CreateAsync(user, request.Password);

            if (identityResult.Succeeded)
            {
                identityResult = await _userManager.AddToRoleAsync(user, "User");

                if (identityResult.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    var jwtToken = _service.CreateJwtToken(user, roles.ToList());
                    var response = new AuthResponseDto()
                    {
                        User = new UserDto
                        {
                            Id = user.Id,
                            Email = user.Email,
                            Roles = roles.ToList()
                        },
                        AuthToken = jwtToken.Token
                    };

                    return Ok(response);
                }
            }
            else
            {
                if (identityResult.Errors.Any())
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return ValidationProblem(ModelState);
        }

    }
}
