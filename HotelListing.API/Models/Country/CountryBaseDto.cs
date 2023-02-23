using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.Country;

public abstract class CountryBaseDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string CountryCode { get; set; }
}
