using HotelListing.API.Models.User;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Interfaces;

public interface IAuthRepo
{
    Task<IEnumerable<IdentityError>> Register(UserDto userDto);
}
