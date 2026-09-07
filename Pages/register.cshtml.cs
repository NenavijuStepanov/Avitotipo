using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using webasp.Data;
using webasp.Models;

namespace webasp.Pages
{
    public class registerModel : PageModel
    {
        private readonly DB _db;
        private readonly IPasswordHasher<User> _passwordHasher;

        public registerModel(DB db, IPasswordHasher<User> passwordHasher)
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

            [Required(ErrorMessage = "Повторите пароль")]
            [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")] 
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public IActionResult OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated) return RedirectToPage("/Index");
            

            return Page();
        }



        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();


            string username = Input.Username;
            string password = Input.Password;
            string passwordconfirm = Input.ConfirmPassword;


            bool usernameExists = await _db.Users.AnyAsync(u => u.Username == Input.Username);
            if (usernameExists)
            {
                ModelState.AddModelError("Input.Username", "Этот имя пользователя уже занят.");
                return Page();  
            }

            var newUser = new User
            {
                Username = Input.Username,
            };
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, Input.Password);

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new Claim(ClaimTypes.Name, newUser.Username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));



            return RedirectToPage("/Index");
        }
    }
}