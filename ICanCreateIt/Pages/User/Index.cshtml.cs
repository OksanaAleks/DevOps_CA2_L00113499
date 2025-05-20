using ICanCreateIt.Data;
using ICanCreateIt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ICanCreateIt.Pages.User
{
    //Only autorised user have access to this page
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        //User will see their published images if they have any 
        public List<UserImage> PublishedImages { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            PublishedImages = await _db.UserImages
                .Where(img => img.UserId == user.Id && img.IsPublished)
                .ToListAsync();
        }
    }
}
