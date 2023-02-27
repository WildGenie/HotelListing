using HotelListing.API.Data;

namespace HotelListing.API.Interfaces;

public interface ICountriesRepo : IGenericRepo<Country>
{
    Task<Country> GetDetails(int id);
}
