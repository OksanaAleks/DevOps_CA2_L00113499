using ICanCreateIt.Models;
using Microsoft.AspNetCore.Identity;


public class ApplicationUser : IdentityUser
{
    // Custom properties for the user this info will kept in ApplicationDBContext
    public string Name { get; set; }
    public string Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public ICollection<UserImage> UserImages { get; set; }

}


