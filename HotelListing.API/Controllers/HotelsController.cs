using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.Models.Hotels;
using AutoMapper;
using HotelListing.API.Interfaces;

namespace HotelListing.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IHotelsRepo _hotelsRepo;

    public HotelsController(
        IMapper mapper,
        IHotelsRepo hotelsRepo 
        )
    {
        _mapper = mapper;
        _hotelsRepo = hotelsRepo;
    }

    // GET: api/Hotels
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelGetDto>>> GetHotels()
    {
        var hotels = await _hotelsRepo.GetAllAsync();
        
        var records = _mapper.Map<List<HotelGetDto>>(hotels);
        
        return Ok(records); 
    }

    // GET: api/Hotels/5
    [HttpGet("{id}")]
    public async Task<ActionResult<HotelDto>> GetHotel(int id)
    {
        var hotel = await _hotelsRepo.GetDetails(id);

        if (hotel == null)
        {
            return NotFound();
        }

        var hotelDto = _mapper.Map<HotelDto>(hotel);

        return Ok(hotelDto);
    }

    // PUT: api/Hotels/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutHotel(int id, HotelGetDto hotelGetDto)
    {
        if (id != hotelGetDto.Id)
        {
            return BadRequest("Invalid Record Id");
        }

        //_context.Entry(hotel).State = EntityState.Modified;
        ///////////////////////////////////////////////////////
        
        var hotel = await _hotelsRepo.GetAsync(id);
        
        if (hotel == null)
        {
            return NotFound();
        }

        _mapper.Map(hotelGetDto, hotel);

        try
        {
            await _hotelsRepo.UpdateAsync(hotel);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await HotelExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Hotels
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Hotel>> PostHotel(HotelCreateDto hotelCreateDto)
    {
        var hotel = _mapper.Map<Hotel>(hotelCreateDto);

        await _hotelsRepo.AddAsync(hotel);
        
        return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
    }

    // DELETE: api/Hotels/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var hotel = await _hotelsRepo.GetAsync(id);
        if (hotel == null)
        {
            return NotFound();
        }

        await _hotelsRepo.DeleteAsync(id);

        return NoContent();
    }

    private async Task<bool> HotelExists(int id)
    {
        return await _hotelsRepo.Exists(id);
    }
}
