namespace HotelListing.API.Models.Hotels;

public class HotelDto : HotelBaseDto
{
    public int Id { get; set; }
    public string Adress { get; set; }
    public double Rating { get; set; }
    public string Country { get; set; }
}