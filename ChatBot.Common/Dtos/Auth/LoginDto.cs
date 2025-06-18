using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Common.Dtos.Auth
{
    public class LoginDto
    {
        [Display(Name = "E-mail")]
        [EmailAddress(ErrorMessage = "Email inválido.")]   
        public string Email { get; set; }

        [Display(Name = "Senha")]
        public string Password { get; set; }
    }
}
