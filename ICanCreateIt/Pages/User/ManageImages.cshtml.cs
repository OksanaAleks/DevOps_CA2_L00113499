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
    public class ManageImagesModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        // Constructor to inject dependencies
        public ManageImagesModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _db = db;
            _userManager = userManager;
            _env = env;
        }
        //List of images
        public List<UserImage> Images { get; set; }
        //  Load all images belonging to the logged-in user
        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            Images = _db.UserImages.Where(i => i.UserId == user.Id).ToList();
        }
        [BindProperty]
        public string NewDescription { get; set; }

        [BindProperty]
        public IFormFile NewImage { get; set; }
        public async Task<IActionResult> OnPostTogglePublishAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var image = await _db.UserImages.FindAsync(id);
            if (image == null || image.UserId != user.Id) return NotFound();

            image.IsPublished = !image.IsPublished;
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }
        // Deletes an image and its file 
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var image = await _db.UserImages.FindAsync(id);
            if (image == null || image.UserId != user.Id) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, image.FilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _db.UserImages.Remove(image);
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }
        //  Updates the description of an image
        public async Task<IActionResult> OnPostUpdateDescriptionAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var image = await _db.UserImages.FindAsync(id);
            if (image == null || image.UserId != user.Id) return NotFound();

            image.Description = NewDescription;
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }
        //  Replace the image with a new one
        public async Task<IActionResult> OnPostChangeImageAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var image = await _db.UserImages.FindAsync(id);
            if (image == null || image.UserId != user.Id || NewImage == null) return NotFound();
            //Deteles old image
            var oldPath = Path.Combine(_env.WebRootPath, image.FilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
            //Replace it with a new file 
            var fileName = Path.GetFileName(NewImage.FileName);
            var uploadDir = Path.Combine(_env.WebRootPath, "images", user.Id);
            Directory.CreateDirectory(uploadDir);

            var newPath = Path.Combine(uploadDir, fileName);
            using (var stream = new FileStream(newPath, FileMode.Create))
            {
                await NewImage.CopyToAsync(stream);
            }

            image.FilePath = $"/images/{user.Id}/{fileName}";
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }

    }

}
