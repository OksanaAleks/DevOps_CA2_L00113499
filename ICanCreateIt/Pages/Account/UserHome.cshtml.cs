using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using ICanCreateIt.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ICanCreateIt.Pages.Account
{
    // Restrict access to authenticated users only
    [Authorize]
    public class UserHomeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserHomeModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        // Property to hold the user's display name
        public string Name { get; set; }
        // Retrieve the currently logged-in user
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            Name = user.Name ?? user.Email; // fallback if name is not set
            return Page();
        }
    }
}
