using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using AutoMapper;
using HotelListing.API.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace HotelListing.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ICountriesRepo _countriesRepo;

    public CountriesController(
        IMapper mapper,
        ICountriesRepo countriesRepo
        )
    {
        _mapper = mapper;
        _countriesRepo = countriesRepo;
    }

    // GET: api/Countries
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CountryGetDto>>> GetCountries()
    {
        var countries = await _countriesRepo.GetAllAsync();
        
        var records = _mapper.Map<List<CountryGetDto>>(countries);
        
        return Ok(records); 
    }

    // GET: api/Countries/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CountryDto>> GetCountry(int id)
    {
        var country = await _countriesRepo.GetDetails(id);

        if (country == null)
        {
            return NotFound();
        }

        var countryDto = _mapper.Map<CountryDto>(country);

        return Ok(countryDto);
    }

    // PUT: api/Countries/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutCountry(int id, CountryGetDto countryGetDto)
    {
        if (id != countryGetDto.Id)
        {
            return BadRequest("Invalid Record Id");
        }

        //_context.Entry(country).State = EntityState.Modified;
        ///////////////////////////////////////////////////////
        
        var country = await _countriesRepo.GetAsync(id);
        
        if (country == null)
        {
            return NotFound();
        }

        _mapper.Map(countryGetDto, country);

        try
        {
            await _countriesRepo.UpdateAsync(country);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await CountryExists(id))
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

    // POST: api/Countries
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Country>> PostCountry(CountryCreateDto countryCreateDto)
    {
        var country = _mapper.Map<Country>(countryCreateDto);

        await _countriesRepo.AddAsync(country);
        
        return CreatedAtAction("GetCountry", new { id = country.Id }, country);
    }

    // DELETE: api/Countries/5
    [HttpDelete("{id}")]
    [Authorize(Roles ="Administrator")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var country = await _countriesRepo.GetAsync(id);
        if (country == null)
        {
            return NotFound();
        }

        await _countriesRepo.DeleteAsync(id);

        return NoContent();
    }

    private async Task<bool> CountryExists(int id)
    {
        return await _countriesRepo.Exists(id);
    }
}
