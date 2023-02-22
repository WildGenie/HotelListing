using HotelListing.API.Data;
using HotelListing.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository;

public class HotelRepo : GenericRepo<Hotel>, IHotelsRepo
{
    private readonly HotelDbContext _context;

    public HotelRepo(HotelDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Hotel> GetDetails(int id)
    {
        return await _context.Hotels.Include(get => get.Country)
            .FirstOrDefaultAsync(get => get.Id == id);
    }
}
