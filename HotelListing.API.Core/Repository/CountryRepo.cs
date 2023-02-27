using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.API.Data;
using HotelListing.API.Exceptions;
using HotelListing.API.Interfaces;
using HotelListing.API.Models.Country;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository;

public class CountryRepo : GenericRepo<Country>, ICountriesRepo
{
    private readonly HotelDbContext _context;
    private readonly IMapper _mapper;

    public CountryRepo(HotelDbContext context, IMapper mapper) : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CountryDto> GetDetails(int id)
    {
        var country = await _context.Countries.Include(get => get.Hotels)
            .ProjectTo<CountryDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(get => get.Id == id);

        return country ?? throw new NotFoundException(nameof(GetDetails), id);
    }
}
