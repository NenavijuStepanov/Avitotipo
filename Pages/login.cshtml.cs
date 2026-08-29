using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using webasp.Data;
using webasp.Models;

namespace webasp.Pages
{
    public class loginModel : PageModel
    {
        private readonly DB _db;
        private readonly IPasswordHasher<User> _passwordHasher;

        public loginModel(DB db, IPasswordHasher<User> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }


        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Введите имя пользователя")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Имя должно быть от 3 до 50 символов")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Введите пароль")]
            [StringLength(24, MinimumLength = 8, ErrorMessage = "Пароль должен быть от 8 до 24 символов")]
            public string Password { get; set; } = string.Empty;

        }




        public void OnGet()
        {
        }
    }
}
