using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using AutoMapper;
using HotelListing.API.Interfaces;
using HotelListing.API.Models.Hotels;
using Microsoft.AspNetCore.Authorization;
using HotelListing.API.Models.Country;
using HotelListing.API.Models;
using static HotelListing.API.Models.QueryParameters;

namespace HotelListing.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IHotelsRepo _hotelsRepo;

    public HotelsController(IMapper mapper, IHotelsRepo hotelsRepo)
    {
        _mapper = mapper;
        _hotelsRepo = hotelsRepo;
    }

    // GET: api/Hotels/?
    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels()
    {
        var hotels = await _hotelsRepo.GetAllAsync();
        
        return Ok(_mapper.Map<List<HotelDto>>(hotels));
    }

    // GET: api/Hotels/?StartIndex=0&pagesize=25&PageNumber=1
    [HttpGet]
    public async Task<ActionResult<PageResult<HotelDto>>> GetPagedHotels([FromQuery] QueryParameters queryParameters)
    {
        var pageHotelsResult = await _hotelsRepo.GetAllAsync<HotelDto>(queryParameters);
        return Ok(pageHotelsResult);
    }

    // GET: api/Hotels/5
    [HttpGet("{id}")]
    public async Task<ActionResult<HotelDto>> GetHotel(int id)
    {
        var hotel = await _hotelsRepo.GetAsync(id);

        if (hotel == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<HotelDto>(hotel));
    }

    // PUT: api/Hotels/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutHotel(int id, HotelDto hotelDto)
    {
        if (id != hotelDto.Id)
        {
            return BadRequest();
        }

        //_hotelsRepo.Entry(hotel).State = EntityState.Modified;
        ////////////////////////////////////////////////////////
        
        var hotel = await _hotelsRepo.GetAsync(id);

        if(hotel == null)
        {
            return NotFound();
        }

        _mapper.Map(hotelDto, hotel);
        
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
    [Authorize]
    public async Task<ActionResult<Hotel>> PostHotel(HotelCreateDto hotelCreateDto)
    {
        var hotel = _mapper.Map<Hotel>(hotelCreateDto);

        await _hotelsRepo.AddAsync(hotel);

        return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
    }

    // DELETE: api/Hotels/5
    [HttpDelete("{id}")]
    [Authorize]
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
