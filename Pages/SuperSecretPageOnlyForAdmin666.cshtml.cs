using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webasp.Data;
using webasp.Models;

namespace webasp.Pages
{
    public class SuperSecretPageOnlyForAdmin666Model : PageModel
    {
        private readonly DB _db;

        public SuperSecretPageOnlyForAdmin666Model(DB db)
        {
            _db = db;
        }
        public List<Ad> AdsList { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {

            if (User.Identity is null) return NotFound();

            if (!(User.Identity.Name == "superpuperadmin666777")) return NotFound();


            var query = _db.Ads
                .Include(a => a.User)
                .AsNoTracking();
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim();
                query = query.Where(a => a.Title.Contains(term) || a.Description.Contains(term));
            }
            AdsList = await query
                .OrderByDescending(a => a.Id)
                .ToListAsync();

            return Page();
        }
    }
}