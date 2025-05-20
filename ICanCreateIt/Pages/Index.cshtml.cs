using ICanCreateIt.Data;
using ICanCreateIt.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ICanCreateIt.Pages
{
    //All users can see publisshed images
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<UserImage> PublishedImages { get; set; }

        public async Task OnGetAsync()
        {
            PublishedImages = await _db.UserImages
                .Where(i => i.IsPublished)
                .OrderByDescending(i => i.Id)
                .ToListAsync();
        }
    }
}
