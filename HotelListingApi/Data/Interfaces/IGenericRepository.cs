using System.Linq.Expressions;

namespace HotelListingApi.Data.Interfaces
{
    public interface IGenericRepository<T> where T : class 
    {
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderdBy = null,
            List<string> includes = null
            );

        Task<T> GetByIdAsync(
            Expression<Func<T, bool>> expression, List<string> includes = null);

        Task CreateAsync(T entity);
        Task CreateRangeAsync(IEnumerable<T> enttities);
        Task DeleteAsync(int id);
        void DeleteRangeAsync(IEnumerable<T> enttities);
        void UpdateAsync(T entity);


    }
}
