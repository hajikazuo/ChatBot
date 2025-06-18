using ChatBot.Api.Models.Users;
using ChatBot.Common.Dtos.Auth;

namespace ChatBot.Api.Services
{
    public interface ITokenService
    {
        TokenResultDto CreateJwtToken(User user, IList<string> roles);
    }
}
