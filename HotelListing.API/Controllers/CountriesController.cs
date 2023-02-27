using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using AutoMapper;
using HotelListing.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HotelListing.API.Models;
using static HotelListing.API.Models.QueryParameters;
using Microsoft.AspNetCore.OData.Query;
using HotelListing.API.Exceptions;

namespace HotelListing.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ICountriesRepo _countriesRepo;
    private readonly ILogger<CountriesController> _logger;

    public CountriesController(
        IMapper mapper,
        ICountriesRepo countriesRepo,
        ILogger<CountriesController> logger
        )
    {
        _mapper = mapper;
        _countriesRepo = countriesRepo;
        _logger = logger;
    }

    // GET: api/Countries/GetAll
    [HttpGet("GetAll")]
    [EnableQuery]
    public async Task<ActionResult<IEnumerable<CountryGetDto>>> GetCountries()
    {
        var countries = await _countriesRepo.GetAllAsync<CountryGetDto>();
        return Ok(countries); 
    }

    // GET: api/Countries/?StartIndex=0&pagesize=25&PageNumber=1
    [HttpGet]
    public async Task<ActionResult<PageResult<CountryGetDto>>> GetPagedCountries([FromQuery] QueryParameters queryParameters)
    {
        var pageCountriesResult = await _countriesRepo.GetAllAsync<CountryGetDto>(queryParameters);
        return Ok(pageCountriesResult);
    }

    // GET: api/Countries/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CountryDto>> GetCountry(int id)
    {
        var country = await _countriesRepo.GetDetails(id);
        return Ok(country);
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

        try
        {
            await _countriesRepo.UpdateAsync(id, countryGetDto);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await CountryExists(id))
            {
                return NotFound();
            }
            else
            {
                throw new NotFoundException(nameof(countryGetDto.Name), id);
            }
        }

        return NoContent();
    }

    // POST: api/Countries
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CountryDto>> PostCountry(CountryCreateDto countryCreateDto)
    {
        var country = await _countriesRepo.AddAsync<CountryCreateDto, CountryGetDto>(countryCreateDto);
        return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, country);
    }

    // DELETE: api/Countries/5
    [HttpDelete("{id}")]
    [Authorize(Roles ="Administrator")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        await _countriesRepo.DeleteAsync(id);
        return NoContent();
    }

    private async Task<bool> CountryExists(int id)
    {
        return await _countriesRepo.Exists(id);
    }
}
