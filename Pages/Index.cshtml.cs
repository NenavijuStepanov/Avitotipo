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


        public List<Ad> AdsList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            UsersList = await _db.Users.AsNoTracking().ToListAsync();


            AdsList = await _db.Ads
                .Include(a => a.User)
                .AsNoTracking()
                .OrderByDescending(a => a.Id) 
                .ToListAsync();
            UsersList = await _db.Users.ToListAsync();
            return Page();
        }



        public async Task<IActionResult> OnPostLogOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Login");
        }

        

    }
}
