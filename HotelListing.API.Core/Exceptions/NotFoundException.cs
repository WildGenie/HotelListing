using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HotelListing.API.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string name, object key) : base($"{name} ({key}) NOT FOUND")
    {
        
    }
}
