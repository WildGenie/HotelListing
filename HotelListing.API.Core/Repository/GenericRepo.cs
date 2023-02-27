using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.API.Data;
using HotelListing.API.Interfaces;
using HotelListing.API.Models;
using Microsoft.EntityFrameworkCore;
using static HotelListing.API.Models.QueryParameters;
using HotelListing.API.Exceptions;

namespace HotelListing.API.Repository;

public class GenericRepo<T> : IGenericRepo<T> where T : class
{
    private readonly HotelDbContext _context;
    private readonly IMapper _mapper;

    public GenericRepo(HotelDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    /*public async Task<T> AddAsync(T entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }*/

    public async Task<TResult> AddAsync<TSource, TResult>(TSource source)
    {
        var entity = _mapper.Map<T>(source);

        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();

        return _mapper.Map<TResult>(entity);
    }

    /*public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync(); 
    }*/

    public async Task<List<TResult>> GetAllAsync<TResult>()
    {
        return await _context.Set<T>()
            .ProjectTo<TResult>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<PageResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters)
    {
        var totalSize = await _context.Set<T>().CountAsync();
        var items = await _context.Set<T>()
            .Skip(queryParameters.StartIndex)
            .Take(queryParameters.PageSize)
            .ProjectTo<TResult>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PageResult<TResult>
        {
            Items = items,
            PageNumber = queryParameters.PageNumber,
            RecordNumber = queryParameters.PageSize,
            TotalCount = totalSize
        };
    }

    /*public async Task<T> GetAsync(int? id)
    {
        if(id is null)
        {
            return null;
        }
        
        return await _context.Set<T>().FindAsync(id);
    }*/

    public async Task<TResult> GetAsync<TResult>(int? id)
    {
        var results = await _context.Set<T>().FindAsync(id);

        return results is null
            ? throw new NotFoundException(typeof(T).Name, id.HasValue ? id : "No Key Provided")
            : _mapper.Map<TResult>(results);
    }

    /*public async Task UpdateAsync(T entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }*/

    public async Task UpdateAsync<TSource>(int id, TSource source)
    {
        var entity = await GetAsync<T>(id) ?? throw new NotFoundException(typeof(T).Name, id);

        _mapper.Map(source, entity);

        _context.Update(entity);
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetAsync<T>(id) ?? throw new NotFoundException(typeof(T).Name, id);
        
        _context.Set<T>().Remove(entity);
        
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Exists(int id)
    {
        var entity = await GetAsync<T>(id);
        return entity != null;
    }
}
