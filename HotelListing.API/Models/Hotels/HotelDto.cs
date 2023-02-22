namespace HotelListing.API.Models.Hotels;

public class HotelDto : HotelCreateDto
{
    public int Id { get; set; }
    public string Adress { get; set; }
    public double Rating { get; set; }
}