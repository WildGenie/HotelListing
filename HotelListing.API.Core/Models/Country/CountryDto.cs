using HotelListing.API.Models.Hotels;

namespace HotelListing.API.Models.Country;

public class CountryDto : CountryBaseDto
{
    public int Id { get; set; }
    public List<HotelDto> Hotels { get; set; }
}
