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
    public class ProfileModel : PageModel
    {
        private readonly DB _db;
        private readonly IPasswordHasher<User> _passwordHasher;

        public ProfileModel(DB db, IPasswordHasher<User> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
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

            await _db.SaveChangesAsync();

            return RedirectToPage("/Index");
        }

    }
}
