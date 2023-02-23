using AutoMapper;
using HotelListing.API.Data;
using HotelListing.API.Interfaces;
using HotelListing.API.Models.User;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Repository;

public class AuthRepo : IAuthRepo
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public AuthRepo(IMapper mapper, UserManager<User> userManager)
    {
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<IEnumerable<IdentityError>> Register(UserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);
        user.UserName = userDto.Email;

        var result = await _userManager.CreateAsync(user, userDto.Password);

        if(result.Succeeded)
        {
           await _userManager.AddToRoleAsync(user, "User");
        }

        return result.Errors;
    }
}
