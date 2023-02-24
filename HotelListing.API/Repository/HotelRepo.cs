using AutoMapper;
using HotelListing.API.Data;
using HotelListing.API.Interfaces;

namespace HotelListing.API.Repository;

public class HotelRepo : GenericRepo<Hotel>, IHotelsRepo
{
    public HotelRepo(HotelDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}
