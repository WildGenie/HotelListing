using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Data;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public string Country { get; set; }
}
