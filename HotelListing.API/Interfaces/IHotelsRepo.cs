﻿using HotelListing.API.Data;

namespace HotelListing.API.Interfaces;

public interface IHotelsRepo : IGenericRepo<Hotel>
{
    Task<Hotel> GetDetails(int id);
}
