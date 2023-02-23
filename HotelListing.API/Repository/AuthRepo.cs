﻿using AutoMapper;
using HotelListing.API.Data;
using HotelListing.API.Interfaces;
using HotelListing.API.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HotelListing.API.Repository;

public class AuthRepo : IAuthRepo
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    private User _user;

    private const string _loginProvider = "HotelListing";
    private const string _refreshToken = "RefreshToken";

    public AuthRepo(IMapper mapper, UserManager<User> userManager, IConfiguration configuration)
    {
        _mapper = mapper;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<string> CreateRefreshToken()
    {
        await _userManager.RemoveAuthenticationTokenAsync(
            _user, _loginProvider, _refreshToken);
        
        var newToken = await _userManager.GenerateUserTokenAsync(
            _user, _loginProvider, _refreshToken);
        
        var result = await _userManager.SetAuthenticationTokenAsync(
            _user, _loginProvider, _refreshToken, newToken);

        return newToken;
    }

    public async Task<UserAuthDto> VerifyRefreshToken(UserAuthDto request)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
        var username = tokenContent.Claims.ToList()
            .FirstOrDefault(token => token.Type == JwtRegisteredClaimNames.Email)?.Value;

        _user = await _userManager.FindByNameAsync(username);

        if (_user is null || _user.Id != request.UserId) { return null; }

        var isValidToken = await _userManager.VerifyUserTokenAsync(_user, _loginProvider, _refreshToken, request.RefreshToken);

        var token = await GenerateToken();

        if(isValidToken)
        {
            return new UserAuthDto
            {
                Token = token,
                UserId = _user.Id,
                RefreshToken = await CreateRefreshToken()
            };
        }

        await _userManager.UpdateSecurityStampAsync(_user);

        return null;
    }

    public async Task<UserAuthDto> Login(UserLoginDto userLoginDto)
    {
        _user = await _userManager.FindByEmailAsync(userLoginDto.Email);
        bool isValidCheck = await _userManager.CheckPasswordAsync(_user, userLoginDto.Password);

        if (_user is null || !isValidCheck)
        {
            return null;
        }

        var token = await GenerateToken();

        return new UserAuthDto
        {
            Token = token,
            UserId = _user.Id
        };
    }

    public async Task<IEnumerable<IdentityError>> Register(UserDto userDto)
    {
        _user = _mapper.Map<User>(userDto);
        _user.UserName = userDto.Email;

        var result = await _userManager.CreateAsync(_user, userDto.Password);

        if(result.Succeeded)
        {
           await _userManager.AddToRoleAsync(_user, "User");
        }

        return result.Errors;
    }


    private async Task<string> GenerateToken()
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["JwtSettings:Key"]));

        var credits = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(_user);
        var roleClaims = roles.Select(claim => new Claim(ClaimTypes.Role, claim)).ToList();
        var userClaims = await _userManager.GetClaimsAsync(_user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, _user.Email),
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
            signingCredentials: credits
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    } 
}
