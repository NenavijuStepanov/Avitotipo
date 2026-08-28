using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace webasp.Pages
{
    public class registerModel : PageModel
    {


        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }



        public async Task<IActionResult> OnPostAsync()
        {
            string userEmail = Input.Username;
            string userPassword = Input.Password;

            // Здесь будет валидация, хеширование и сохранение в БД...

            return RedirectToPage("/");
        }
    }
}