using HotelListing.API.Data;
using HotelListing.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository;

public class GenericRepo<T> : IGenericRepo<T> where T : class
{
    private readonly HotelDbContext _context;

    public GenericRepo(HotelDbContext context)
    {
        _context = context;
    }
    public async Task<T> AddAsync(T entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Exists(int id)
    {
        var entity = await GetAsync(id);
        return entity != null;
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync(); 
    }

    public async Task<T> GetAsync(int? id)
    {
        if(id is null)
        {
            return null;
        }
        
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }
}
