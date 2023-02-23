using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.Hotels;

public abstract class HotelBaseDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Adress { get; set; }
    [Required]
    public int CountryId { get; set; }
}
