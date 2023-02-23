using AutoMapper;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using HotelListing.API.Models.Hotels;
using HotelListing.API.Models.User;

namespace HotelListing.API.Configurations;

public class MapperConfig : Profile
{
	public MapperConfig()
	{
		CreateMap<Country, CountryCreateDto>().ReverseMap();
        CreateMap<Country, CountryGetDto>().ReverseMap();
        CreateMap<Country, CountryDto>().ReverseMap();

        CreateMap<Hotel, HotelCreateDto>().ReverseMap();
        CreateMap<Hotel, HotelDto>().ReverseMap();

        CreateMap<UserDto, User>().ReverseMap();
    }
}
