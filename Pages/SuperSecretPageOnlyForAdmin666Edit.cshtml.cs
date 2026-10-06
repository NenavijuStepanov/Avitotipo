using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using webasp.Data;

namespace webasp.Pages
{
    public class SuperSecretPageOnlyForAdmin666EditModel : PageModel
    {
        private readonly DB _db;
        private readonly IWebHostEnvironment _environment;

        public SuperSecretPageOnlyForAdmin666EditModel(DB db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? CurrentImagePath { get; set; }

        public class InputModel
        {

            [Required(ErrorMessage = "Введите заголовок объявления")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Заголовок должен быть от 3 до 50 символов")]
            public string Title { get; set; } = string.Empty;

            [StringLength(300, ErrorMessage = "Описание должно быть не более 300 символов")]
            public string Description { get; set; } = string.Empty;

            public IFormFile? ImageFile { get; set; }


        }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (User.Identity is null) return NotFound();

            if (!(User.Identity.Name == "superpuperadmin666777")) return NotFound();

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToPage("/Login");

            var ad = await _db.Ads.FirstOrDefaultAsync(a => a.Id == id);
            if (ad == null)
                return NotFound();

            Input = new InputModel
            {
                Title = ad.Title,
                Description = ad.Description
            };
            CurrentImagePath = ad.ImagePath;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (User.Identity is null) return NotFound();

            if (!(User.Identity.Name == "superpuperadmin666777")) return NotFound();

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToPage("/Login");

            var ad = await _db.Ads.FirstOrDefaultAsync(a => a.Id == id);
            if (ad == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                CurrentImagePath = ad.ImagePath;
                return Page();
            }

            ad.Title = Input.Title;
            ad.Description = Input.Description;

            if (Input.ImageFile != null && Input.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(ad.ImagePath))
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, ad.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{Input.ImageFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ImageFile.CopyToAsync(fileStream);
                }

                ad.ImagePath = $"/uploads/{uniqueFileName}";
            }



            await _db.SaveChangesAsync();
            return RedirectToPage("/MyAd");
        }


        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (User.Identity is null) return NotFound();

            if (!(User.Identity.Name == "superpuperadmin666777")) return NotFound();

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToPage("/Login");

            var ad = await _db.Ads.FirstOrDefaultAsync(a => a.Id == id);
            if (ad == null)
                return NotFound();


            if (!string.IsNullOrEmpty(ad.ImagePath))
            {
                var filePath = Path.Combine(_environment.WebRootPath, ad.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _db.Ads.Remove(ad);
            await _db.SaveChangesAsync();


            return RedirectToPage("/SuperSecretPageOnlyForAdmin666");
        }
    }
}