using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.User;

public class UserDto : UserLoginDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
}