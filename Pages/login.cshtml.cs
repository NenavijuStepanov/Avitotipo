using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using webasp.Data;
using webasp.Models;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();


            string username = Input.Username;
            string password = Input.Password;

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == Input.Username);
            if (user == null)
            {
                // Не показываем, ЧТО именно неверно (логин или пароль), из соображений безопасности
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
                return Page();
            }

            // 2. Проверяем соответствие введенного пароля с хэшем из БД
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Input.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
                return Page();
            }


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 3. Записываем куку в браузер
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));



            return RedirectToPage("/Index");
        }
    }
}
