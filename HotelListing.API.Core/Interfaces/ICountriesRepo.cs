using HotelListing.API.Data;
using HotelListing.API.Models.Country;

namespace HotelListing.API.Interfaces;

public interface ICountriesRepo : IGenericRepo<Country>
{
    Task<CountryDto> GetDetails(int id);
}
