using AutoMapper;
using HotelListing.API.Data;
using HotelListing.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository;

public class CountryRepo : GenericRepo<Country>, ICountriesRepo
{
    private readonly HotelDbContext _context;

    public CountryRepo(HotelDbContext context, IMapper mapper) : base(context, mapper)
    {
        _context = context;
    }

    public async Task<Country> GetDetails(int id)
    {
        return await _context.Countries.Include(get => get.Hotels)
            .FirstOrDefaultAsync(get => get.Id == id);
    }
}
