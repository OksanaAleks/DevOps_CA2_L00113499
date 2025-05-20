using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ICanCreateIt.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;  // <-- Change IdentityUser to ApplicationUser
        private readonly SignInManager<ApplicationUser> _signInManager;  // <-- Change IdentityUser to ApplicationUser

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        // Input model for the registration form
        public class InputModel
        {
            [Required]
            public string Name { get; set; } //input for name

            [Required]
            [EmailAddress]
            public string Email { get; set; }//input for email

            [Required]
            [DataType(DataType.Password)]//input for password
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Gender { get; set; }  // Gender

            [Required]
            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }  //  Date of Birth
        }
        // Create user object with custom ApplicationUser fields
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Name = Input.Name,          // Assign Name
                    Gender = Input.Gender,      // Assign Gender
                    DateOfBirth = Input.DateOfBirth  // Assign DateOfBirth
                };
                // Create user in the database with hashed password
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToPage("/Index");
                }
                // validation errors
                foreach (var error in result.Errors)
                {
                    if (error.Code == "PasswordRequiresNonAlphanumeric")
                    {
                        ModelState.AddModelError("Input.Password", "Password must contain a non-alphanumeric character.");
                    }
                    else if (error.Code == "PasswordRequiresDigit")
                    {
                        ModelState.AddModelError("Input.Password", "Password must contain a digit.");
                    }
                    else if (error.Code == "PasswordRequiresUpper")
                    {
                        ModelState.AddModelError("Input.Password", "Password must contain an uppercase letter.");
                    }
                    else if (error.Code == "PasswordTooShort")
                    {
                        ModelState.AddModelError("Input.Password", $"Password must be at least {6} characters long."); // Use options.Password.RequiredLength
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error.Description); // Default error
                    }
                }
            }
            return Page();
        }
    }
}
