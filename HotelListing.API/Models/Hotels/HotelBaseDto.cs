using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.Hotels;

public class HotelBaseDto
{
    [Required]
    public string Name { get; set; }
    public int CountryId { get; set; }
    public string Country { get; set; }
}
