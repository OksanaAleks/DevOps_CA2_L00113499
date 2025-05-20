using ICanCreateIt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ICanCreateIt.Pages.User
{
    // Restrict access to authenticated users only
    [Authorize]
    public class EditProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public EditProfileModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public string DisplayName { get; set; }

        [BindProperty]
        public string Email { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            DisplayName = user.Name;
            Email = user.Email;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (user.Name != DisplayName)
                user.Name = DisplayName;

            if (user.Email != Email)
            {
                user.Email = Email;
                user.UserName = Email; // Updates username if needed
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to update profile.");
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user); // re-login if needed
            return RedirectToPage("Index");
        }
    }
}
