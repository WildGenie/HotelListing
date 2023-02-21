using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.Country;

public class CountryBaseDto
{
    [Required]
    public string Name { get; set; }
    public string CountryCode { get; set; }
}
