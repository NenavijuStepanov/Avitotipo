using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using webasp.Data;
using webasp.Models;

namespace webasp.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly DB _db;
        private readonly IPasswordHasher<User> _passwordHasher;

        public IndexModel(DB db, IPasswordHasher<User> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        public List<User> UsersList { get; set; } = new();

        public async Task<IActionResult> OnGet()
        {
            UsersList = await _db.Users.ToListAsync();
            return Page();
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Введите пароль")]
            public string OldPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Введите пароль")]
            [StringLength(24, MinimumLength = 8, ErrorMessage = "Пароль должен быть от 8 до 24 символов")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Повторите пароль")]
            [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }



        public async Task<IActionResult> OnPostLogOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Login");
        }

        public async Task<IActionResult> OnPostChangeAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            string password = Input.Password;
            string passwordconfirm = Input.ConfirmPassword;
            
            if (User.Identity == null)
            {
                ModelState.AddModelError(string.Empty, "Что-то пошло не так");
                return Page();
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Input.OldPassword);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Неверный старый пароль");
                return Page();
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, Input.Password);

            // 3. Сохраняем изменения в базе данных MySQL
            await _db.SaveChangesAsync();

            // Перенаправляем на ту же страницу, чтобы сбросить форму
            return RedirectToPage("/Index");
        }

    }
}
