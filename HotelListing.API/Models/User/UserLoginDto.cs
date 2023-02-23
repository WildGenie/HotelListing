using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.User;

public class UserLoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}