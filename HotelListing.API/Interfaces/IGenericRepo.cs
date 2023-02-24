using HotelListing.API.Models;
using static HotelListing.API.Models.QueryParameters;

namespace HotelListing.API.Interfaces;

public interface IGenericRepo<T> where T : class
{
    Task<T> GetAsync(int? id);
    Task<List<T>> GetAllAsync();
    Task<PageResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters);
    Task<T> AddAsync (T entity);
    Task DeleteAsync (int id);
    Task UpdateAsync (T entity);
    Task<bool> Exists(int id);
}
