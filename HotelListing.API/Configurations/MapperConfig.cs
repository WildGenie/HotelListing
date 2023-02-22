using AutoMapper;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using HotelListing.API.Models.Hotels;

namespace HotelListing.API.Configurations;

public class MapperConfig : Profile
{
	public MapperConfig()
	{
		CreateMap<Country, CountryCreateDto>().ReverseMap();
        CreateMap<Country, CountryGetDto>().ReverseMap();
        CreateMap<Country, CountryDto>().ReverseMap();

        CreateMap<Hotel, HotelCreateDto>().ReverseMap();
        CreateMap<Hotel, HotelGetDto>().ReverseMap();
        CreateMap<Hotel, HotelDto>().ReverseMap();
    }
}
