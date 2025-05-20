using ICanCreateIt.Data;
using ICanCreateIt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ICanCreateIt.Pages.User
{
    // Restrict access to authenticated users only
    [Authorize]
    public class UploadImageModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        // Constructor to inject dependencies
        public UploadImageModel(IWebHostEnvironment env, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _env = env;
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public IFormFile Upload { get; set; }

        [BindProperty]
        public string Description { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (Upload != null && Upload.Length > 0)
            {
                var fileName = Path.GetFileName(Upload.FileName);
                var uploadPath = Path.Combine(_env.WebRootPath, "images", user.Id);
                Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Upload.CopyToAsync(stream);
                }

                var image = new UserImage
                {
                    UserId = user.Id,
                    FilePath = $"/images/{user.Id}/{fileName}",
                    Description = Description,
                    UploadedAt = DateTime.Now
                };
                _db.UserImages.Add(image);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage("ManageImages");
        }
    }

}
