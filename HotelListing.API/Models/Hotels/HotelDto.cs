namespace HotelListing.API.Models.Hotels;

public class HotelDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Adress { get; set; }
    public double Rating { get; set; }
    public int CountryId { get; set; }
}