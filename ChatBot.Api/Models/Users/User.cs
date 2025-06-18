using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ChatBot.Api.Models.Users
{
    public class User : IdentityUser<Guid>
    {
    }
}
