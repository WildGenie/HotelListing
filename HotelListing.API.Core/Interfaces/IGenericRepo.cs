using HotelListing.API.Models;
using static HotelListing.API.Models.QueryParameters;

namespace HotelListing.API.Interfaces;

public interface IGenericRepo<T> where T : class
{
    //Task<T> GetAsync(int? id);
    Task<TResult> GetAsync<TResult>(int? id); 
    //Task<List<T>> GetAllAsync();
    Task<List<TResult>> GetAllAsync<TResult>();
    Task<PageResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters);
    //Task<T> AddAsync (T entity);
    Task<TResult> AddAsync<TSource, TResult> (TSource source);
    Task DeleteAsync (int id);
    //Task UpdateAsync (T entity);
    Task UpdateAsync<TSource> (int id, TSource source);
    Task<bool> Exists(int id);
}
