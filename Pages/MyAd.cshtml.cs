using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using webasp.Data;
using webasp.Models;

namespace webasp.Pages
{
    [Authorize]
    public class MyadModel : PageModel
    {
        private readonly DB _db;
        private readonly IWebHostEnvironment _environment;

        public MyadModel(DB db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }


        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {

            [Required(ErrorMessage = "Введите заголовок объявления")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Заголовок должен быть от 3 до 50 символов")]
            public string Title { get; set; } = string.Empty;

            [StringLength(300, ErrorMessage = "Описание должно быть не более 300 символов")]
            public string Description { get; set; } = string.Empty;

            public IFormFile? ImageFile { get; set; }


        }


        public List<Ad> AdsList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToPage("/Login");
            }

            AdsList = await _db.Ads
                .Where(a => a.UserId == userId)
                .Include(a => a.User)
                .AsNoTracking()
                .OrderByDescending(a => a.Id)
                .ToListAsync();
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            string? relativeImagePath = null;

            if (Input.ImageFile != null && Input.ImageFile.Length > 0)
            {

                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Input.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ImageFile.CopyToAsync(fileStream);
                }

                relativeImagePath = "/uploads/" + uniqueFileName;
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToPage("/Login");
            }

            var newAd = new Ad
            {
                Title = Input.Title,
                Description = Input.Description,
                ImagePath = relativeImagePath,
                UserId = userId
            };

            _db.Ads.Add(newAd);
            await _db.SaveChangesAsync();

            return RedirectToPage("/Index");
        } 


        

    }
}
