using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webasp.Data;
using webasp.Models;
using Microsoft.EntityFrameworkCore;

namespace webasp.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly DB _db;

        public List<User> UsersList { get; set; } = new();

        public IndexModel(DB db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGet()
        {
            UsersList = await _db.Users.ToListAsync();
            return Page();
        }



        public async Task<IActionResult> OnPostAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Login");
        }

    }
}
